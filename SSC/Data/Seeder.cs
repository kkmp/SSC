using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using System.Security.Cryptography;

namespace SSC.Data
{
    public static class Seeder
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(new[]
            {
                new Role {Id = new Guid("64afcb40-4e45-4b94-9f31-19b57c20027f"), Name = Roles.Doctor, Description = "Pracownik służby zdrowia, który jest odpowiedzialny za badanie, diagnostykę i leczenie chorób u pacjentów."},
                new Role {Id = new Guid("0c051c70-61a2-44bb-bf9c-691bf30a9c11"), Name = Roles.LabAssistant, Description = "Pomocniczy pracownik laboratorium odpowiedzialny za wykonywanie analiz laboratoryjnych."},
                new Role {Id = new Guid("af4fafea-b5fe-4a17-be95-60f35f4cca0a"), Name = Roles.Admin, Description = "Pracownik zarządzający systemem i kontami użytkowników."}
            });
        }

        public static void SeedTreatmentStatuses(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TreatmentStatus>().HasData(new[]
            {
                new TreatmentStatus {Id = new Guid("6eac697b-8839-4d87-bf3c-51ba2308e75f"), Name = TreatmentStatusOptions.Started},
                new TreatmentStatus {Id = new Guid("85ddc1f6-b4d5-4936-9e1f-7d8d1e7e6bc9"), Name = TreatmentStatusOptions.Death},
                new TreatmentStatus {Id = new Guid("3b55e949-1c36-4182-b0c4-38eaf9e70251"), Name= TreatmentStatusOptions.Recovery}
            });
        }

        public static void SeedDiseaseCourses(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiseaseCourse>().HasData(new[]
            {
                new DiseaseCourse {Id = new Guid("ce4d9efd-4065-4c7d-ad64-20ad0e2b65c1"), Name = "Nawracający ból głowy", Description = "Klasterowy bądź napięciowy ból głowy, który może wynikać z bólu twarzy, głowy lub szyi."},
                new DiseaseCourse {Id = new Guid("87feb19d-84c7-4332-856e-cbeefb08a66d"), Name = "Mgła mózgowa", Description = "Powikłania neurologiczne przejawiające się problemami z pamięcią i koncentracją."},
                new DiseaseCourse {Id = new Guid("455df914-7758-4e95-8e15-a32c8dce71aa"), Name = "Gorączka", Description = "Zwiększona temperatura ciała, która może obejmować stan podgorączkowy, gorączkę nieznaczną, umiarkowaną, znaczną i wysoką."},
                new DiseaseCourse {Id = new Guid("5a055498-aa04-4d96-a8a1-8ef5b08f3e5a"), Name = "Zaburzenia rytmu serca", Description = "Problemy kardiologiczne charakteryzujące się przyśpieszeniem, zwolnieniem lub nieregularnością rytmu bicia serca."},
                new DiseaseCourse {Id = new Guid("b942cf77-74c0-47f7-a626-dd8784cbcd3e"), Name = "Bóle mięśniowo-stawowe", Description = "Bóle mięśni, stawów i kości. Najczęściej dotyczą bólu kończyn górnych oraz dolnych, szyi, ramion i pleców."},
                new DiseaseCourse {Id = new Guid("6d3a708a-b686-4126-81c4-3638ae855ddd"), Name = "Zapalenie płuc", Description = "Stan zapalny płuc powodujący przedłużający się kaszel, uczucie duszności, łatwe męczenie się oraz ból w klatce piersiowej."},
            });
        }

        public static void SeedCitizenships(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Citizenship>().HasData(new[]
            {
                new Citizenship {Id = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e"), Name = "Obywatelstwo polskie"},
                new Citizenship {Id = new Guid("e676e030-baa1-4091-b685-9b326782da57"), Name = "Obywatelstwo francuskie"},
                new Citizenship {Id = new Guid("b5b842de-e3af-4717-816e-3993c19a8d30"), Name = "Obywatelstwo brytyjskie"},
                new Citizenship {Id = new Guid("ac9a7363-a4d1-4e58-be30-09a2c43be9c5"), Name = "Obywatelstwo słowackie"},
                new Citizenship {Id = new Guid("8ecd38fb-b961-4a97-89b9-79a46818a733"), Name = "Obywatelstwo włoskie"},
            });
        }

        public static void SeedProvinces(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Province>().HasData(new[]
            {
                new Province {Id = new Guid("3e4ae992-210e-482b-a961-3e132fe02d15"), Name = "Województwo dolnośląskie"},
                new Province {Id = new Guid("23a0abb8-e9b6-4f7e-99e3-edef8989f5f4"), Name = "Województwo kujawsko-pomorskie"},
                new Province {Id = new Guid("4afb93c9-f1e7-49b9-ab9c-7152d98c51c9"), Name = "Województwo lubelskie"},
                new Province {Id = new Guid("8e56be84-1e53-48cc-8cfd-efe9de55091e"), Name = "Województwo lubuskie"},
                new Province {Id = new Guid("0ea63e3a-31f4-4b49-a908-1f85152d3e67"), Name = "Województwo łódzkie"},
                new Province {Id = new Guid("424d832e-b345-4d77-be86-61b57018052e"), Name = "Województwo małopolskie"},
                new Province {Id = new Guid("263e6778-60e5-4caa-9389-a57a6505cfab"), Name = "Województwo mazowieckie"},
                new Province {Id = new Guid("39f1ab90-d13a-4e8b-ba4b-38e690463c0b"), Name = "Województwo opolskie"},
                new Province {Id = new Guid("09e64566-4f17-4e1e-af07-fe3651ee5784"), Name = "Województwo podkarpackie"},
                new Province {Id = new Guid("b543d147-8550-46b0-8035-2027e54028a4"), Name = "Województwo podlaskie"},
                new Province {Id = new Guid("628490fe-563f-4a2f-9938-0d0ef543575f"), Name = "Województwo pomorskie"},
                new Province {Id = new Guid("114b7d3e-3c94-4d56-9139-4e232f6fdad4"), Name = "Województwo śląskie"},
                new Province {Id = new Guid("b7fbea40-841f-41c5-b8b4-802639e09211"), Name = "Województwo świętokrzyskie"},
                new Province {Id = new Guid("0751b70c-ca54-4bfc-ac68-61468112fbcb"), Name = "Województwo świętokrzyskie"},
                new Province {Id = new Guid("79ff058e-cdf0-4dc0-a47f-397988305bf1"), Name = "Województwo wielkopolskie"},
                new Province {Id = new Guid("61f9e794-c161-4df2-b7fe-01cee90a62cc"), Name = "Województwo zachodniopomorskie"}
            });
        }

        public static void SeedCities(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(new[]
            {
                new City {Id = new Guid("ca1f1f15-f996-4f54-ad9f-1bdb91ebc98d"), Name = "Wrocław", ProvinceId = new Guid("3e4ae992-210e-482b-a961-3e132fe02d15")},
                new City {Id = new Guid("0966f27d-a998-4eff-bf88-9e8f47ad497c"), Name = "Bydgoszcz", ProvinceId = new Guid("23a0abb8-e9b6-4f7e-99e3-edef8989f5f4")},
                new City {Id = new Guid("a238f198-3bed-4cdd-8c03-c0ed8371fff6"), Name = "Lublin", ProvinceId = new Guid("4afb93c9-f1e7-49b9-ab9c-7152d98c51c9")},
                new City {Id = new Guid("44c2b1ef-622b-4c60-bd54-f104cbfc4ace"), Name = "Zielona Góra", ProvinceId = new Guid("8e56be84-1e53-48cc-8cfd-efe9de55091e")},
                new City {Id = new Guid("2317669a-df86-47a8-bdca-36dd6ffac43a"), Name = "Łódź", ProvinceId = new Guid("0ea63e3a-31f4-4b49-a908-1f85152d3e67")},
                new City {Id = new Guid("a0e9046a-ceff-4dcf-a9b9-aa963850ecae"), Name = "Kraków", ProvinceId = new Guid("424d832e-b345-4d77-be86-61b57018052e")},
                new City {Id = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"), Name = "Warszawa", ProvinceId = new Guid("263e6778-60e5-4caa-9389-a57a6505cfab")},
                new City {Id = new Guid("e5a73afd-cf08-4a97-8b8b-44fad559ac47"), Name = "Opole", ProvinceId = new Guid("39f1ab90-d13a-4e8b-ba4b-38e690463c0b")},
                new City {Id = new Guid("6eae3a80-7f29-4a8f-b4ae-7d0ffe2f27d1"), Name = "Rzeszów", ProvinceId = new Guid("09e64566-4f17-4e1e-af07-fe3651ee5784")},
                new City {Id = new Guid("797550f3-975d-4a22-912b-9febd426c5f9"), Name = "Białystok", ProvinceId = new Guid("b543d147-8550-46b0-8035-2027e54028a4")},
                new City {Id = new Guid("2b22474e-8adb-4e82-915a-d253f6ac5d78"), Name = "Gdańsk", ProvinceId = new Guid("628490fe-563f-4a2f-9938-0d0ef543575f")},
                new City {Id = new Guid("9c635480-4bac-415a-a9ea-3a5413edd398"), Name = "Katowice", ProvinceId = new Guid("114b7d3e-3c94-4d56-9139-4e232f6fdad4")},
                new City {Id = new Guid("2d3aa1f8-2c04-430d-b1e4-a9cf8a5bb844"), Name = "Kielce", ProvinceId = new Guid("b7fbea40-841f-41c5-b8b4-802639e09211")},
                new City {Id = new Guid("67d6d5ba-d6f4-4bc4-bbec-3c786dcdd763"), Name = "Olsztyn", ProvinceId = new Guid("0751b70c-ca54-4bfc-ac68-61468112fbcb")},
                new City {Id = new Guid("21a02399-e314-4c87-9fbe-435ca0975187"), Name = "Poznań", ProvinceId = new Guid("79ff058e-cdf0-4dc0-a47f-397988305bf1")},
                new City {Id = new Guid("0abfc740-3794-4bdd-87c8-9cc4e3e53e85"), Name = "Szczecin", ProvinceId = new Guid("61f9e794-c161-4df2-b7fe-01cee90a62cc")}
            });
        }

        public static void SeedTestTypes(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestType>().HasData(new[]
            {
                new TestType {Id = new Guid("e3b9c042-6910-4353-8c47-8dacf77b149b"), Name = "Test antygenowy"},
                new TestType {Id = new Guid("7a273b91-7d73-4060-ab98-77d66ef1a187"), Name = "Test RT-PCR/RT-LAMP"},
                new TestType {Id = new Guid("867a5e9c-32c2-49ea-b540-9f300f94416e"), Name = "Test na przeciwciała"}
            });
        }

        public static void SeedUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(MigrationUsers.Select(x => new User
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                PasswordHash = x.Password.Item1,
                PasswordSalt = x.Password.Item2,
                Date = DateTime.Now,
                IsActive = true,
                RoleId = x.RoleId
            }).ToArray());
        }

        public record MigrationUser(Guid Id, string Name, string Surname, (byte[], byte[]) Password, string Email, string PhoneNumber, Guid RoleId);

        public static List<MigrationUser> MigrationUsers { get; } = new List<MigrationUser> {
            new MigrationUser
            (
                Id: new Guid("3e7e4cc3-d3e1-4917-9647-42422b4c429a"),
                Name: "Jan",
                Surname: "Kowalski",
                Password: HashPassword("password123"),
                Email: "j.kowalski@gmail.com",
                PhoneNumber: "123456789",
                RoleId: new Guid("af4fafea-b5fe-4a17-be95-60f35f4cca0a")
            ),
            new MigrationUser
            (
                Id: new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"),
                Name: "Adam",
                Surname: "Nowak",
                Password: HashPassword("password123"),
                Email: "a.nowak@gmail.com",
                PhoneNumber: "987654321",
                RoleId: new Guid("64afcb40-4e45-4b94-9f31-19b57c20027f")
            ),
            new MigrationUser
            (
                Id: new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"),
                Name: "Szymon",
                Surname: "Kowalczyk",
                Password: HashPassword("password123"),
                Email: "s.kowalczyk@gmail.com",
                PhoneNumber: "456123789",
                RoleId: new Guid("0c051c70-61a2-44bb-bf9c-691bf30a9c11")
            )
        };

        public static (byte[], byte[]) HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            HMACSHA512 hmac = new HMACSHA512();

            var PasswordSalt = hmac.Key;
            var PasswordHash = hmac.ComputeHash(System.Text.ASCIIEncoding.UTF8.GetBytes(password));

            return (PasswordHash, PasswordSalt);
        }
    }
}
