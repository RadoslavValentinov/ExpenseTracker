using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add UserId temporarily as nullable
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Reminders",
                type: "TEXT",
                nullable: true);

            // 2. Assign all existing reminders
            //    to the only existing user
            migrationBuilder.Sql("""
                UPDATE Reminders
                SET UserId = (
                    SELECT Id
                    FROM AspNetUsers
                    LIMIT 1
                )
                WHERE UserId IS NULL;
                """);

            // 3. Make UserId required
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Reminders",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reminders");
        }
    }
}