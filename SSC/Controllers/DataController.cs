using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.DTO;
using SSC.DTO.Citizenship;
using SSC.DTO.City;
using SSC.DTO.TestType;

namespace SSC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : CommonController
    {
        private readonly ICityRepository cityRepository;
        private readonly ICitizenshipRepository citizenshipRepository;
        private readonly ITestTypeRepository testTypeRepository;
        private readonly IDiseaseCourseRepository diseaseCourseRepository;
        private readonly IMapper mapper;

        public DataController(ICityRepository cityRepository, ICitizenshipRepository citizenshipRepository, ITestTypeRepository testTypeRepository, IDiseaseCourseRepository diseaseCourseRepository, IMapper mapper)
        {
            this.cityRepository = cityRepository;
            this.citizenshipRepository = citizenshipRepository;
            this.testTypeRepository = testTypeRepository;
            this.diseaseCourseRepository = diseaseCourseRepository;
            this.mapper = mapper;
        }

        [HttpGet("getCities")]
        public async Task<IActionResult> GetCities()
        {
                var result = await cityRepository.GetCities();
                return Ok(mapper.Map<List<CityGetDTO>>(result));
        }

        [HttpGet("getCitizenships")]
        public async Task<IActionResult> GetCitizenships()
        {
            var result = await citizenshipRepository.GetCitizenships();
            return Ok(mapper.Map<List<CitizenshipGetDTO>>(result));
        }

        [HttpGet("getTestTypes")]
        public async Task<IActionResult> GetTestTypes()
        {
            var result = await testTypeRepository.GetTestTypes();
            return Ok(mapper.Map<List<TestTypeGetDTO>>(result));
        }

        [HttpGet("getDiseaseCourses")]
        public async Task<IActionResult> GetDiseaseCourses()
        {
            var result = await diseaseCourseRepository.GetDiseaseCourses();
            return Ok(mapper.Map<List<DiseaseCourseGetDTO>>(result));
        }
    }
}
