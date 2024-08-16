using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuitkuMvpNetApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeReferenceColumnTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "MsTransactionReference",
                table: "Transactions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MsTransactionReference",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "Transactions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
