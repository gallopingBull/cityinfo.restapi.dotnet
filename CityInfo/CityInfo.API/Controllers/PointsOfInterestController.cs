using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {

        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly CitiesDataStore _cityDataStore;

        public PointsOfInterestController (ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _mailService = mailService ?? throw new ArgumentNullException (nameof (mailService));
            _cityDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet]   
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            try
            {
                var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest!");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
                return StatusCode(500, "A problem occured while handling your request.");
            }
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) { return NotFound(); }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
            if(pointOfInterest == null) { return NotFound(); }

            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) { return NotFound(); }

            var maxPointOfInterestId = _cityDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description,
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", 
                new { cityId = cityId, pointOfInterestId = finalPointOfInterest.Id }, 
                finalPointOfInterest);
        }

        [HttpPut("{pointofinterestid}")]
        public ActionResult<PointOfInterestDto> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) { return NotFound(); }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
            if(pointOfInterestFromStore == null) { return NotFound(); }
           
            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;
            
            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public ActionResult<PointOfInterestDto> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) { return NotFound(); }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
            if (pointOfInterestFromStore == null) { return NotFound(); }

            var PointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description,
            };

            patchDocument.ApplyTo(PointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if(!TryValidateModel(PointOfInterestToPatch)) { return BadRequest(ModelState); }    

            pointOfInterestFromStore.Name = PointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = PointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public ActionResult<PointOfInterestDto> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _cityDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) { return NotFound(); }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
            if (pointOfInterestFromStore == null) { return NotFound(); }

            city.PointsOfInterest.Remove(pointOfInterestFromStore);
            _mailService.Send("Point of interest deleted.", 
                $"Point of interest {pointOfInterestFromStore.Name} wth id {pointOfInterestFromStore.Id} was deleted");

            //if (!ModelState.IsValid) { return BadRequest(ModelState); }

            return NoContent();
        }
    }
}
