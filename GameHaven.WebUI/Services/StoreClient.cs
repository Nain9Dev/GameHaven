using System.Net.Http.Json;
using System.Text.Json;
using GameHaven.WebUI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GameHaven.WebUI.Services;

public enum StoreMode
{
    Connecting,
    Live,
    Offline
}

/// <summary>
/// Single entry point the pages use to read the catalogue and mutate the cart.
///
/// The public demo runs against a free-tier container that sleeps after a period
/// of inactivity and needs up to a minute to wake up. Rather than showing a
/// spinner for that whole time, the client falls back to a catalogue snapshot
/// shipped with the app and keeps cart and library in localStorage, so a visitor
/// can always browse and check out. <see cref="RetryAsync"/> promotes the session
/// back to <see cref="StoreMode.Live"/> once the API answers.
/// </summary>
public sealed class StoreClient
{
    private static readonly TimeSpan ApiTimeout = TimeSpan.FromSeconds(8);

    private const string OfflineCartKey = "gamehaven.offline.cart";
    private const string OfflineLibraryKey = "gamehaven.offline.library";

    private readonly HttpClient _http;
    private readonly UserService _users;
    private readonly IJSRuntime _js;
    private readonly NavigationManager _navigation;

    private readonly List<CartLine> _cart = new();
    private readonly HashSet<Guid> _owned = new();

    private Task? _initialization;

    public StoreClient(HttpClient http, UserService users, IJSRuntime js, NavigationManager navigation)
    {
        _http = http;
        _users = users;
        _js = js;
        _navigation = navigation;
    }

    public StoreMode Mode { get; private set; } = StoreMode.Connecting;

    public IReadOnlyList<GameSummary> Catalog { get; private set; } = Array.Empty<GameSummary>();

    public IReadOnlyList<CartLine> Cart => _cart;

    public int CartCount => _cart.Count;

    public decimal CartTotal => _cart.Sum(line => line.PriceAtAddedTime);

    public int LibraryCount => _owned.Count;

    /// <summary>Raised whenever the catalogue, cart, library or mode changes.</summary>
    public event Action? Changed;

    public bool Owns(Guid gameId) => _owned.Contains(gameId);

    public bool InCart(Guid gameId) => _cart.Any(line => line.GameId == gameId);

    public GameSummary? Find(Guid gameId) => Catalog.FirstOrDefault(game => game.Id == gameId);

    /// <summary>
    /// Loads catalogue, cart and library once per session. Concurrent callers
    /// await the same task, so three pages rendering at once still make one trip.
    /// </summary>
    public Task EnsureInitializedAsync() => _initialization ??= InitializeAsync();

    /// <summary>Re-attempts the API after an offline start. Safe to call repeatedly.</summary>
    public async Task RetryAsync()
    {
        Mode = StoreMode.Connecting;
        Notify();

        _initialization = InitializeAsync();
        await _initialization;
    }

    private async Task InitializeAsync()
    {
        var catalog = await TryLoadCatalogFromApiAsync();

        if (catalog is not null)
        {
            Catalog = catalog;
            Mode = StoreMode.Live;
            await LoadLiveCartAndLibraryAsync();
        }
        else
        {
            Catalog = await LoadCatalogSnapshotAsync();
            Mode = StoreMode.Offline;
            await LoadOfflineCartAndLibraryAsync();
        }

        Notify();
    }

