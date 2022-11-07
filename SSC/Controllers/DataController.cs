using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.UnitOfWork;
using SSC.DTO.Citizenship;
using SSC.DTO.City;
using SSC.DTO.DiseaseCourse;
using SSC.DTO.Place;
using SSC.DTO.Province;
using SSC.DTO.TestType;
using SSC.DTO.TreatmentStatus;

namespace SSC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : CommonController
    {
        private readonly IUnitOfWork unitOfWork;

        public DataController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("getCities")]
        public async Task<IActionResult> GetCities()
        {
                var result = await unitOfWork.CityRepository.GetCities();
                return Ok(unitOfWork.Mapper.Map<List<CityGetDTO>>(result));
        }

        [HttpGet("getCitizenships")]
        public async Task<IActionResult> GetCitizenships()
        {
            var result = await unitOfWork.CitizenshipRepository.GetCitizenships();
            return Ok(unitOfWork.Mapper.Map<List<CitizenshipGetDTO>>(result));
        }

        [HttpGet("getTestTypes")]
        public async Task<IActionResult> GetTestTypes()
        {
            var result = await unitOfWork.TestTypeRepository.GetTestTypes();
            return Ok(unitOfWork.Mapper.Map<List<TestTypeGetDTO>>(result));
        }

        [HttpGet("getDiseaseCourses")]
        public async Task<IActionResult> GetDiseaseCourses()
        {
            var result = await unitOfWork.DiseaseCourseRepository.GetDiseaseCourses();
            return Ok(unitOfWork.Mapper.Map<List<DiseaseCourseGetDTO>>(result));
        }

        [HttpGet("getTreatmentStatuses")]
        public async Task<IActionResult> GetTreatmentStatuses()
        {
            var result = await unitOfWork.TreatmentStatusRepository.GetTreatmentStatuses();
            return Ok(unitOfWork.Mapper.Map<List<TreatmentStatusGetDTO>>(result));
        }

        [HttpGet("getPlaces")]
        public async Task<IActionResult> GetPlaces()
        {
            var result = await unitOfWork.PlaceRepository.GetPlaces();
            return Ok(unitOfWork.Mapper.Map<List<PlaceGetDTO>>(result));
        }

        [HttpGet("getProvinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var result = await unitOfWork.ProvinceRepository.GetProvinces();
            return Ok(unitOfWork.Mapper.Map<List<ProvinceGetDTO>>(result));
        }
    }
}
