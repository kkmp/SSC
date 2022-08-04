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

        public static void SeedPatients(this ModelBuilder modelBuilder)
        {

        }
    }
}
