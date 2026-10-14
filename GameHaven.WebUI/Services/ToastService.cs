namespace GameHaven.WebUI.Services;

public enum ToastLevel
{
    Info,
    Success,
    Error
}

public sealed record Toast(Guid Id, string Message, ToastLevel Level);

/// <summary>
/// Replaces the browser alert() the demo used to block on. Toasts stack in the
/// corner and retire themselves after a few seconds.
/// </summary>
public sealed class ToastService
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(4);

    private readonly List<Toast> _toasts = new();

    public IReadOnlyList<Toast> Toasts => _toasts;

    public event Action? Changed;

    public void Success(string message) => Show(message, ToastLevel.Success);

    public void Error(string message) => Show(message, ToastLevel.Error);

    public void Info(string message) => Show(message, ToastLevel.Info);

    public void Show(string message, ToastLevel level)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        var toast = new Toast(Guid.NewGuid(), message, level);
        _toasts.Add(toast);
        Changed?.Invoke();

        _ = RetireAsync(toast);
    }

    public void Dismiss(Guid id)
    {
        if (_toasts.RemoveAll(toast => toast.Id == id) > 0) Changed?.Invoke();
    }

    private async Task RetireAsync(Toast toast)
    {
        await Task.Delay(Lifetime);
        Dismiss(toast.Id);
    }
}
