using BlazorContextMenu;
using BlazorContextMenu.BlazorTestApp.Client;
using BlazorContextMenu.BlazorTestApp.Client.Services;
using BlazorContextMenu.TestAppsCommon;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using BlazorContextMenu.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<ISampleDataService, SampleDataService>();
builder.Services.AddBlazorContextMenu(options =>
{
    options.ConfigureTemplate(defaultTemplate =>
    {
        defaultTemplate.MenuCssClass = "my-menu";
        defaultTemplate.MenuItemCssClass = "my-menu-item";
        defaultTemplate.SeparatorCssClass = "my-menu-separator";
    });

    options.ConfigureTemplate("red", template =>
    {
        template.MenuCssClass = "red-menu";
        template.MenuItemCssClass = "red-menu-item";
        template.MenuItemDisabledCssClass = "red-menu-item--disabled";
        template.SeparatorHrCssClass = "red-menu-separator-hr";
        template.MenuItemWithSubMenuCssClass = "red-menu-item--with-submenu";
        template.Animation = Animation.FadeIn;
    });

    options.ConfigureTemplate("dark", template =>
    {
        template.MenuCssClass = "dark-menu";
        template.MenuItemCssClass = "dark-menu-item";
        template.MenuItemDisabledCssClass = "dark-menu-item--disabled";
        template.SeparatorHrCssClass = "dark-menu-separator-hr";
        template.MenuItemWithSubMenuCssClass = "dark-menu-item--with-submenu";
        template.Animation = Animation.FadeIn;
    });
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
