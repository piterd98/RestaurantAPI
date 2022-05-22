using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Models.Validators
{
    public class RestaurantQueryValidator: AbstractValidator<RestaurantQuery>
    {
	    public int[] allowedPageSizes = new[] {5, 10, 25};

	    public string[] allowedSortByColumnNames =
		    {nameof(Restaurant.Name), nameof(Restaurant.Description), nameof(Restaurant.Category)};
	    public RestaurantQueryValidator()
	    {
		    RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
		    RuleFor(r => r.PageRange).Custom((value, context) =>
		    {
			    if (!allowedPageSizes.Contains(value))
			    {
				    context.AddFailure("PageRange",$"PageRange must in [{string.Join(",",allowedPageSizes)}]");
			    }
		    });
		    RuleFor(r => r.SortBy)
			    .Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
			    .WithMessage($"Sort by is optional or must be in [{string.Join(",", allowedSortByColumnNames)}]");
	    }  
    }
}
