using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSC.Migrations
{
    public partial class init15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentDiseaseCourses_Patients_PatientId",
                table: "TreatmentDiseaseCourses");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentDiseaseCourses_PatientId",
                table: "TreatmentDiseaseCourses");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "TreatmentDiseaseCourses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "TreatmentDiseaseCourses",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentDiseaseCourses_PatientId",
                table: "TreatmentDiseaseCourses",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentDiseaseCourses_Patients_PatientId",
                table: "TreatmentDiseaseCourses",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