    private async Task<List<GameSummary>?> TryLoadCatalogFromApiAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(ApiTimeout);
            var games = await _http.GetFromJsonAsync<List<GameSummary>>("api/games", cts.Token);
            return games is { Count: > 0 } ? games : null;
        }
        catch (Exception)
        {
            // Asleep, rate limited, offline or CORS-blocked: all mean the same
            // thing to the storefront, which is that the snapshot takes over.
            return null;
        }
    }

    private async Task<List<GameSummary>> LoadCatalogSnapshotAsync()
    {
        try
        {
            // Absolute URL on purpose: the shared HttpClient points at the API host,
            // and the snapshot is served next to the app itself.
            var snapshotUrl = new Uri(new Uri(_navigation.BaseUri), "data/catalog.json");
            var snapshot = await _http.GetFromJsonAsync<List<GameSummary>>(snapshotUrl);
            return snapshot ?? new List<GameSummary>();
        }
        catch (Exception)
        {
            return new List<GameSummary>();
        }
    }

    private async Task LoadLiveCartAndLibraryAsync()
    {
        var userId = await _users.GetUserIdAsync();

        _cart.Clear();
        _owned.Clear();

        try
        {
            var cart = await _http.GetFromJsonAsync<CartResponse>($"api/cart/{userId}");
            if (cart?.Items is not null) _cart.AddRange(cart.Items);

            var library = await _http.GetFromJsonAsync<LibraryResponse>($"api/library/{userId}");
            if (library?.OwnedGames is not null)
                foreach (var owned in library.OwnedGames) _owned.Add(owned.GameId);
        }
        catch (Exception)
        {
            // A live catalogue with an unreadable cart is still a usable store.
        }
    }

    private async Task LoadOfflineCartAndLibraryAsync()
    {
        _cart.Clear();
        _owned.Clear();

        var storedCart = await ReadLocalAsync<List<CartLine>>(OfflineCartKey);
        if (storedCart is not null)
        {
            // Drop lines whose game is no longer in the snapshot.
            _cart.AddRange(storedCart.Where(line => Catalog.Any(game => game.Id == line.GameId)));
        }

        var storedLibrary = await ReadLocalAsync<List<Guid>>(OfflineLibraryKey);
        if (storedLibrary is not null)
        {
            foreach (var id in storedLibrary) _owned.Add(id);
        }
    }

    public async Task<StoreResult> AddToCartAsync(Guid gameId)
    {
        var game = Find(gameId);
        if (game is null) return StoreResult.Fail("Ese juego ya no está en el catálogo.");
        if (Owns(gameId)) return StoreResult.Fail($"Ya tienes {game.Title} en tu biblioteca.");
        if (InCart(gameId)) return StoreResult.Fail($"{game.Title} ya está en el carrito.");

        if (Mode == StoreMode.Live)
        {
            var response = await _http.PostAsJsonAsync(
                $"api/cart/{await _users.GetUserIdAsync()}/add",
                new { GameId = gameId });

            if (!response.IsSuccessStatusCode)
                return StoreResult.Fail(await ReadProblemAsync(response));
        }

        _cart.Add(new CartLine { GameId = gameId, PriceAtAddedTime = game.CurrentPrice });
        await PersistOfflineStateAsync();
        Notify();

        return StoreResult.Ok($"{game.Title} añadido al carrito.");
    }

    public async Task<StoreResult> RemoveFromCartAsync(Guid gameId)
    {
        var game = Find(gameId);

        if (Mode == StoreMode.Live)
        {
            var response = await _http.PostAsJsonAsync(
                $"api/cart/{await _users.GetUserIdAsync()}/remove",
                new { GameId = gameId });

            if (!response.IsSuccessStatusCode)
                return StoreResult.Fail(await ReadProblemAsync(response));
        }

        _cart.RemoveAll(line => line.GameId == gameId);
        await PersistOfflineStateAsync();
        Notify();

        return StoreResult.Ok($"{game?.Title ?? "El juego"} se ha quitado del carrito.");
    }

    public async Task<StoreResult> CheckoutAsync()
    {
        if (_cart.Count == 0) return StoreResult.Fail("El carrito está vacío.");

        var purchased = _cart.Count;

        if (Mode == StoreMode.Live)
        {
            var response = await _http.PostAsync(
                $"api/cart/{await _users.GetUserIdAsync()}/checkout",
                content: null);

            if (!response.IsSuccessStatusCode)
                return StoreResult.Fail(await ReadProblemAsync(response));
        }

        foreach (var line in _cart) _owned.Add(line.GameId);
        _cart.Clear();

        await PersistOfflineStateAsync();
        Notify();

        var noun = purchased == 1 ? "juego" : "juegos";
        return StoreResult.Ok($"Compra completada. {purchased} {noun} en tu biblioteca.");
    }

    private static async Task<string> ReadProblemAsync(HttpResponseMessage response)
    {
        if ((int)response.StatusCode == 429)
            return "Demasiadas peticiones seguidas. Espera unos segundos.";

        var body = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(body) ? "La operación no se ha podido completar." : body;
    }

    /// <summary>
    /// Offline state lives in localStorage so a reload keeps the demo cart. In live
    /// mode the API already owns the state, so this is a no-op.
    /// </summary>
    private async Task PersistOfflineStateAsync()
    {
        if (Mode != StoreMode.Offline) return;

        await WriteLocalAsync(OfflineCartKey, _cart);
        await WriteLocalAsync(OfflineLibraryKey, _owned.ToList());
    }

    private async Task<T?> ReadLocalAsync<T>(string key)
    {
        try
        {
            var raw = await _js.InvokeAsync<string?>("localStorage.getItem", key);
            return string.IsNullOrWhiteSpace(raw) ? default : JsonSerializer.Deserialize<T>(raw);
        }
        catch (Exception)
        {
            return default; // private mode, blocked storage or corrupt payload
        }
    }

    private async Task WriteLocalAsync<T>(string key, T value)
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
        }
        catch (Exception)
        {
            // Storage is a convenience here, never a requirement.
        }
    }

    private void Notify() => Changed?.Invoke();

    private sealed class CartResponse
    {
        public List<CartLine> Items { get; set; } = new();
    }

    private sealed class LibraryResponse
    {
        public List<OwnedGameResponse> OwnedGames { get; set; } = new();
    }

    private sealed class OwnedGameResponse
    {
        public Guid GameId { get; set; }
    }
}

public readonly record struct StoreResult(bool Succeeded, string Message)
{
    public static StoreResult Ok(string message) => new(true, message);
    public static StoreResult Fail(string message) => new(false, message);
}
