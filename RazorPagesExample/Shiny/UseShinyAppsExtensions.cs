using System.Reflection;

public static class UseShinyAppsExtensions
{

    public static IApplicationBuilder RegisterShinyApps(this IApplicationBuilder applicationBuilder, params Assembly[] assemblies)
    {
        // could do better here!
        ShinyAppLookup.AddAssembly(Assembly.GetExecutingAssembly());
        foreach (var assembly in assemblies)
        {
            ShinyAppLookup.AddAssembly(assembly);
        }

        return applicationBuilder;
    }

    public static IApplicationBuilder ProcessShinyWebSockets(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Use(async (context, next) =>
        {
            var requestPath = context.Request.Path.ToString();
            if (requestPath.EndsWith("/websocket/")) // hard coded path in Shiny
            {
                // this could go wrong... could match the middle of a path - but is OK for now
                var withoutWebsocket = requestPath.Replace("/websocket/", "/");

                if (ShinyAppLookup.Constructors.TryGetValue(withoutWebsocket, out var constructorInfo))
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var shinyApp = (ShinyAppBase)constructorInfo.Invoke(new[] { webSocket });
                        await shinyApp.RunApp();
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                }
            }
            else
            {
                await next(context);
            }
        });

        return applicationBuilder;
    }
}

