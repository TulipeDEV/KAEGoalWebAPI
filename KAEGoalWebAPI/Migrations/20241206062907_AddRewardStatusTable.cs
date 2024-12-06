using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KAEGoalWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRewardStatusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RewardStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardStatuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "RewardStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Reward Requested" },
                    { 2, "Reward Awaiting Approval" },
                    { 3, "Prize Being Procued" },
                    { 4, "Prize Verification" },
                    { 5, "Priz Ready for Pickup" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RewardStatuses");
        }
    }
}
