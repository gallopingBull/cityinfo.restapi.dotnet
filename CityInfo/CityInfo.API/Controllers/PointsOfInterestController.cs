using AutoMapper;
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
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController (ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository , IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _mailService = mailService ?? throw new ArgumentNullException (nameof (mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]   
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId)) 
            {
                _logger.LogInformation($"city with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();  
            }

            var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync (cityId);  

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);  

            if (pointOfInterest == null) 
            { 
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                }, 
                createdPointOfInterestToReturn);
        }
        
        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult<PointOfInterestDto>> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEnity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEnity == null) { return NotFound(); }

            _mapper.Map(pointOfInterest, pointOfInterestEnity);
            
            await _cityInfoRepository.SaveChangesAsync();   

            return NoContent();
        }
        
        //[HttpPatch("{pointofinterestid}")]
        //public ActionResult<PointOfInterestDto> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null) { return NotFound(); }
        //
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        //    if (pointOfInterestFromStore == null) { return NotFound(); }
        //
        //    var PointOfInterestToPatch = new PointOfInterestForUpdateDto()
        //    {
        //        Name = pointOfInterestFromStore.Name,
        //        Description = pointOfInterestFromStore.Description,
        //    };
        //
        //    patchDocument.ApplyTo(PointOfInterestToPatch, ModelState);
        //
        //    if (!ModelState.IsValid) { return BadRequest(ModelState); }
        //
        //    if(!TryValidateModel(PointOfInterestToPatch)) { return BadRequest(ModelState); }    
        //
        //    pointOfInterestFromStore.Name = PointOfInterestToPatch.Name;
        //    pointOfInterestFromStore.Description = PointOfInterestToPatch.Description;
        //
        //    return NoContent();
        //}
        //
        //[HttpDelete("{pointofinterestid}")]
        //public ActionResult<PointOfInterestDto> DeletePointOfInterest(int cityId, int pointOfInterestId)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null) { return NotFound(); }
        //
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        //    if (pointOfInterestFromStore == null) { return NotFound(); }
        //
        //    city.PointsOfInterest.Remove(pointOfInterestFromStore);
        //    _mailService.Send("Point of interest deleted.", 
        //        $"Point of interest {pointOfInterestFromStore.Name} wth id {pointOfInterestFromStore.Id} was deleted");
        //
        //    //if (!ModelState.IsValid) { return BadRequest(ModelState); }
        //
        //    return NoContent();
        //}
    }
}
