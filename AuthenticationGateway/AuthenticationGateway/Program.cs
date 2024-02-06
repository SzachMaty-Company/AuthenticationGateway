using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
            .AddJsonFile("ocelot.json")
            .AddEnvironmentVariables();

var authenticationProviderKey = "JwtBearer";
builder.Services
    .AddAuthentication()
    .AddJwtBearer(authenticationProviderKey, 
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"]
        };
    });


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

app.UseAuthentication();

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