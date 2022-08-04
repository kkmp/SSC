using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSC.Migrations
{
    public partial class init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { new Guid("0c051c70-61a2-44bb-bf9c-691bf30a9c11"), "Sprawdza ludzi", "Laborant" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { new Guid("64afcb40-4e45-4b94-9f31-19b57c20027f"), "Leczy ludzi", "Lekarz" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { new Guid("af4fafea-b5fe-4a17-be95-60f35f4cca0a"), "Zarzadza ludzi", "Administrator" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("0c051c70-61a2-44bb-bf9c-691bf30a9c11"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("64afcb40-4e45-4b94-9f31-19b57c20027f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("af4fafea-b5fe-4a17-be95-60f35f4cca0a"));
        }
    }
}
