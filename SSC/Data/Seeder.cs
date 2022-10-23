using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data
{
    public static class Seeder
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(new[]
            {
                new Role {Id = new Guid("64afcb40-4e45-4b94-9f31-19b57c20027f"), Name = Roles.Doctor, Description = "Leczy ludzi"},
                new Role {Id = new Guid("0c051c70-61a2-44bb-bf9c-691bf30a9c11"), Name = Roles.LabAssistant, Description = "Sprawdza ludzi"},
                new Role {Id = new Guid("af4fafea-b5fe-4a17-be95-60f35f4cca0a"), Name = Roles.Admin, Description = "Zarzadza ludzi"}
            });
        }

        public static void SeedTreatmentStatuses(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TreatmentStatus>().HasData(new[]
            {
                new TreatmentStatus {Id = new Guid("6eac697b-8839-4d87-bf3c-51ba2308e75f"), Name = TreatmentStatusOptions.Started},
                new TreatmentStatus {Id = new Guid("6eac697b-8839-4d87-bf3c-51ba2308e75e"), Name = TreatmentStatusOptions.Death}
            });
        }

        public static void SeedDiseaseCourses(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiseaseCourse>().HasData(new[]
            {
                new DiseaseCourse {Id = new Guid("ce4d9efd-4065-4c7d-ad64-20ad0e2b65c1"), Name = DiseaseCourseOptions.Cough, Description = "Opis kaszlu"},
                new DiseaseCourse {Id = new Guid("87feb19d-84c7-4332-856e-cbeefb08a66d"), Name = DiseaseCourseOptions.SoreThroat, Description = "Opis bólu gardła"},
            });
        }
    }
}
