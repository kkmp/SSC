﻿using SSC.Data.Models;
using SSC.DTO.Patient;
using System.Linq.Expressions;

namespace SSC.Data.Repositories
{
    public interface IPatientRepository
    {
        Task<DbResult<Patient>> AddPatient(PatientCreateDTO patient, Guid issuerId);
        Task<DbResult<Patient>> EditPatient(PatientUpdateDTO patient, Guid issuerId);
        Task<List<Patient>> GetPatients();
        Task<Patient> GetPatient(Guid patientId);
        Task<List<Patient>> GetPatients(Expression<Func<Patient, bool>> condition);
        Task<DbResult<Patient>> PatientDetails(Guid patientId);
        Task<List<Patient>> RecentlyAddedPatients(int quantity, Guid issuerId);
    }
}
