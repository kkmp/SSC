using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.DTO;
using SSC.Models;
using System.Web.Http.Cors;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class PatientController : CommonController
    {
        private IConfiguration _config;
        private readonly IPatientRepository patientRepository;
        private readonly IMapper mapper;

        public PatientController(IConfiguration config, 
            IPatientRepository patientRepository,
            IMapper mapper)
        {
            _config = config;
            this.patientRepository = patientRepository;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpPost("addPatient")]
        public async Task<IActionResult> AddPatient([FromBody] PatientViewModel patient)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await patientRepository.AddPatient(patient, id);
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
        [HttpGet("showPatients")]
        public async Task<IActionResult> ShowPatients()
        {
            var result = await patientRepository.GetPatients();
            return Ok(result.Select(x => new
            {
                x.Id,
                x.Name,
                x.Surname,
                x.Pesel, 
                BirthDate = x.BirthDate?.ToString(), 
            }));
        }

        [Authorize]
        [HttpGet("patientDetails")]
        public async Task<IActionResult> PatientDetails([FromBody] IdViewModel patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await patientRepository.PatientDetails(patientId.Id);
                return Ok(new
                {
                    result.Name,
                    result.Surname,
                    result.Pesel,
                    BirthDate = result.BirthDate?.ToString(),
                    result.Sex,
                    result.Street,
                    result.Address,
                    result.PhoneNumber,
                    City = result.City.Name, // Czy tak można >>>>>>>>>>>> ???
                    Province = result.City.Province.Name,
                    Citizenship = result.Citizenship.Name
                });
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
            }

            switch (option)
            {
                case "surname":
                    result = (orderType == "descending" ? result.OrderByDescending(x => x.Surname) : result.OrderBy(x => x.Surname)).ToList();
                    break;
                case "birthdate":
                    result = (orderType == "descending" ? result.OrderByDescending(x => x.BirthDate) : result.OrderBy(x => x.BirthDate)).ToList();
                    break;
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
            return Ok(mapper.Map<List<PatientDTO>>(result));
        }

    }
}
