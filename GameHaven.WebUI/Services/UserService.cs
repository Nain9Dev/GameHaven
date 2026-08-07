using Microsoft.JSInterop;

namespace GameHaven.WebUI.Services;

public class UserService
{
    private readonly IJSRuntime _js;
    private Guid? _userId;

    public UserService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<Guid> GetUserIdAsync()
    {
        if (_userId.HasValue)
            return _userId.Value;

        var storedId = await _js.InvokeAsync<string>("localStorage.getItem", "demoUserId");
        if (string.IsNullOrEmpty(storedId) || !Guid.TryParse(storedId, out var guid))
        {
            guid = Guid.NewGuid();
            await _js.InvokeVoidAsync("localStorage.setItem", "demoUserId", guid.ToString());
        }

        _userId = guid;
        return _userId.Value;
    }
}
