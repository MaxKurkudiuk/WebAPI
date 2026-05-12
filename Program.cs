using Microsoft.AspNetCore.Authentication.Negotiate;
using Serilog;
using WebAPI.Application.Interfaces;
using WebAPI.Application.Jobs;
using WebAPI.Application.Jobs.Processing;
using WebAPI.Infrastructure.Middleware;
using WebAPI.Infrastructure.Persistence;
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

    // ASP.NET Core Health Checks part 1
    //builder.Services.AddHealthChecks();
    //builder.AddSqlServer(connectionString);
    //builder.AddAzureServiceBusQueue(...);

    // Add services to the container.
    builder.Services.AddControllers();

    builder.Services.AddSingleton<IJobQueue, InMemoryJobQueue>();
    builder.Services.AddSingleton<IJobStore, InMemoryJobStore>();

    builder.Services.AddHostedService<JobProcessor>();

    builder.Services.AddSingleton<IFileProcessor, CsvFileProcessor>();
    builder.Services.AddSingleton<IFileProcessor, ExcelFileProcessor>();


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

    // ASP.NET Core Health Checks part 2
    //app.MapHealthChecks("/health");

    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}