using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }
        //public static CitiesDataStore Current { get; set; }  = new CitiesDataStore();   
        public CitiesDataStore()
        {
            // init dummy data
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "The one with that big park.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "at night it's known as rape central by female joggers."
                        },
                         new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Central Park",
                            Description = "at night it's known as rape central by female joggers."
                        }
                    }
                },
                new CityDto()
                {
                    Id = 2,

                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never finished.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 3,
                            Name = "The place in that movie",
                            Description = "It was the location from the hit classic film Deuce Bigalo 2: Europen Jigalo."
                        },
                         new PointOfInterestDto()
                        {
                            Id = 4,
                            Name = "Some gay european bar",
                            Description = "At night it's known as rape central by male joggers."
                        }
                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with the big tower.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 5,
                            Name = "Eiffel Tower",
                            Description = "Not the thing you did with a buddy and a mutual female friend ;)"
                        },
                         new PointOfInterestDto()
                        {
                            Id = 6,
                            Name = "Ratatouile Restaurant",
                            Description = "The restaurant from the hit movie Ratatoutile"
                        }
                    }
                }
            };
        }
    }
}
