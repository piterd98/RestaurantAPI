using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {
	    private readonly RestaurantDBContext _dbContext;

	    [HttpGet]

	    public ActionResult<IEnumerable<Restaurant>> GetAll()
	    {
		    //EF tworzy kod sql, ktory pobierze z bazy restauracje i zwroci w tej zmiennej
		    var restaurants = _dbContext
			    .Restaurants
			    .ToList();
		    return Ok(restaurants);
	    }

	    public RestaurantController(RestaurantDBContext dbContext)
	    {
		    _dbContext = dbContext;
	    }
    }
}
