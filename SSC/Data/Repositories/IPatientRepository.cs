using SSC.Data.Models;
using SSC.Models;
using System.Linq.Expressions;

namespace SSC.Data.Repositories
{
    public interface IPatientRepository
    {
        Task<DbResult<Patient>> AddPatient(PatientViewModel p, Guid issuerId);
        Task<List<Patient>> GetPatients();
        Task<Patient> GetPatient(Guid patientId);
        Task<List<Patient>> GetPatients(Expression<Func<Patient, bool>> condition);
        Task<Patient> PatientDetails(Guid patientId);
    }
}
