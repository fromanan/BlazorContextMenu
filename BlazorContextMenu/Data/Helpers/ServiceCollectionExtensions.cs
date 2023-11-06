using BlazorContextMenu;
using BlazorContextMenu.Services;
using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorContextMenu(this IServiceCollection services)
    {
        services.AddSingleton<BlazorContextMenuSettings>();

        CommonRegistrations(services);
        return services;
    }

    public static IServiceCollection AddBlazorContextMenu(this IServiceCollection services,
        Action<BlazorContextMenuSettingsBuilder> settings)
    {
        BlazorContextMenuSettings settingsObj = new();
        BlazorContextMenuSettingsBuilder settingsBuilder = new(settingsObj);
        settings(settingsBuilder);
        services.AddSingleton(settingsObj);

        CommonRegistrations(services);
        return services;
    }

    private static void CommonRegistrations(IServiceCollection services)
    {
        services.AddSingleton<IMenuTreeTraverser>(_ => new MenuTreeTraverser());
        services.AddScoped<IInternalContextMenuHandler, InternalContextMenuHandler>();
        services.AddScoped<IContextMenuStorage, ContextMenuStorage>();
        services.AddScoped<IBlazorContextMenuService, BlazorContextMenuService>();
    }
}