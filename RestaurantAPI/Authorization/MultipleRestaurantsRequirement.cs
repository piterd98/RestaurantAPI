using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class MultipleRestaurantsRequirement: IAuthorizationRequirement
    {
	    public int MinimumRestaurantsCreated { get;}

	    public MultipleRestaurantsRequirement(int minimumRestaurantsCreated)
	    {
		    MinimumRestaurantsCreated = minimumRestaurantsCreated;
	    }
    }
}
