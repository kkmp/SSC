using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Models;
using System.Linq.Expressions;
using System.Web.Http;

namespace SSC.Data.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly DataContext context;

        public PatientRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<DbResult<Patient>> AddPatient(PatientViewModel p, Guid issuerId)
        {
            if (await GetPatientPesel(p.Pesel) != null)
            {
                return DbResult<Patient>.CreateFail("Patient has already been added");
            }

            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == issuerId);
            if(user == null)
            {
                return DbResult<Patient>.CreateFail("Invalid issuer id");
            }

            var peselValidator = new PeselValidator(p.Pesel);
            if(!peselValidator.Valid)
            {
                return DbResult<Patient>.CreateFail("Invalid pesel");
            }
            if(peselValidator.Date != p.BirthDate)
            {
                return DbResult<Patient>.CreateFail("Birthdate is not associated with pesel");
            }
            if(peselValidator.Sex != p.Sex.ToString())
            {
                return DbResult<Patient>.CreateFail("Sex is not associated with pesel");
            }

            var patient = new Patient
            {
                Pesel = p.Pesel,
                Name = p.Name,
                Surname = p.Surname,
                Sex = p.Sex,
                BirthDate = p.BirthDate,
                Street = p.Street,
                Address = p.Address,
                PhoneNumber = p.PhoneNumber,
                User = user,
            };

            var city = await context.Cities.FirstOrDefaultAsync(x => x.Name == p.City); //dopisywanie miasta do województwa
            patient.City = city;
            patient.Citizenship = await context.Citizenships.FirstOrDefaultAsync(x => x.Name == p.Citizenship);

            await context.AddAsync(patient);
            await context.SaveChangesAsync();
            return DbResult<Patient>.CreateSuccess("Patient added", patient);
        }

        public async Task<Patient> GetPatient(Guid patientId)
        {
            return await context.Patients.FirstOrDefaultAsync(x => x.Id == patientId);
        }

        public async Task<List<Patient>> GetPatients()
        {
            return await context.Patients.ToListAsync();
        }

        public async Task<List<Patient>> GetPatients(Expression<Func<Patient, bool>> condition)
        {
            return await context.Patients.Where(condition).ToListAsync();
        }

        public async Task<Patient> PatientDetails(Guid patientId)
        {
            return await context.Patients
                .Include(x => x.City)
                .Include(x => x.Citizenship)
                .Include(x => x.City.Province)
                .FirstOrDefaultAsync(x => x.Id == patientId);
        }

        private async Task<Patient> GetPatientPesel(string pesel) => await context.Patients.FirstOrDefaultAsync(x => x.Pesel == pesel);

    }
}
