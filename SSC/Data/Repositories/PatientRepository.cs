﻿using AutoMapper;
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
        private readonly ICityRepository cityRepository;
        private readonly ICitizenshipRepository citizenshipRepository;
        private readonly IMapper mapper;

        public PatientRepository(DataContext context, ICityRepository cityRepository, ICitizenshipRepository citizenshipRepository, IMapper mapper)
        {
            this.context = context;
            this.cityRepository = cityRepository;
            this.citizenshipRepository = citizenshipRepository;
            this.mapper = mapper;
        }

        public async Task<DbResult<Patient>> AddPatient(PatientCreateDTO patient, Guid issuerId)
        {
            var peselValidator = new PeselValidator(patient.Pesel);
            var citizenship = await citizenshipRepository.GetCitizenship(patient.CitizenshipId);
            var city = await cityRepository.GetCity(patient.CityId);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
                { () => GetPatientByPesel(patient.Pesel).Result != null, "Patient has already been added" },
                { () => !peselValidator.Valid, "Invalid pesel" },
                { () => peselValidator.Date != patient.BirthDate, "Birthdate is not associated with pesel" },
                { () => peselValidator.Sex != patient.Sex.ToString(), "Sex is not associated with pesel" },
                { () => city == null, "City does not exist" },
                { () => citizenship == null, "Citizenship does not exist" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            Patient newPatient = mapper.Map<Patient>(patient);

            newPatient.City = city;
            newPatient.Citizenship = citizenship;
            newPatient.UserId = issuerId;

            await context.AddAsync(newPatient);
            await context.SaveChangesAsync();

            return DbResult<Patient>.CreateSuccess("Patient added", newPatient);
        }

        public async Task<DbResult<Patient>> EditPatient(PatientUpdateDTO patient, Guid issuerId)
        {
            var patientToCheck = await GetPatient(patient.Id);
            var citizenship = await citizenshipRepository.GetCitizenship(patient.CitizenshipId);
            var city = await cityRepository.GetCity(patient.CityId);

            Dictionary<Func<bool>, string> conditions = new Dictionary<Func<bool>, string>
            {
               { () => GetPatient(patient.Id).Result == null, "Patient does not exist" },
               { () => city == null, "City does not exist" },
               { () => citizenship == null, "Citizenship does not exist" }
            };

            var result = Validate(conditions);
            if (result != null)
            {
                return result;
            }

            mapper.Map(patient, patientToCheck);

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

        public async Task<List<Patient>> RecentlyAddedPatients(int quantity, Guid issuerId)
        {
            return await context.Patients.OrderByDescending(x => x.AddDate).Take(quantity).ToListAsync();
        }

        private async Task<Patient> GetPatientByPesel(string pesel) => await context.Patients.FirstOrDefaultAsync(x => x.Pesel == pesel);
    }
}
