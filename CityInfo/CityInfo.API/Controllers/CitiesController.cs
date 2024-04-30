using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    //[Route("api/[controller]")] <-- this will automatically add the controller name to the URI.
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper) 
        { 
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));   
        }

        [HttpGet]
        public async Task<ActionResult<CityWithoutPointsOfInterestDto>> GetCities() 
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
            //return Ok(_cityDataStore.Cities);
        }

        [HttpGet("{id}")]   
        public ActionResult<CityDto> GetCity(int id) 
        {

            //var cityToReturn = _cityDataStore.Cities.FirstOrDefault(c => c.Id == id);
            //
            //if (cityToReturn == null) 
            //{
            //    return NotFound();
            //}
            //
            //return Ok(cityToReturn);
            return Ok();
        }
    }
}
