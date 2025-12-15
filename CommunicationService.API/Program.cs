using CommunicationService.Application.Interfaces;
using CommunicationService.Application.Services;
using CommunicationService.Domain.Interfaces;
using CommunicationService.ExternalServices;
using CommunicationService.ExternalServices.WhatsApp;
using CommunicationService.Infrastructure.Consumers;
using CommunicationService.Jobs;
using CommunicationService.Persistence;
using CommunicationService.Persistence.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Refit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║           COMMUNICATION SERVICE - Starting...                ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

var connectionString = builder.Configuration.GetConnectionString("CommunicationDb")!;

// DbContext - PostgreSQL
builder.Services.AddDbContext<CommunicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositories
builder.Services.AddScoped<IMessageLogRepository, MessageLogRepository>();
builder.Services.AddScoped<IMessageTemplateRepository, MessageTemplateRepository>();
builder.Services.AddScoped<IScheduledMessageRepository, ScheduledMessageRepository>();

// Application Services
builder.Services.AddScoped<ICommunicationService, CommunicationAppService>();
builder.Services.AddScoped<ITemplateRenderer, TemplateRenderer>();
builder.Services.AddScoped<IChannelSenderFactory, ChannelSenderFactory>();

// Channel Senders
builder.Services.AddScoped<IChannelSender, WhatsAppSender>();

// External Services - MessageService (WhatsApp)
builder.Services.AddRefitClient<IMessageServiceClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(
        builder.Configuration["ExternalServices:MessageService:BaseUrl"] ?? "http://localhost:5275"));

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CommunicationRequestConsumer>();
    x.AddConsumer<CancelScheduledMessagesConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", 
            ushort.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672"), 
            builder.Configuration["RabbitMQ:VirtualHost"] ?? "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        // Retry via RabbitMQ: 3 tentativas com intervalo exponencial (5s, 10s, 15s)
        // Após 3 falhas → mensagem vai para fila _error (Dead Letter Queue)
        cfg.UseMessageRetry(r => r.Exponential(
            retryLimit: 3,
            minInterval: TimeSpan.FromSeconds(5),
            maxInterval: TimeSpan.FromMinutes(1),
            intervalDelta: TimeSpan.FromSeconds(5)));

        cfg.ConfigureEndpoints(context);
    });
});

// Hangfire - PostgreSQL
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

builder.Services.AddHangfireServer();

// Jobs
builder.Services.AddScoped<ScheduledMessageJob>();
builder.Services.AddScoped<RetryFailedMessagesJob>();
builder.Services.Configure<RetrySettings>(builder.Configuration.GetSection("Retry"));

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "database")
    .AddRabbitMQ(rabbitConnectionString: 
        $"amqp://{builder.Configuration["RabbitMQ:Username"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:Host"]}:{builder.Configuration["RabbitMQ:Port"]}/", 
        name: "rabbitmq");

var app = builder.Build();

// Aplicar migrações automaticamente
var logger = app.Services.GetRequiredService<ILogger<Program>>();

Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
Console.WriteLine("│  📦 Applying database migrations...                          │");
Console.WriteLine("└──────────────────────────────────────────────────────────────┘");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CommunicationDbContext>();
    try
    {
        context.Database.Migrate();
        Console.WriteLine("│  ✅ Database migrations applied successfully                 │");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"│  ❌ Failed to apply migrations: {ex.Message}");
        throw;
    }
}


Console.WriteLine();
Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
Console.WriteLine("│  🔧 Service Configuration                                    │");
Console.WriteLine("├──────────────────────────────────────────────────────────────┤");
Console.WriteLine($"│  Database:    PostgreSQL (Port 5436)                         │");
Console.WriteLine($"│  RabbitMQ:    {builder.Configuration["RabbitMQ:Host"]}:{builder.Configuration["RabbitMQ:Port"]}                                    │");
Console.WriteLine($"│  Hangfire:    Enabled (Dashboard: /hangfire)                 │");
Console.WriteLine($"│  Swagger:     Enabled (/swagger)                             │");
Console.WriteLine("└──────────────────────────────────────────────────────────────┘");
Console.WriteLine();

// Swagger - habilitado em todos os ambientes
app.UseSwagger();
app.UseSwaggerUI();

// Health Check
app.MapHealthChecks("/health");

app.UseAuthorization();
app.MapControllers();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// Configurar Jobs Recorrentes
RecurringJob.AddOrUpdate<ScheduledMessageJob>(
    "process-scheduled-messages",
    job => job.ProcessPendingMessagesAsync(),
    "*/5 * * * *");

RecurringJob.AddOrUpdate<RetryFailedMessagesJob>(
    "retry-failed-messages",
    job => job.ProcessFailedMessagesAsync(),
    "*/10 * * * *");

Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
Console.WriteLine("│  ⏰ Scheduled Jobs                                           │");
Console.WriteLine("├──────────────────────────────────────────────────────────────┤");
Console.WriteLine("│  • Process Scheduled Messages    → Every 5 minutes           │");
Console.WriteLine("│  • Retry Failed Messages         → Every 10 minutes          │");
Console.WriteLine("└──────────────────────────────────────────────────────────────┘");
Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  🚀 COMMUNICATION SERVICE READY                              ║");
Console.WriteLine("║                                                              ║");
Console.WriteLine("║  API:       http://localhost:5000                            ║");
Console.WriteLine("║  Swagger:   http://localhost:5000/swagger                    ║");
Console.WriteLine("║  Hangfire:  http://localhost:5000/hangfire                   ║");
Console.WriteLine("║  Health:    http://localhost:5000/health                     ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

app.Run();
