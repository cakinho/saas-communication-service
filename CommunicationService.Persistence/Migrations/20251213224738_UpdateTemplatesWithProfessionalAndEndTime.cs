using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunicationService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTemplatesWithProfessionalAndEndTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "message_templates",
                keyColumn: "id",
                keyValue: "tpl-default-apt-cancelled",
                columns: new[] { "created_at", "template_body", "updated_at", "variables" },
                values: new object[] { new DateTime(2025, 12, 13, 22, 47, 37, 763, DateTimeKind.Utc).AddTicks(755), "Olá {{client_name}}! Seu agendamento de {{service_name}} com {{professional_name}} do dia {{scheduled_date}} das {{scheduled_time}} às {{end_time}} foi cancelado.", new DateTime(2025, 12, 13, 22, 47, 37, 763, DateTimeKind.Utc).AddTicks(755), "[\"client_name\", \"service_name\", \"professional_name\", \"scheduled_date\", \"scheduled_time\", \"end_time\"]" });

            migrationBuilder.UpdateData(
                table: "message_templates",
                keyColumn: "id",
                keyValue: "tpl-default-apt-created",
                columns: new[] { "created_at", "template_body", "updated_at", "variables" },
                values: new object[] { new DateTime(2025, 12, 13, 22, 47, 37, 763, DateTimeKind.Utc).AddTicks(718), "Olá {{client_name}}! Seu agendamento de {{service_name}} com {{professional_name}} foi confirmado para {{scheduled_date}} das {{scheduled_time}} às {{end_time}}. Até lá!", new DateTime(2025, 12, 13, 22, 47, 37, 763, DateTimeKind.Utc).AddTicks(723), "[\"client_name\", \"service_name\", \"professional_name\", \"scheduled_date\", \"scheduled_time\", \"end_time\"]" });

            migrationBuilder.UpdateData(
                table: "message_templates",
                keyColumn: "id",
                keyValue: "tpl-default-apt-rescheduled",
                columns: new[] { "created_at", "template_body", "updated_at", "variables" },
                values: new object[] { new DateTime(2025, 12, 13, 22, 47, 37, 763, DateTimeKind.Utc).AddTicks(750), "Olá {{client_name}}! Seu agendamento com {{professional_name}} foi alterado de {{old_date}} para {{new_date}} das {{new_time}} às {{new_end_time}}.", new DateTime(2025, 12, 13, 22, 47, 37, 763, DateTimeKind.Utc).AddTicks(750), "[\"client_name\", \"professional_name\", \"old_date\", \"new_date\", \"new_time\", \"new_end_time\"]" });

            migrationBuilder.UpdateData(
                table: "message_templates",
                keyColumn: "id",
                keyValue: "tpl-default-reminder-1h",
                columns: new[] { "created_at", "template_body", "updated_at", "variables" },
                values: new object[] { new DateTime(2025, 12, 13, 22, 47, 37, 763, DateTimeKind.Utc).AddTicks(765), "Lembrete: Seu agendamento de {{service_name}} com {{professional_name}} é daqui a 1 hora. Estamos te esperando!", new DateTime(2025, 12, 13, 22, 47, 37, 763, DateTimeKind.Utc).AddTicks(765), "[\"service_name\", \"professional_name\"]" });

            migrationBuilder.UpdateData(
                table: "message_templates",
                keyColumn: "id",
                keyValue: "tpl-default-reminder-24h",
                columns: new[] { "created_at", "template_body", "updated_at", "variables" },
                values: new object[] { new DateTime(2025, 12, 13, 22, 47, 37, 763, DateTimeKind.Utc).AddTicks(760), "Lembrete: Você tem agendamento amanhã de {{service_name}} com {{professional_name}} das {{scheduled_time}} às {{end_time}}. Nos vemos lá!", new DateTime(2025, 12, 13, 22, 47, 37, 763, DateTimeKind.Utc).AddTicks(760), "[\"service_name\", \"professional_name\", \"scheduled_time\", \"end_time\"]" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "message_templates",
                keyColumn: "id",
                keyValue: "tpl-default-apt-cancelled",
                columns: new[] { "created_at", "template_body", "updated_at", "variables" },
                values: new object[] { new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2068), "Olá {{client_name}}! Seu agendamento de {{service_name}} do dia {{scheduled_date}} foi cancelado.", new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2068), "[\"client_name\", \"service_name\", \"scheduled_date\"]" });

            migrationBuilder.UpdateData(
                table: "message_templates",
                keyColumn: "id",
                keyValue: "tpl-default-apt-created",
                columns: new[] { "created_at", "template_body", "updated_at", "variables" },
                values: new object[] { new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2054), "Olá {{client_name}}! Seu agendamento de {{service_name}} foi confirmado para {{scheduled_date}} às {{scheduled_time}}. Até lá!", new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2057), "[\"client_name\", \"service_name\", \"scheduled_date\", \"scheduled_time\"]" });

            migrationBuilder.UpdateData(
                table: "message_templates",
                keyColumn: "id",
                keyValue: "tpl-default-apt-rescheduled",
                columns: new[] { "created_at", "template_body", "updated_at", "variables" },
                values: new object[] { new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2063), "Olá {{client_name}}! Seu agendamento foi alterado de {{old_date}} para {{new_date}} às {{new_time}}.", new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2063), "[\"client_name\", \"old_date\", \"new_date\", \"new_time\"]" });

            migrationBuilder.UpdateData(
                table: "message_templates",
                keyColumn: "id",
                keyValue: "tpl-default-reminder-1h",
                columns: new[] { "created_at", "template_body", "updated_at", "variables" },
                values: new object[] { new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2100), "Lembrete: Seu agendamento é daqui a 1 hora - {{service_name}}. Estamos te esperando!", new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2100), "[\"service_name\"]" });

            migrationBuilder.UpdateData(
                table: "message_templates",
                keyColumn: "id",
                keyValue: "tpl-default-reminder-24h",
                columns: new[] { "created_at", "template_body", "updated_at", "variables" },
                values: new object[] { new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2095), "Lembrete: Você tem agendamento amanhã às {{scheduled_time}} - {{service_name}}. Nos vemos lá!", new DateTime(2025, 12, 13, 17, 59, 22, 309, DateTimeKind.Utc).AddTicks(2095), "[\"scheduled_time\", \"service_name\"]" });
        }
    }
}
