using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PanoramicData.JsonMagic.Web.Interfaces;
using PanoramicData.JsonMagic.Web.Services;
using Sotsera.Blazor.Toaster.Core.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
	.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
	.AddScoped<IToastService, ToastService>()
	.AddToaster(config =>
	{
		config.PositionClass = Defaults.Classes.Position.BottomRight;
		config.PreventDuplicates = false;
		config.ShowTransitionDuration = 500;
		config.HideTransitionDuration = 500;
	});

await builder.Build().RunAsync();
