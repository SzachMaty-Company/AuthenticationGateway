using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;

new WebHostBuilder()
    .UseKestrel()
    .UseContentRoot(Directory.GetCurrentDirectory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config
            .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
            .AddJsonFile("ocelot.json")
            .AddEnvironmentVariables();
    })
    .ConfigureServices(services => {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "all",
                policy => policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });
        services.AddOcelot();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        //add your logging
        logging.AddConsole();
    })
    .UseIISIntegration()
    .Configure(app =>
    {
        app.UseCors("all");
        app.UseWebSockets();
        app.UseOcelot(new OcelotPipelineConfiguration
        {
            PreQueryStringBuilderMiddleware = async (context, next) =>
            {
                // Log information on the console before processing the request
                Console.WriteLine($"Request: {context.Request.Path}");
                await next.Invoke();
            },
            PreAuthenticationMiddleware = async (context, next) =>
            {
                // Add any pre-authentication logic here if needed
                await next.Invoke();
            }
            // Add other middleware as needed
        }).Wait();
    })
    .Build()
    .Run();