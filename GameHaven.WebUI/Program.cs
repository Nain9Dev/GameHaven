using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameHaven.WebUI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

using GameHaven.WebUI.Services;

var apiBaseAddress = builder.HostEnvironment.IsDevelopment() 
    ? "http://localhost:5000" 
    : "https://gamehaven-97og.onrender.com";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
builder.Services.AddScoped<UserService>();

await builder.Build().RunAsync();
