using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.DTO;
using SSC.DTO.Patient;
using SSC.Models;
using SSC.Tools;
using System.ComponentModel.DataAnnotations;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : CommonController
    {
        private readonly IPatientRepository patientRepository;
        private readonly IMapper mapper;

        public PatientController(IPatientRepository patientRepository, IMapper mapper)
        {
            this.patientRepository = patientRepository;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpPost("addPatient")]
        public async Task<IActionResult> AddPatient(PatientCreateDTO patient)
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
        [HttpGet("patientDetails/{patientId}")]
        public async Task<IActionResult> PatientDetails(Guid patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await patientRepository.PatientDetails(patientId);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(mapper.Map<PatientGetDTO>(result.Data));
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [Authorize]
        [HttpGet("filterPatients/{pageNr}/{option}/{orderType}/{sex}")]
        [HttpGet("filterPatients/{pageNr}/{option}/{orderType}/{sex}/{searchName}")]
        public async Task<IActionResult> FilterPatients([Range(1, 100000000)] int pageNr, string option, string orderType, string sex, string? searchName)
        {
            IEnumerable<Patient> result = new List<Patient>(); 
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
                    || (x.Name + " " + x.Surname).ToLower().Contains(searchName));
            }

            result = result.GetPage(pageNr, 3).ToList();

            return Ok(mapper.Map<List<PatientOverallGetDTO>>(result));
        }

        [Authorize]
        [HttpGet("recentlyAddedPatients")]
        public async Task<IActionResult> RecentlyAddedPatients()
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();

                var result = await patientRepository.RecentlyAddedPatients(3, issuerId);

                return Ok(mapper.Map<List<PatientOverallGetDTO>>(result));
            }
            return BadRequest(new { message = "Invalid data" });
        }
    }
}
