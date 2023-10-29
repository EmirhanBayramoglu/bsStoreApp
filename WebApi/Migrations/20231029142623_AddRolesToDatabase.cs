using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1f1f6cdb-89ad-4d0d-b646-43ecc9c2887f", "1751a1e5-fcab-4f22-bd91-c4cd99091399", "Admin", "ADMIN" },
                    { "90a7b89e-7c7c-423d-8152-bb5ba3ef6bdd", "de3cdf35-e881-4b85-a748-ea9945fe5037", "Editor", "EDıTOR" },
                    { "f7b28f10-a458-4a33-bbd1-72b84175eb60", "871e47f4-7e67-4767-a049-34069e5cd34d", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f1f6cdb-89ad-4d0d-b646-43ecc9c2887f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90a7b89e-7c7c-423d-8152-bb5ba3ef6bdd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f7b28f10-a458-4a33-bbd1-72b84175eb60");
        }
    }
}
