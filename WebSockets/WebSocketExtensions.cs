using System.Reflection;

namespace DotWebRTC.WebSockets;

public static class WebSocketExtensions
{
    public static IApplicationBuilder MapWebSocketHandler(this IApplicationBuilder app, PathString path, WebSocketHandler? handler)
        =>
            app.Map(path, appBuilder => appBuilder.UseMiddleware<WebSocketMiddleware>(handler));



    public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
    {
        services.AddTransient<WebSocketManager>();

        foreach (var type in Assembly.GetEntryAssembly()?.ExportedTypes ?? [])
        {
            if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                services.AddSingleton(type);
        }

        return services;
    }
}