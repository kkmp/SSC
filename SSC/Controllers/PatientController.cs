using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.DTO.Patient;
using SSC.Tools;
using System.ComponentModel.DataAnnotations;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : CommonController
    {
        private readonly IUnitOfWork unitOfWork;

        public PatientController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost("addPatient")]
        public async Task<IActionResult> AddPatient(PatientCreateDTO patient)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await unitOfWork.PatientRepository.AddPatient(patient, issuerId);

                if (result.Success)
                {
                    return Ok(new { message = result.Message });
                }
                else
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [Authorize]
        [HttpPut("editPatient")]
        public async Task<IActionResult> EditPatient(PatientUpdateDTO patient)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await unitOfWork.PatientRepository.EditPatient(patient, issuerId);
                if (result.Success)
                {
                    return Ok(new { message = result.Message });
                }
                else
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [Authorize]
        [HttpGet("patientDetails/{patientId}")]
        public async Task<IActionResult> PatientDetails(Guid patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await unitOfWork.PatientRepository.PatientDetails(patientId);

                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }

                return Ok(unitOfWork.Mapper.Map<PatientGetDTO>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [Authorize]
        [HttpGet("filterPatients/{pageNr}/{option}/{orderType}/{sex}")]
        [HttpGet("filterPatients/{pageNr}/{option}/{orderType}/{sex}/{searchName}")]
        public async Task<IActionResult> FilterPatients([Range(1, 100000000)] int pageNr, string option, string orderType, string sex, string? searchName)
        {
            IEnumerable<Patient> result = new List<Patient>();
            switch (sex)
            {
                case "both":
                    result = await unitOfWork.PatientRepository.GetPatients();
                    break;
                case "male":
                    result = await unitOfWork.PatientRepository.GetPatients(x => x.Sex == 'M');
                    break;
                case "female":
                    result = await unitOfWork.PatientRepository.GetPatients(x => x.Sex == 'F');
                    break;
                default:
                    return BadRequest(new { errors = new { Message = new string[] { "Niepoprawna opcja wyboru płci" } } });
            }

            switch (option)
            {
                case "surname":
                    result = (orderType == "descending" ? result.OrderByDescending(x => x.Surname) : result.OrderBy(x => x.Surname)).ToList();
                    break;
                case "birthdate":
                    result = (orderType == "descending" ? result.OrderByDescending(x => x.BirthDate) : result.OrderBy(x => x.BirthDate)).ToList();
                    break;
                default:
                    return BadRequest(new { errors = new { Message = new string[] { "Niepoprawna opcja filtrowania" } } });

            }

            if (searchName != null)
            {
                searchName = NormalizeWhiteSpaceExtension.NormalizeWhiteSpaceForLoop(searchName.ToLower().Trim());
                result = result
                    .Where(x => x.Name.ToLower().Contains(searchName)
                    || x.Surname.ToLower().Contains(searchName)
                    || x.Pesel.ToLower().Contains(searchName)
                    || (x.Name + " " + x.Surname).ToLower().Contains(searchName)
                    || (x.Surname + " " + x.Name).ToLower().Contains(searchName));
            }

            result = result.GetPage(pageNr, 3).ToList();

            return Ok(unitOfWork.Mapper.Map<List<PatientOverallGetDTO>>(result));
        }

        [Authorize]
        [HttpGet("recentlyAddedPatients")]
        public async Task<IActionResult> RecentlyAddedPatients()
        {
            var issuerId = GetUserId();

            var result = await unitOfWork.PatientRepository.RecentlyAddedPatients(3, issuerId);

            return Ok(unitOfWork.Mapper.Map<List<PatientOverallGetDTO>>(result));
        }
    }
}
