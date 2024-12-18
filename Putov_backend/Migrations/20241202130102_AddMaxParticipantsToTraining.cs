using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Putov_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxParticipantsToTraining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxParticipants",
                table: "Trainings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxParticipants",
                table: "Trainings");
        }
    }
}
