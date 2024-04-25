using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    //[Route("api/[controller]")] <-- this will automatically add the controller name to the URI.
    public class ClassCitiesController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetCities() 
        {
            return new JsonResult(new List<object>
            {
                new { id = 1, Name = "New York City"},
                new {id = 2 , Name = "Antwerp"}
            } );
        }
    }
}
