using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationNoticeInHoursToTrainer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CancellationNoticeInHours",
                table: "Trainers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationNoticeInHours",
                table: "Trainers");
        }
    }
}
