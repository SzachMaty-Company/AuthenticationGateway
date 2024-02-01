using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
            .AddJsonFile("ocelot.json")
            .AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "all",
        policy => policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddOcelot();

builder.Logging.AddConsole();

//builder.WebHost.UseIISIntegration();


//Application pipeline
var app = builder.Build();

app.UseCors("all");

app.UseWebSockets();

await app.UseOcelot(new OcelotPipelineConfiguration
{
    PreQueryStringBuilderMiddleware = async (context, next) =>
    {
        Console.WriteLine($"Request: {context.Request.Path}");
        await next.Invoke();
    },
    PreAuthenticationMiddleware = async (context, next) =>
    {
        await next.Invoke();
    }
});

app.Run();