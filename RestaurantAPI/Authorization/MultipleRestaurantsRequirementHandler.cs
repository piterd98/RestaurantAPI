using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Authorization
{
    public class MultipleRestaurantsRequirementHandler: AuthorizationHandler<MultipleRestaurantsRequirement>
    {
		private readonly RestaurantDBContext _context;

		public MultipleRestaurantsRequirementHandler(RestaurantDBContext context)
	    {
		    _context = context;
	    }
	    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MultipleRestaurantsRequirement requirement)
	    {
		    var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

		    var createdRestaurantsCount = _context
			    .Restaurants
			    .Count(r => r.CreatedById == userId);
		    if (createdRestaurantsCount >= requirement.MinimumRestaurantsCreated)
		    {
				context.Succeed(requirement);
		    }

		    return Task.CompletedTask;

	    }
    }
}
