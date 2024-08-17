using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Node",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("59ce29b2-c519-4070-89ee-9379652a2920"), "Stock.API" },
                    { new Guid("8ca872a4-bab2-4fb9-9e66-903f909c4411"), "Payment.API" },
                    { new Guid("9dfdb0e5-7782-4772-b152-435879cb5c0b"), "Order.API" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Node",
                keyColumn: "Id",
                keyValue: new Guid("59ce29b2-c519-4070-89ee-9379652a2920"));

            migrationBuilder.DeleteData(
                table: "Node",
                keyColumn: "Id",
                keyValue: new Guid("8ca872a4-bab2-4fb9-9e66-903f909c4411"));

            migrationBuilder.DeleteData(
                table: "Node",
                keyColumn: "Id",
                keyValue: new Guid("9dfdb0e5-7782-4772-b152-435879cb5c0b"));
        }
    }
}
