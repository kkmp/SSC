using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;

namespace SSC.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.SeedRoles();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Citizenship> Citizenships { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<DiseaseCourse> DiseaseCourses { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<TreatmentDiseaseCourse> TreatmentDiseaseCourses { get; set; }
        public DbSet<TreatmentStatus> TreatmentStatuses { get; set; }
        public DbSet<Patient> Patients { get; set; }

    }
}
