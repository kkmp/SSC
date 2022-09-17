using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.DTO.Citizenship;
using SSC.DTO.City;

namespace SSC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : CommonController
    {
        private readonly ICityRepository cityRepository;
        private readonly ICitizenshipRepository citizenshipRepository;
        private readonly IMapper mapper;

        public DataController(ICityRepository cityRepository, ICitizenshipRepository citizenshipRepository, IMapper mapper)
        {
            this.cityRepository = cityRepository;
            this.citizenshipRepository = citizenshipRepository;
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
    }
}
