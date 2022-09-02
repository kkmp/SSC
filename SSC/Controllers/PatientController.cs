using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.DTO;
using SSC.DTO.Patient;
using SSC.Models;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : CommonController
    {
        private IConfiguration _config;
        private readonly IPatientRepository patientRepository;
        private readonly IMapper mapper;

        public PatientController(IConfiguration config, IPatientRepository patientRepository, IMapper mapper)
        {
            _config = config;
            this.patientRepository = patientRepository;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpPost("addPatient")]
        public async Task<IActionResult> AddPatient(PatientViewModel patient)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await patientRepository.AddPatient(patient, issuerId);
                var msg = new { errors = new { Message = new string[] { result.Message } } };
                if (result.Success)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [Authorize]
        [HttpPut("editPatient")]
        public async Task<IActionResult> EditPatient(PatientUpdateDTO patient)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await patientRepository.EditPatient(patient, issuerId);
                var msg = new { errors = new { Message = new string[] { result.Message } } };
                if (result.Success)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [Authorize]
        [HttpGet("patientDetails")]
        public async Task<IActionResult> PatientDetails(IdViewModel patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await patientRepository.PatientDetails(patientId.Id);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(mapper.Map<PatientDTO>(result.Data));
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [Authorize]
        [HttpGet("filterPatients/{option}/{orderType}/{sex}")]
        [HttpGet("filterPatients/{option}/{orderType}/{sex}/{searchName}")]
        public async Task<IActionResult> FilterPatients(string option, string orderType, string sex, string? searchName)
        {
            List<Patient> result = new List<Patient>(); 
            switch(sex)
            {
                case "both":
                    result = await patientRepository.GetPatients();
                    break;
                case "male":
                    result = await patientRepository.GetPatients(x => x.Sex == 'M');
                    break;
                case "female":
                    result = await patientRepository.GetPatients(x => x.Sex == 'F');
                    break;
                default:
                    return BadRequest(new { message = "Incorrect sex option" });
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
                    return BadRequest(new { message = "Incorrect filter option" });
            }

            if(searchName != null)
            {
                searchName = searchName.ToLower();
                result = result
                    .Where(x => x.Name.ToLower().Contains(searchName) 
                    || x.Surname.ToLower().Contains(searchName) 
                    || x.Pesel.ToLower().Contains(searchName)
                    || (x.Name + " " + x.Surname).ToLower().Contains(searchName))
                    .ToList();
            }
            return Ok(mapper.Map<List<PatientOverallDTO>>(result));
        }

        [Authorize]
        [HttpGet("recentlyAddedPatients")]
        public async Task<IActionResult> RecentlyAddedPatients()
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();

                var result = await patientRepository.RecentlyAddedPatients(0, issuerId);

                return Ok(mapper.Map<List<PatientOverallDTO>>(result));
            }
            return BadRequest(new { message = "Invalid data" });
        }
    }
}
