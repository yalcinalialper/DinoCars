using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DinoCars.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniquePlatePerCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CustomerId",
                table: "Vehicles");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CustomerId_LicensePlate",
                table: "Vehicles",
                columns: new[] { "CustomerId", "LicensePlate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CustomerId_LicensePlate",
                table: "Vehicles");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CustomerId",
                table: "Vehicles",
                column: "CustomerId");
        }
    }
}
