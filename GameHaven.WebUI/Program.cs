using GameHaven.WebUI;
using GameHaven.WebUI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Local ports come from the machine-wide registry D:\Development\projects.json:
// 5340 for this client, 8180 for the API. The default 5000/5001 pair
// is deliberately avoided because other software on the workstation claims it.
// docs/60-runbook.md carries the copy that matters for this repository.
var apiBaseAddress = builder.HostEnvironment.IsDevelopment()
    ? "http://localhost:8180"
    : "https://gamehaven-97og.onrender.com";

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<StoreClient>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<BrowserEffects>();

await builder.Build().RunAsync();
