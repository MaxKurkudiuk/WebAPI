using Microsoft.AspNetCore.Authentication.Negotiate;
using Serilog;
using WebAPI.Infrastructure.Middleware;
using WebAPI.Application.Interfaces;
using WebAPI.Application.Jobs;
using WebAPI.Infrastructure.Queues;

var builder = WebApplication.CreateBuilder(args);

ConfigureBuilder(builder);

var app = builder.Build();

ConfigureApp(app);

app.Run();

static void ConfigureBuilder(WebApplicationBuilder builder) {
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProcessId()
        .Enrich.WithThreadId()
        .WriteTo.Console()
        .CreateLogger();

    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllers();

    builder.Services.AddSingleton<IJobQueue, InMemoryJobQueue>();
    builder.Services.AddHostedService<JobProcessor>();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
       .AddNegotiate();

    builder.Services.AddAuthorization(options => {
        // By default, all incoming requests will be authorized according to the default policy.
        options.FallbackPolicy = options.DefaultPolicy;
    });
}

static void ConfigureApp(WebApplication app) {
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment()) {
        app.MapOpenApi();
    }
    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}