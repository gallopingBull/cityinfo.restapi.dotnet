using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    //[Route("api/[controller]")] <-- this will automatically add the controller name to the URI.
    public class CitiesController : ControllerBase
    {

        private readonly CitiesDataStore _cityDataStore;

        public CitiesController(CitiesDataStore cityDataStore) 
        { 
            _cityDataStore = cityDataStore ?? throw new ArgumentNullException(nameof(cityDataStore)); 
        }

        [HttpGet]
        public ActionResult<CityDto> GetCities() 
        {
            return Ok(_cityDataStore.Cities);
        }

        [HttpGet("{id}")]   
        public ActionResult<CityDto> GetCity(int id) 
        {
            var cityToReturn = _cityDataStore.Cities.FirstOrDefault(c => c.Id == id);

            if (cityToReturn == null) 
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }
    }
}
