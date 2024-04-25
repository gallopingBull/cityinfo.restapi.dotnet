using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    //[Route("api/[controller]")] <-- this will automatically add the controller name to the URI.
    public class ClassCitiesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<CityDto> GetCities() 
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]   
        public ActionResult<CityDto> GetCity(int id) 
        {
            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            if (cityToReturn == null) 
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }
    }
}
