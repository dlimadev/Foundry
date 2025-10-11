using Foundry.Api.BuildingBlocks.Localization;
using Foundry.Api.BuildingBlocks.Middlewares;
using Foundry.Domain.Interfaces.EventSourcing;
using Foundry.Domain.Notifications;
using Foundry.Infrastructure.Logging;
using Foundry.Infrastructure.Persistence.EventSourcing.Implementations;
using Marten;
using Sample.FinancialMarket.Api.Resources;
using Sample.FinancialMarket.Application;
using Sample.FinancialMarket.Infrastructure;
using Foundry.Api.BuildingBlocks.Extensions;
using Foundry.Api.BuildingBlocks.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseFoundrySerilog();

builder.Services.AddScoped<INotificationHandler, NotificationHandler>();
builder.Services.AddSampleApplication();
builder.Services.AddSampleInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Sample.FinancialMarket.Application.DependencyInjection).Assembly));

builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("PostgresConnection"));

    options.Events.DatabaseSchemaName = "event_sourcing";

    options.Events.AddEventTypes(new[] { typeof(Sample.FinancialMarket.Domain.Aggregates.Orders.Order) });

}).UseLightweightSessions();


builder.Services.AddScoped<IEventStore, MartenEventStore>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "SampleAPI_";
});

// --- Internationalization (I18N) Configuration ---
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddScoped<ILocalizerService, LocalizerService<SharedResources>>();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var localizationSettings = builder.Configuration.GetSection("LocalizationSettings");
    var defaultCulture = localizationSettings["DefaultCulture"] ?? "en-US";
    var supportedCultures = localizationSettings.GetSection("SupportedCultures").Get<string[]>() ?? new[] { "en-US" };

    options.SetDefaultCulture(defaultCulture)
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

builder.Services.AddFoundryApiVersioning();
builder.Services.AddFoundrySwagger("Financial Market API");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFoundryObservability(builder.Configuration); 
builder.Services.AddFoundryTelemetry(builder.Configuration);

// --- 3. Build the Application ---
var app = builder.Build();

// --- 4. Configure the HTTP Request Pipeline ---
app.UseFoundryExceptionHandler(app.Environment);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseFoundryCorrelationId();
app.UseRequestLocalization();
app.UseAuthorization();
app.MapControllers();

// --- 5. Ensure Marten schema is up to date (only in dev/test) ---
using (var scope = app.Services.CreateScope())
{
    var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();

    if (app.Environment.IsDevelopment())
    {
        store.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();
    }
}

// --- 6. Run the Application ---
app.Run();
