using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Addresses.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "addresses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_addresses_Name",
                table: "addresses",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_addresses_Name",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "addresses");
        }
    }
}
