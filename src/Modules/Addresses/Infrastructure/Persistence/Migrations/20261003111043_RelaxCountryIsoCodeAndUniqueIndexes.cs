using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Addresses.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RelaxCountryIsoCodeAndUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cities_ZipCode_CountryId",
                table: "cities");

            migrationBuilder.AlterColumn<string>(
                name: "IsoCode",
                table: "countries",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.CreateIndex(
                name: "IX_countries_Name",
                table: "countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cities_Name_ZipCode_CountryId",
                table: "cities",
                columns: new[] { "Name", "ZipCode", "CountryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_countries_Name",
                table: "countries");

            migrationBuilder.DropIndex(
                name: "IX_cities_Name_ZipCode_CountryId",
                table: "cities");

            migrationBuilder.AlterColumn<string>(
                name: "IsoCode",
                table: "countries",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_cities_ZipCode_CountryId",
                table: "cities",
                columns: new[] { "ZipCode", "CountryId" });
        }
    }
}
