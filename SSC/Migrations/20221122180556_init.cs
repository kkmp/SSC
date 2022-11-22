using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSC.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Citizenships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citizenships", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DiseaseCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseCourses", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TestTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TreatmentStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentStatuses", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProvinceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Surname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<byte[]>(type: "longblob", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "longblob", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Street = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CityId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Places_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChangePasswordCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiredDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangePasswordCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangePasswordCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Pesel = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Surname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sex = table.Column<string>(type: "varchar(1)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BirthDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Street = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AddDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CityId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CitizenshipId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patients_Citizenships_CitizenshipId",
                        column: x => x.CitizenshipId,
                        principalTable: "Citizenships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicalHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PatientId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalHistories_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Treatments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsCovid = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    PatientId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TreatmentStatusId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Treatments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Treatments_TreatmentStatuses_TreatmentStatusId",
                        column: x => x.TreatmentStatusId,
                        principalTable: "TreatmentStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Treatments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TestDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OrderNumber = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResultDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Result = table.Column<string>(type: "varchar(1)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TestTypeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TreatmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlaceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tests_TestTypes_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tests_Treatments_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "Treatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TreatmentDiseaseCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TreatmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DiseaseCourseId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentDiseaseCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentDiseaseCourses_DiseaseCourses_DiseaseCourseId",
                        column: x => x.DiseaseCourseId,
                        principalTable: "DiseaseCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentDiseaseCourses_Treatments_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "Treatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentDiseaseCourses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Citizenships",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e"), "Obywatelstwo polskie" },
                    { new Guid("8ecd38fb-b961-4a97-89b9-79a46818a733"), "Obywatelstwo włoskie" },
                    { new Guid("ac9a7363-a4d1-4e58-be30-09a2c43be9c5"), "Obywatelstwo słowackie" },
                    { new Guid("b5b842de-e3af-4717-816e-3993c19a8d30"), "Obywatelstwo brytyjskie" },
                    { new Guid("e676e030-baa1-4091-b685-9b326782da57"), "Obywatelstwo francuskie" }
                });

            migrationBuilder.InsertData(
                table: "DiseaseCourses",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("455df914-7758-4e95-8e15-a32c8dce71aa"), "Zwiększona temperatura ciała, która może obejmować stan podgorączkowy, gorączkę nieznaczną, umiarkowaną, znaczną i wysoką.", "Gorączka" },
                    { new Guid("5a055498-aa04-4d96-a8a1-8ef5b08f3e5a"), "Problemy kardiologiczne charakteryzujące się przyśpieszeniem, zwolnieniem lub nieregularnością rytmu bicia serca.", "Zaburzenia rytmu serca" },
                    { new Guid("6d3a708a-b686-4126-81c4-3638ae855ddd"), "Stan zapalny płuc powodujący przedłużający się kaszel, uczucie duszności, łatwe męczenie się oraz ból w klatce piersiowej.", "Zapalenie płuc" },
                    { new Guid("87feb19d-84c7-4332-856e-cbeefb08a66d"), "Powikłania neurologiczne przejawiające się problemami z pamięcią i koncentracją.", "Mgła mózgowa" },
                    { new Guid("b942cf77-74c0-47f7-a626-dd8784cbcd3e"), "Bóle mięśni, stawów i kości. Najczęściej dotyczą bólu kończyn górnych oraz dolnych, szyi, ramion i pleców.", "Bóle mięśniowo-stawowe" },
                    { new Guid("ce4d9efd-4065-4c7d-ad64-20ad0e2b65c1"), "Klasterowy bądź napięciowy ból głowy, który może wynikać z bólu twarzy, głowy lub szyi.", "Nawracający ból głowy" }
                });

            migrationBuilder.InsertData(
                table: "Provinces",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0751b70c-ca54-4bfc-ac68-61468112fbcb"), "Województwo warmińsko-mazurskie" },
                    { new Guid("09e64566-4f17-4e1e-af07-fe3651ee5784"), "Województwo podkarpackie" },
                    { new Guid("0ea63e3a-31f4-4b49-a908-1f85152d3e67"), "Województwo łódzkie" },
                    { new Guid("114b7d3e-3c94-4d56-9139-4e232f6fdad4"), "Województwo śląskie" },
                    { new Guid("23a0abb8-e9b6-4f7e-99e3-edef8989f5f4"), "Województwo kujawsko-pomorskie" },
                    { new Guid("263e6778-60e5-4caa-9389-a57a6505cfab"), "Województwo mazowieckie" },
                    { new Guid("39f1ab90-d13a-4e8b-ba4b-38e690463c0b"), "Województwo opolskie" },
                    { new Guid("3e4ae992-210e-482b-a961-3e132fe02d15"), "Województwo dolnośląskie" },
                    { new Guid("424d832e-b345-4d77-be86-61b57018052e"), "Województwo małopolskie" },
                    { new Guid("4afb93c9-f1e7-49b9-ab9c-7152d98c51c9"), "Województwo lubelskie" },
                    { new Guid("61f9e794-c161-4df2-b7fe-01cee90a62cc"), "Województwo zachodniopomorskie" },
                    { new Guid("628490fe-563f-4a2f-9938-0d0ef543575f"), "Województwo pomorskie" },
                    { new Guid("79ff058e-cdf0-4dc0-a47f-397988305bf1"), "Województwo wielkopolskie" },
                    { new Guid("8e56be84-1e53-48cc-8cfd-efe9de55091e"), "Województwo lubuskie" },
                    { new Guid("b543d147-8550-46b0-8035-2027e54028a4"), "Województwo podlaskie" },
                    { new Guid("b7fbea40-841f-41c5-b8b4-802639e09211"), "Województwo świętokrzyskie" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("0c051c70-61a2-44bb-bf9c-691bf30a9c11"), "Pomocniczy pracownik laboratorium odpowiedzialny za wykonywanie analiz laboratoryjnych.", "Laborant" },
                    { new Guid("64afcb40-4e45-4b94-9f31-19b57c20027f"), "Pracownik służby zdrowia, który jest odpowiedzialny za badanie, diagnostykę i leczenie chorób u pacjentów.", "Lekarz" },
                    { new Guid("af4fafea-b5fe-4a17-be95-60f35f4cca0a"), "Pracownik zarządzający systemem i kontami użytkowników.", "Administrator" }
                });

            migrationBuilder.InsertData(
                table: "TestTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("7a273b91-7d73-4060-ab98-77d66ef1a187"), "Test RT-PCR/RT-LAMP" },
                    { new Guid("867a5e9c-32c2-49ea-b540-9f300f94416e"), "Test na przeciwciała" },
                    { new Guid("e3b9c042-6910-4353-8c47-8dacf77b149b"), "Test antygenowy" }
                });

            migrationBuilder.InsertData(
                table: "TreatmentStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("3b55e949-1c36-4182-b0c4-38eaf9e70251"), "Ozdrowienie" },
                    { new Guid("6eac697b-8839-4d87-bf3c-51ba2308e75f"), "Rozpoczęto" },
                    { new Guid("85ddc1f6-b4d5-4936-9e1f-7d8d1e7e6bc9"), "Zgon pacjenta" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Name", "ProvinceId" },
                values: new object[,]
                {
                    { new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"), "Warszawa", new Guid("263e6778-60e5-4caa-9389-a57a6505cfab") },
                    { new Guid("0966f27d-a998-4eff-bf88-9e8f47ad497c"), "Bydgoszcz", new Guid("23a0abb8-e9b6-4f7e-99e3-edef8989f5f4") },
                    { new Guid("0abfc740-3794-4bdd-87c8-9cc4e3e53e85"), "Szczecin", new Guid("61f9e794-c161-4df2-b7fe-01cee90a62cc") },
                    { new Guid("21a02399-e314-4c87-9fbe-435ca0975187"), "Poznań", new Guid("79ff058e-cdf0-4dc0-a47f-397988305bf1") },
                    { new Guid("2317669a-df86-47a8-bdca-36dd6ffac43a"), "Łódź", new Guid("0ea63e3a-31f4-4b49-a908-1f85152d3e67") },
                    { new Guid("2b22474e-8adb-4e82-915a-d253f6ac5d78"), "Gdańsk", new Guid("628490fe-563f-4a2f-9938-0d0ef543575f") },
                    { new Guid("2d3aa1f8-2c04-430d-b1e4-a9cf8a5bb844"), "Kielce", new Guid("b7fbea40-841f-41c5-b8b4-802639e09211") },
                    { new Guid("44c2b1ef-622b-4c60-bd54-f104cbfc4ace"), "Zielona Góra", new Guid("8e56be84-1e53-48cc-8cfd-efe9de55091e") },
                    { new Guid("67d6d5ba-d6f4-4bc4-bbec-3c786dcdd763"), "Olsztyn", new Guid("0751b70c-ca54-4bfc-ac68-61468112fbcb") },
                    { new Guid("6eae3a80-7f29-4a8f-b4ae-7d0ffe2f27d1"), "Rzeszów", new Guid("09e64566-4f17-4e1e-af07-fe3651ee5784") },
                    { new Guid("797550f3-975d-4a22-912b-9febd426c5f9"), "Białystok", new Guid("b543d147-8550-46b0-8035-2027e54028a4") },
                    { new Guid("9c635480-4bac-415a-a9ea-3a5413edd398"), "Katowice", new Guid("114b7d3e-3c94-4d56-9139-4e232f6fdad4") },
                    { new Guid("a0e9046a-ceff-4dcf-a9b9-aa963850ecae"), "Kraków", new Guid("424d832e-b345-4d77-be86-61b57018052e") },
                    { new Guid("a238f198-3bed-4cdd-8c03-c0ed8371fff6"), "Lublin", new Guid("4afb93c9-f1e7-49b9-ab9c-7152d98c51c9") },
                    { new Guid("ca1f1f15-f996-4f54-ad9f-1bdb91ebc98d"), "Wrocław", new Guid("3e4ae992-210e-482b-a961-3e132fe02d15") },
                    { new Guid("e5a73afd-cf08-4a97-8b8b-44fad559ac47"), "Opole", new Guid("39f1ab90-d13a-4e8b-ba4b-38e690463c0b") }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Date", "Email", "IsActive", "Name", "PasswordHash", "PasswordSalt", "PhoneNumber", "RoleId", "Surname" },
                values: new object[,]
                {
                    { new Guid("3e7e4cc3-d3e1-4917-9647-42422b4c429a"), new DateTime(2022, 11, 22, 19, 5, 56, 19, DateTimeKind.Local).AddTicks(9267), "j.kowalski@gmail.com", true, "Jan", new byte[] { 237, 252, 127, 72, 112, 89, 59, 150, 57, 135, 93, 228, 212, 49, 35, 166, 100, 242, 55, 195, 76, 229, 201, 50, 131, 215, 13, 175, 17, 237, 45, 15, 100, 115, 150, 72, 253, 25, 245, 18, 241, 185, 111, 56, 248, 85, 171, 120, 99, 95, 46, 30, 109, 169, 26, 185, 169, 63, 188, 212, 236, 182, 192, 128 }, new byte[] { 250, 77, 138, 143, 145, 45, 32, 18, 236, 180, 43, 158, 19, 92, 50, 189, 165, 205, 81, 4, 110, 224, 253, 147, 37, 103, 213, 201, 72, 15, 241, 37, 19, 144, 88, 176, 110, 90, 237, 4, 234, 80, 33, 96, 74, 13, 72, 189, 1, 12, 238, 205, 85, 76, 55, 196, 175, 224, 248, 122, 38, 90, 230, 172, 121, 70, 245, 116, 95, 200, 247, 116, 117, 186, 193, 14, 100, 121, 88, 247, 248, 0, 193, 25, 248, 124, 32, 36, 149, 133, 126, 235, 172, 77, 181, 230, 216, 117, 123, 140, 93, 201, 88, 171, 74, 251, 142, 38, 51, 96, 108, 143, 126, 34, 30, 246, 129, 234, 118, 196, 175, 54, 165, 101, 173, 206, 29, 17 }, "123456789", new Guid("af4fafea-b5fe-4a17-be95-60f35f4cca0a"), "Kowalski" },
                    { new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"), new DateTime(2022, 11, 22, 19, 5, 56, 19, DateTimeKind.Local).AddTicks(9278), "s.kowalczyk@gmail.com", true, "Szymon", new byte[] { 34, 34, 211, 39, 41, 14, 244, 170, 81, 161, 225, 20, 170, 72, 117, 57, 67, 58, 185, 47, 90, 194, 73, 203, 105, 99, 184, 103, 50, 115, 213, 222, 136, 218, 225, 90, 139, 181, 5, 119, 165, 2, 100, 107, 173, 61, 167, 45, 188, 171, 99, 130, 23, 240, 242, 35, 208, 36, 0, 92, 74, 32, 35, 14 }, new byte[] { 27, 217, 239, 148, 179, 106, 214, 126, 195, 56, 94, 217, 251, 163, 119, 148, 178, 105, 245, 189, 64, 110, 159, 238, 237, 221, 163, 167, 11, 197, 79, 179, 0, 246, 120, 74, 3, 172, 41, 29, 121, 224, 94, 224, 219, 173, 8, 96, 4, 246, 236, 28, 54, 183, 238, 197, 107, 78, 198, 58, 192, 111, 150, 130, 241, 42, 166, 230, 57, 221, 148, 48, 220, 11, 201, 139, 186, 22, 125, 38, 72, 26, 247, 147, 125, 176, 255, 20, 149, 176, 192, 238, 17, 177, 140, 113, 188, 138, 236, 230, 159, 74, 56, 200, 90, 23, 209, 39, 156, 156, 71, 78, 162, 186, 220, 196, 250, 181, 57, 215, 35, 214, 239, 17, 161, 109, 111, 53 }, "456123789", new Guid("0c051c70-61a2-44bb-bf9c-691bf30a9c11"), "Kowalczyk" },
                    { new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"), new DateTime(2022, 11, 22, 19, 5, 56, 19, DateTimeKind.Local).AddTicks(9273), "a.nowak@gmail.com", true, "Adam", new byte[] { 161, 140, 144, 194, 255, 4, 100, 194, 252, 47, 200, 228, 140, 115, 150, 94, 128, 135, 29, 141, 88, 72, 224, 236, 12, 149, 195, 186, 246, 248, 203, 250, 62, 130, 215, 88, 226, 145, 27, 13, 249, 152, 104, 197, 53, 143, 129, 212, 156, 49, 122, 226, 184, 137, 157, 135, 123, 253, 220, 222, 46, 179, 139, 47 }, new byte[] { 203, 38, 31, 137, 153, 97, 171, 231, 255, 74, 139, 210, 154, 23, 241, 229, 75, 71, 108, 200, 160, 192, 235, 244, 98, 203, 214, 204, 218, 152, 90, 67, 27, 228, 13, 76, 56, 52, 40, 136, 156, 17, 227, 85, 182, 41, 100, 99, 6, 107, 118, 215, 138, 95, 154, 28, 39, 249, 45, 45, 237, 97, 48, 112, 177, 217, 197, 162, 226, 249, 112, 134, 16, 145, 63, 104, 38, 143, 133, 18, 233, 46, 64, 161, 242, 243, 186, 167, 156, 60, 211, 254, 200, 128, 38, 14, 91, 161, 133, 139, 182, 236, 212, 175, 172, 119, 202, 84, 54, 191, 173, 173, 89, 204, 20, 205, 41, 26, 196, 44, 208, 23, 138, 40, 148, 199, 35, 120 }, "987654321", new Guid("64afcb40-4e45-4b94-9f31-19b57c20027f"), "Nowak" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChangePasswordCodes_UserId",
                table: "ChangePasswordCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceId",
                table: "Cities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizenships_Name",
                table: "Citizenships",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCourses_Name",
                table: "DiseaseCourses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistories_PatientId",
                table: "MedicalHistories",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistories_UserId",
                table: "MedicalHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_CitizenshipId",
                table: "Patients",
                column: "CitizenshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_CityId",
                table: "Patients",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Pesel",
                table: "Patients",
                column: "Pesel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Places_CityId",
                table: "Places",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Places_Name",
                table: "Places",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_Name",
                table: "Provinces",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_OrderNumber",
                table: "Tests",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_PlaceId",
                table: "Tests",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_TestTypeId",
                table: "Tests",
                column: "TestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_TreatmentId",
                table: "Tests",
                column: "TreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_UserId",
                table: "Tests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_Name",
                table: "TestTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentDiseaseCourses_DiseaseCourseId",
                table: "TreatmentDiseaseCourses",
                column: "DiseaseCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentDiseaseCourses_TreatmentId",
                table: "TreatmentDiseaseCourses",
                column: "TreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentDiseaseCourses_UserId",
                table: "TreatmentDiseaseCourses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_PatientId",
                table: "Treatments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_TreatmentStatusId",
                table: "Treatments",
                column: "TreatmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_UserId",
                table: "Treatments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentStatuses_Name",
                table: "TreatmentStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangePasswordCodes");

            migrationBuilder.DropTable(
                name: "MedicalHistories");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "TreatmentDiseaseCourses");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropTable(
                name: "TestTypes");

            migrationBuilder.DropTable(
                name: "DiseaseCourses");

            migrationBuilder.DropTable(
                name: "Treatments");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "TreatmentStatuses");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Citizenships");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
