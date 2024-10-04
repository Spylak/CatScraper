using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatScraper.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalCounterEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cats_CatId",
                table: "Cats");

            migrationBuilder.AlterColumn<string>(
                name: "CatId",
                table: "Cats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "GlobalCounters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalCounters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "GlobalCounters",
                columns: new[] { "Id", "Type", "Value" },
                values: new object[] { 1, 0, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Cats_CatId",
                table: "Cats",
                column: "CatId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalCounters");

            migrationBuilder.DropIndex(
                name: "IX_Cats_CatId",
                table: "Cats");

            migrationBuilder.AlterColumn<string>(
                name: "CatId",
                table: "Cats",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Cats_CatId",
                table: "Cats",
                column: "CatId",
                unique: true,
                filter: "[CatId] IS NOT NULL");
        }
    }
}
