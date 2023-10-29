using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3f54eaf3-53c3-461c-9996-36afdbd66c48", "938c94c5-2d46-4ee3-909c-01c026c92b51", "Editor", "EDITOR" },
                    { "53511772-a43c-49ca-bd74-4a16fe0f2a59", "f07d06ce-5ffc-4cf5-b7ef-44c9c99fb5f8", "Admin", "ADMIN" },
                    { "70daf97c-c206-4e99-b4d3-6932ec644e05", "b60cadd6-03db-42fa-9786-fdf0d73c3ab0", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3f54eaf3-53c3-461c-9996-36afdbd66c48");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "53511772-a43c-49ca-bd74-4a16fe0f2a59");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "70daf97c-c206-4e99-b4d3-6932ec644e05");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

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
    }
}
