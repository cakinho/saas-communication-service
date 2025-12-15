using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CommunicationService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "business_communication_settings",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tenant_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    settings = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_communication_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "communication_events",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tenant_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    event_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    source_service = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    source_entity_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_communication_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "message_templates",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tenant_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    template_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    template_body = table.Column<string>(type: "text", nullable: false),
                    variables = table.Column<string>(type: "jsonb", nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "message_logs",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tenant_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    event_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    recipient = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    template_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    message_body = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    retry_count = table.Column<int>(type: "integer", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_message_logs_communication_events_event_id",
                        column: x => x.event_id,
                        principalTable: "communication_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "scheduled_messages",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tenant_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    event_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    source_entity_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    message_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduled_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_scheduled_messages_communication_events_event_id",
                        column: x => x.event_id,
                        principalTable: "communication_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "message_templates",
                columns: new[] { "id", "active", "channel", "created_at", "template_body", "template_key", "tenant_id", "updated_at", "variables" },
                values: new object[,]
                {
                    { "tpl-default-apt-cancelled", true, "whatsapp", new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2068), "Olá {{client_name}}! Seu agendamento de {{service_name}} do dia {{scheduled_date}} foi cancelado.", "appointment_cancelled", null, new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2068), "[\"client_name\", \"service_name\", \"scheduled_date\"]" },
                    { "tpl-default-apt-created", true, "whatsapp", new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2054), "Olá {{client_name}}! Seu agendamento de {{service_name}} foi confirmado para {{scheduled_date}} às {{scheduled_time}}. Até lá!", "appointment_created", null, new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2057), "[\"client_name\", \"service_name\", \"scheduled_date\", \"scheduled_time\"]" },
                    { "tpl-default-apt-rescheduled", true, "whatsapp", new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2063), "Olá {{client_name}}! Seu agendamento foi alterado de {{old_date}} para {{new_date}} às {{new_time}}.", "appointment_rescheduled", null, new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2063), "[\"client_name\", \"old_date\", \"new_date\", \"new_time\"]" },
                    { "tpl-default-reminder-1h", true, "whatsapp", new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2100), "Lembrete: Seu agendamento é daqui a 1 hora - {{service_name}}. Estamos te esperando!", "appointment_reminder_1h", null, new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2100), "[\"service_name\"]" },
                    { "tpl-default-reminder-24h", true, "whatsapp", new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2095), "Lembrete: Você tem agendamento amanhã às {{scheduled_time}} - {{service_name}}. Nos vemos lá!", "appointment_reminder_24h", null, new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2095), "[\"scheduled_time\", \"service_name\"]" }
                });

            migrationBuilder.CreateIndex(
                name: "idx_settings_tenant",
                table: "business_communication_settings",
                column: "tenant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_events_source",
                table: "communication_events",
                columns: new[] { "source_service", "source_entity_id" });

            migrationBuilder.CreateIndex(
                name: "idx_events_status",
                table: "communication_events",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_events_tenant",
                table: "communication_events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_events_type",
                table: "communication_events",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "idx_logs_created",
                table: "message_logs",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_logs_event",
                table: "message_logs",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "idx_logs_status",
                table: "message_logs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_logs_tenant",
                table: "message_logs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_templates_key",
                table: "message_templates",
                column: "template_key");

            migrationBuilder.CreateIndex(
                name: "idx_templates_tenant",
                table: "message_templates",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "idx_templates_unique",
                table: "message_templates",
                columns: new[] { "tenant_id", "template_key", "channel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_scheduled_source",
                table: "scheduled_messages",
                column: "source_entity_id");

            migrationBuilder.CreateIndex(
                name: "idx_scheduled_status",
                table: "scheduled_messages",
                columns: new[] { "scheduled_at", "status" });

            migrationBuilder.CreateIndex(
                name: "idx_scheduled_tenant",
                table: "scheduled_messages",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_scheduled_messages_event_id",
                table: "scheduled_messages",
                column: "event_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_communication_settings");

            migrationBuilder.DropTable(
                name: "message_logs");

            migrationBuilder.DropTable(
                name: "message_templates");

            migrationBuilder.DropTable(
                name: "scheduled_messages");

            migrationBuilder.DropTable(
                name: "communication_events");
        }
    }
}
