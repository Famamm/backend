using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fleetops_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceReminderStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "reminder_stage",
                table: "maintenances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "maintenances",
                keyColumn: "id",
                keyValue: 1,
                column: "reminder_stage",
                value: 0);

            // Baris yang sudah pernah menerima reminder dianggap sudah melewati tingkat pertama,
            // supaya tidak mengirim ulang H-7 setelah deploy.
            migrationBuilder.Sql(
                "UPDATE maintenances SET reminder_stage = 1 WHERE reminder_sent_at IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reminder_stage",
                table: "maintenances");
        }
    }
}
