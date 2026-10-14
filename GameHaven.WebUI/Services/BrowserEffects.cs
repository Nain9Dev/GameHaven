using Microsoft.JSInterop;

namespace GameHaven.WebUI.Services;

/// <summary>
/// Thin wrapper over wwwroot/js/gamehaven.js. Every call is best-effort: if the
/// module fails to load or the circuit is already gone, the page still renders,
/// it just renders flat.
/// </summary>
public sealed class BrowserEffects : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public BrowserEffects(IJSRuntime js)
    {
        _js = js;
    }

    public Task InitHeroAsync(string canvasId) => InvokeAsync("initHero", canvasId);

    public Task DisposeHeroAsync() => InvokeAsync("disposeHero");

    public Task InitTiltAsync(string containerSelector) => InvokeAsync("initTilt", containerSelector);

    public Task DisposeTiltAsync() => InvokeAsync("disposeTilt");

    public Task InitRevealAsync() => InvokeAsync("initReveal");

    public Task ScrollToTopAsync() => InvokeAsync("scrollToTop");

    public Task ScrollIntoViewAsync(string elementId) => InvokeAsync("scrollIntoView", elementId);

    private async Task InvokeAsync(string identifier, params object?[] args)
    {
        try
        {
            _module ??= await _js.InvokeAsync<IJSObjectReference>("import", "./js/gamehaven.js");
            await _module.InvokeVoidAsync(identifier, args);
        }
        catch (JSException ex)
        {
            // Effects are decoration and must never break a page, but swallowing
            // them without a trace once hid a hero that silently failed to start.
            Console.Error.WriteLine($"[effects] {identifier} failed: {ex.Message}");
        }
        catch (ObjectDisposedException)
        {
        }
        catch (InvalidOperationException)
        {
            // Prerender or teardown: no JS runtime available on this call.
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is null) return;

        try
        {
            await _module.DisposeAsync();
        }
        catch (JSException)
        {
        }
        catch (ObjectDisposedException)
        {
        }

        _module = null;
    }
}
