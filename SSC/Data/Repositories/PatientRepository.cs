using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.DTO.Patient;
using SSC.Models;
using System.Linq.Expressions;
using System.Web.Http;

namespace SSC.Data.Repositories
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public PatientRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<DbResult<Patient>> AddPatient(PatientViewModel patient, Guid issuerId)
        {
            var peselValidator = new PeselValidator(patient.Pesel);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => GetPatientByPesel(patient.Pesel).Result != null, "Patient has already been added" },
                { () => !peselValidator.Valid, "Invalid pesel" },
                { () => peselValidator.Date != patient.BirthDate, "Birthdate is not associated with pesel" },
                { () => peselValidator.Sex != patient.Sex.ToString(), "Sex is not associated with pesel" },
                { () => !context.Cities.AnyAsync(x => x.Name == patient.CityName).Result, "City does not exist" },
                { () => !context.Citizenships.AnyAsync(x => x.Name == patient.CitizenshipName).Result, "Citizenship does not exist" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            Patient newPatient = mapper.Map<Patient>(patient);

            var city = await context.Cities.FirstOrDefaultAsync(x => x.Name == patient.CityName); //dopisywanie miasta do województwa

            newPatient.City = city;
            newPatient.Citizenship = await context.Citizenships.FirstOrDefaultAsync(x => x.Name == patient.CitizenshipName);
            newPatient.UserId = issuerId;

            await context.AddAsync(newPatient);
            await context.SaveChangesAsync();

            return DbResult<Patient>.CreateSuccess("Patient added", newPatient);
        }

        public async Task<DbResult<Patient>> EditPatient(PatientUpdateDTO patient, Guid issuerId)
        {
            var patientToCheck = await GetPatient(patient.Id);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => GetPatient(patient.Id).Result == null, "Patient does not exist" },
               { () => !context.Cities.AnyAsync(x => x.Name == patient.CityName).Result, "City does not exist" },
               { () => !context.Citizenships.AnyAsync(x => x.Name == patient.CitizenshipName).Result, "Citizenship does not exist" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            mapper.Map(patient, patientToCheck);

            patientToCheck.City = await context.Cities.FirstOrDefaultAsync(x => x.Name == patient.CityName);
            patientToCheck.Citizenship = await context.Citizenships.FirstOrDefaultAsync(x => x.Name == patient.CitizenshipName);

            context.Update(patientToCheck);
            await context.SaveChangesAsync();

            return DbResult<Patient>.CreateSuccess("Patient has been edited", patientToCheck);
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

        public async Task<DbResult<Patient>> PatientDetails(Guid patientId)
        {
            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => GetPatient(patientId).Result == null, "Patient does not exist" },
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            var data = await context.Patients
                .Include(x => x.City)
                .Include(x => x.Citizenship)
                .Include(x => x.City.Province)
                .FirstOrDefaultAsync(x => x.Id == patientId);

            return DbResult<Patient>.CreateSuccess("Success", data);
        }

        private async Task<Patient> GetPatientByPesel(string pesel) => await context.Patients.FirstOrDefaultAsync(x => x.Pesel == pesel);
    }
}
