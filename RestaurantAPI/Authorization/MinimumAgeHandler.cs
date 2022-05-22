using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace RestaurantAPI.Authorization
{
	public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
	{
		private readonly ILogger<MinimumAgeHandler> _logger;

		public MinimumAgeHandler(ILogger<MinimumAgeHandler> logger)
		{
			_logger = logger;
		}
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
		{
			var dateOfBirth = DateTime.Parse(context.User.FindFirst( c=> c.Type == "DateOfBirth").Value);
			var userEmail = context.User.FindFirst(c =>c.Type == ClaimTypes.Name).Value;
			_logger.LogInformation($"User: {userEmail} with date of birth: [{dateOfBirth}]");
			if (dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Today)
			{
				_logger.LogInformation("Authorization succeeded");
				context.Succeed(requirement);
			}
			else
			{
				_logger.LogInformation("Authorization failed");
			}

			return Task.CompletedTask;
		}
	}
}
