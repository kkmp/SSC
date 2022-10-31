using Microsoft.EntityFrameworkCore;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.DTO.Patient;
using System.Linq.Expressions;

namespace SSC.Data.Repositories
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        private readonly DataContext context;
        private readonly IUnitOfWork unitOfWork;

        public PatientRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<DbResult<Patient>> AddPatient(PatientCreateDTO patient, Guid issuerId)
        {
            var peselValidator = new PeselValidator(patient.Pesel);
            var citizenship = await unitOfWork.CitizenshipRepository.GetCitizenship(patient.CitizenshipId);
            var city = await unitOfWork.CityRepository.GetCity(patient.CityId);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => GetPatientByPesel(patient.Pesel).Result != null, "Pacjent został już dodany" },
                { () => !peselValidator.Valid, "Niepoprawny numer PESEL" },
                { () => peselValidator.Date != patient.BirthDate, "Data urodzenia nie jest powiązana z numerem PESEL" },
                { () => peselValidator.Sex != patient.Sex.ToString(), "Płeć nie jest powiązana z numerem PESEL" },
                { () => city == null, "Miasto nie istnieje" },
                { () => citizenship == null, "Obywatelstwo nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            Patient newPatient = unitOfWork.Mapper.Map<Patient>(patient);

            newPatient.City = city;
            newPatient.Citizenship = citizenship;
            newPatient.UserId = issuerId;

            await context.AddAsync(newPatient);
            await context.SaveChangesAsync();

            return DbResult<Patient>.CreateSuccess("Pacjent został dodany", newPatient);
        }

        public async Task<DbResult<Patient>> EditPatient(PatientUpdateDTO patient, Guid issuerId)
        {
            var patientToCheck = await GetPatient(patient.Id);
            var citizenship = await unitOfWork.CitizenshipRepository.GetCitizenship(patient.CitizenshipId);
            var city = await unitOfWork.CityRepository.GetCity(patient.CityId);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => GetPatient(patient.Id).Result == null, "Pacjent nie istnieje" },
               { () => city == null, "Miasto nie istnieje" },
               { () => citizenship == null, "Obywatelstwo nie istnieje" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            unitOfWork.Mapper.Map(patient, patientToCheck);

            context.Update(patientToCheck);
            await context.SaveChangesAsync();

            return DbResult<Patient>.CreateSuccess("Pacjent został zedytowany", patientToCheck);
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
               { () => GetPatient(patientId).Result == null, "Pacjent nie istnieje" },
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

            return DbResult<Patient>.CreateSuccess("Powodzenie", data);
        }

        public async Task<List<Patient>> RecentlyAddedPatients(int quantity, Guid issuerId)
        {
            return await context.Patients.OrderByDescending(x => x.AddDate).Take(quantity).ToListAsync();
        }

        private async Task<Patient> GetPatientByPesel(string pesel) => await context.Patients.FirstOrDefaultAsync(x => x.Pesel == pesel);
    }
}
