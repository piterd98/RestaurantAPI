using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantAPI.Entities;

namespace RestaurantAPI
{
    public class RestaurantSeeder
    {
	    private readonly RestaurantDBContext _dbContext;

	    public RestaurantSeeder(RestaurantDBContext dbContext)
	    {
		    _dbContext = dbContext;
	    }
	    public void Seed()
	    {
		    if (_dbContext.Database.CanConnect())
		    {
			    if (!_dbContext.Restaurants.Any())
			    {
				    var restaurants = GetRestaurants();
				    _dbContext.Restaurants.AddRange(restaurants);
				    _dbContext.SaveChanges();
			    }
		    }

	    }

	    private IEnumerable<Restaurant> GetRestaurants()
	    {
		    var restaurants = new List<Restaurant>()
		    {
			    new Restaurant()
			    {
				    Name = "KFC",
				    Category = "Fast food",
				    Description = " KENTUCKY FRIED CHICKEN TO KURCZAKI SĄ",
				    ContactEmail = "contact@kfc.com",
				    HasDelivery = true,
				    Dishes = new List<Dish>()
				    {
					    new Dish()
					    {
						    Name = "Chicken nuggets",
						    Price = 5.30M
					    }
				    },
				    Address = new Address()
				    {
					    City = "Krakow",
					    Street = "Długa 5",
					    PostalCode = "30-001"
				    }
			    }

		    };
		    return restaurants;
	    }
    }
}
