using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add UserId as nullable temporarily
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Tasks",
                type: "TEXT",
                nullable: true);

            // 2. Assign all existing tasks to the only existing user
            migrationBuilder.Sql("""
                UPDATE Tasks
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
                table: "Tasks",
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
                table: "Tasks");
        }
    }
}