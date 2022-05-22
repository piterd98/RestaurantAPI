using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
	[ApiController]
	[Authorize]
    public class RestaurantController : ControllerBase
    {
	    private readonly IRestaurantService _restaurantService;
	    public RestaurantController(IRestaurantService restaurantService)
	    {
		    _restaurantService = restaurantService;
	    }

		[HttpGet]
		[AllowAnonymous]
	    public ActionResult<IEnumerable<RestaurantDto>> GetAll([FromQuery] RestaurantQuery query)
	    {
		    var restaurantsDtos = _restaurantService.GetAll(query);
			return Ok(restaurantsDtos);
	    }

	    [HttpGet("{id}")]
		[Authorize(Policy = "CreatedAtLeast2Restaurants")]
	    public ActionResult<Restaurant> Get([FromRoute] int id)
	    {

		    var restaurant = _restaurantService.GetById(id);
		    if (restaurant is null)
		    {
			    return NotFound();
		    }

		   
		    return Ok(restaurant);
	    }

	    [HttpDelete("{id}")]
	    public ActionResult Delete([FromRoute] int id)
	    {
		     _restaurantService.Delete(id);


		     return NoContent();

	    }

	    [HttpPost]
	    public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
	    {
		    //if (!ModelState.IsValid) [ApiController] zastępuje ten fragment kodu 
		    //{
			   // return BadRequest(ModelState);
		    //}
			//HttpContext.User.IsInRole("Admin") Sprawdzenie Roli uzytkownika
			var userId = User.FindFirst(c=> c.Type == ClaimTypes.NameIdentifier).Value;
			var id = _restaurantService.Create(dto);
		    
		    return Created($"/api/restaurant/{id}",null);
	    }

	    [HttpPut("{id}")]
	    public ActionResult ModifyRestaurant([FromBody] ModifyRestaurantDto dto,[FromRoute] int id)
	    {
		    //if (!ModelState.IsValid)
		    //{
			   // return BadRequest(ModelState);
			    
		    //}

		    //var isUpdated = _restaurantService.Modify(id, dto);
		    //if (!isUpdated) return NotFound(); Refactor z własą obsługą błędu
		    _restaurantService.Modify(id, dto);
		    return Ok();
	    }

	   
    }
}
