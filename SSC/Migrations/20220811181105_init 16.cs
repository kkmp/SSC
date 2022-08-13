using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSC.Migrations
{
    public partial class init16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TreatmentId",
                table: "TreatmentDiseaseCourses",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentDiseaseCourses_TreatmentId",
                table: "TreatmentDiseaseCourses",
                column: "TreatmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentDiseaseCourses_Treatments_TreatmentId",
                table: "TreatmentDiseaseCourses",
                column: "TreatmentId",
                principalTable: "Treatments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentDiseaseCourses_Treatments_TreatmentId",
                table: "TreatmentDiseaseCourses");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentDiseaseCourses_TreatmentId",
                table: "TreatmentDiseaseCourses");

            migrationBuilder.DropColumn(
                name: "TreatmentId",
                table: "TreatmentDiseaseCourses");
        }
    }
}
