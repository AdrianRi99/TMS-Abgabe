using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Orders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                schema: "orders",
                table: "order_items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Bestehende Positionen erhalten eine feste, fortlaufende Reihenfolge pro Bestellung.
            migrationBuilder.Sql(@"
                UPDATE orders.order_items AS i
                SET ""Position"" = s.rn
                FROM (
                    SELECT ""Id"", ROW_NUMBER() OVER (PARTITION BY ""OrderId"" ORDER BY ""Id"") - 1 AS rn
                    FROM orders.order_items
                ) AS s
                WHERE i.""Id"" = s.""Id"";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                schema: "orders",
                table: "order_items");
        }
    }
}
