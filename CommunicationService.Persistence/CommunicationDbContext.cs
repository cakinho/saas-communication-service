using CommunicationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunicationService.Persistence;

public class CommunicationDbContext : DbContext
{
    public CommunicationDbContext(DbContextOptions<CommunicationDbContext> options) : base(options)
    {
    }

    public DbSet<CommunicationEvent> CommunicationEvents => Set<CommunicationEvent>();
    public DbSet<MessageTemplate> MessageTemplates => Set<MessageTemplate>();
    public DbSet<MessageLog> MessageLogs => Set<MessageLog>();
    public DbSet<ScheduledMessage> ScheduledMessages => Set<ScheduledMessage>();
    public DbSet<BusinessCommunicationSettings> BusinessCommunicationSettings => Set<BusinessCommunicationSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCommunicationEvent(modelBuilder);
        ConfigureMessageTemplate(modelBuilder);
        ConfigureMessageLog(modelBuilder);
        ConfigureScheduledMessage(modelBuilder);
        ConfigureBusinessCommunicationSettings(modelBuilder);
        SeedDefaultTemplates(modelBuilder);
    }

    private static void ConfigureCommunicationEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommunicationEvent>(entity =>
        {
            entity.ToTable("communication_events");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(50);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id").HasMaxLength(50).IsRequired();
            entity.Property(e => e.EventType).HasColumnName("event_type").HasMaxLength(100).IsRequired();
            entity.Property(e => e.SourceService).HasColumnName("source_service").HasMaxLength(50).IsRequired();
            entity.Property(e => e.SourceEntityId).HasColumnName("source_entity_id").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Payload).HasColumnName("payload").HasColumnType("jsonb").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            entity.Property(e => e.ProcessedAt).HasColumnName("processed_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            entity.HasIndex(e => e.TenantId).HasDatabaseName("idx_events_tenant");
            entity.HasIndex(e => e.EventType).HasDatabaseName("idx_events_type");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_events_status");
            entity.HasIndex(e => new { e.SourceService, e.SourceEntityId }).HasDatabaseName("idx_events_source");
        });
    }

    private static void ConfigureMessageTemplate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageTemplate>(entity =>
        {
            entity.ToTable("message_templates");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(50);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id").HasMaxLength(50);
            entity.Property(e => e.TemplateKey).HasColumnName("template_key").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Channel).HasColumnName("channel").HasMaxLength(20).IsRequired();
            entity.Property(e => e.TemplateBody).HasColumnName("template_body").IsRequired();
            entity.Property(e => e.Variables).HasColumnName("variables").HasColumnType("jsonb");
            entity.Property(e => e.Active).HasColumnName("active").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => new { e.TenantId, e.TemplateKey, e.Channel }).IsUnique().HasDatabaseName("idx_templates_unique");
            entity.HasIndex(e => e.TenantId).HasDatabaseName("idx_templates_tenant");
            entity.HasIndex(e => e.TemplateKey).HasDatabaseName("idx_templates_key");
        });
    }

    private static void ConfigureMessageLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageLog>(entity =>
        {
            entity.ToTable("message_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(50);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id").HasMaxLength(50).IsRequired();
            entity.Property(e => e.EventId).HasColumnName("event_id").HasMaxLength(50);
            entity.Property(e => e.Channel).HasColumnName("channel").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Recipient).HasColumnName("recipient").HasMaxLength(50).IsRequired();
            entity.Property(e => e.TemplateKey).HasColumnName("template_key").HasMaxLength(100).IsRequired();
            entity.Property(e => e.MessageBody).HasColumnName("message_body").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            entity.Property(e => e.RetryCount).HasColumnName("retry_count").IsRequired();
            entity.Property(e => e.SentAt).HasColumnName("sent_at");
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            entity.HasIndex(e => e.TenantId).HasDatabaseName("idx_logs_tenant");
            entity.HasIndex(e => e.EventId).HasDatabaseName("idx_logs_event");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_logs_status");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_logs_created");

            entity.HasOne<CommunicationEvent>()
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureScheduledMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduledMessage>(entity =>
        {
            entity.ToTable("scheduled_messages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(50);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id").HasMaxLength(50).IsRequired();
            entity.Property(e => e.EventId).HasColumnName("event_id").HasMaxLength(50);
            entity.Property(e => e.SourceEntityId).HasColumnName("source_entity_id").HasMaxLength(50).IsRequired();
            entity.Property(e => e.MessageType).HasColumnName("message_type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.ScheduledAt).HasColumnName("scheduled_at").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            entity.Property(e => e.SentAt).HasColumnName("sent_at");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            entity.HasIndex(e => e.TenantId).HasDatabaseName("idx_scheduled_tenant");
            entity.HasIndex(e => new { e.ScheduledAt, e.Status }).HasDatabaseName("idx_scheduled_status");
            entity.HasIndex(e => e.SourceEntityId).HasDatabaseName("idx_scheduled_source");

            entity.HasOne<CommunicationEvent>()
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureBusinessCommunicationSettings(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessCommunicationSettings>(entity =>
        {
            entity.ToTable("business_communication_settings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(50);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Settings).HasColumnName("settings").HasColumnType("jsonb").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.TenantId).IsUnique().HasDatabaseName("idx_settings_tenant");
        });
    }

    private static void SeedDefaultTemplates(ModelBuilder modelBuilder)
    {
        var templates = new[]
        {
            new MessageTemplate
            {
                Id = "tpl-default-apt-created",
                TenantId = null,
                TemplateKey = "appointment_created",
                Channel = "whatsapp",
                TemplateBody = "Olá {{client_name}}! Seu agendamento de {{service_name}} com {{professional_name}} foi confirmado para {{scheduled_date}} das {{scheduled_time}} às {{end_time}}. Até lá!",
                Variables = "[\"client_name\", \"service_name\", \"professional_name\", \"scheduled_date\", \"scheduled_time\", \"end_time\"]",
                Active = true
            },
            new MessageTemplate
            {
                Id = "tpl-default-apt-rescheduled",
                TenantId = null,
                TemplateKey = "appointment_rescheduled",
                Channel = "whatsapp",
                TemplateBody = "Olá {{client_name}}! Seu agendamento com {{professional_name}} foi alterado de {{old_date}} para {{new_date}} das {{new_time}} às {{new_end_time}}.",
                Variables = "[\"client_name\", \"professional_name\", \"old_date\", \"new_date\", \"new_time\", \"new_end_time\"]",
                Active = true
            },
            new MessageTemplate
            {
                Id = "tpl-default-apt-cancelled",
                TenantId = null,
                TemplateKey = "appointment_cancelled",
                Channel = "whatsapp",
                TemplateBody = "Olá {{client_name}}! Seu agendamento de {{service_name}} com {{professional_name}} do dia {{scheduled_date}} das {{scheduled_time}} às {{end_time}} foi cancelado.",
                Variables = "[\"client_name\", \"service_name\", \"professional_name\", \"scheduled_date\", \"scheduled_time\", \"end_time\"]",
                Active = true
            },
            new MessageTemplate
            {
                Id = "tpl-default-reminder-24h",
                TenantId = null,
                TemplateKey = "appointment_reminder_24h",
                Channel = "whatsapp",
                TemplateBody = "Lembrete: Você tem agendamento amanhã de {{service_name}} com {{professional_name}} das {{scheduled_time}} às {{end_time}}. Nos vemos lá!",
                Variables = "[\"service_name\", \"professional_name\", \"scheduled_time\", \"end_time\"]",
                Active = true
            },
            new MessageTemplate
            {
                Id = "tpl-default-reminder-1h",
                TenantId = null,
                TemplateKey = "appointment_reminder_1h",
                Channel = "whatsapp",
                TemplateBody = "Lembrete: Seu agendamento de {{service_name}} com {{professional_name}} é daqui a 1 hora. Estamos te esperando!",
                Variables = "[\"service_name\", \"professional_name\"]",
                Active = true
            }
        };

        modelBuilder.Entity<MessageTemplate>().HasData(templates);
    }
}
