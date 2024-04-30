using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; } 

        public DbSet<PointOfInterest> PointsOfInterest { get; set;}

        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<City>()
                    .HasData(
                new City("New York City")
                {
                    Id = 1,
                    Description = "The one with the big park"
                },
                new City("Antwerp")
                {
                    Id = 2,
                    Description = "The one with the cathedral"
                },
                new City("Paris")
                {
                    Id = 3,
                    Description = "The one with the tower"
                },
                new City("Japan")
                {
                    Id = 4,
                    Description = "The one with the crazys shit"
                });

            modelBuilder.Entity<PointOfInterest>()
                    .HasData(
                new PointOfInterest("Cental Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "fuck"
                },
                new PointOfInterest("Empire State Buiilding")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "my ass"
                },
                new PointOfInterest("Cathedrawl")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "some old shit"
                },
                new PointOfInterest("Louve")
                {
                    Id = 4,
                    CityId = 2,
                    Description = "some french shit"
                },
                new PointOfInterest("Hiroshima")
                {
                    Id = 5,
                    CityId = 4,
                    Description = "boom"
                });


            base.OnModelCreating(modelBuilder);     
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("connectionstring");
        //    base.OnConfiguring(optionsBuilder); 
        //}
    }
}
