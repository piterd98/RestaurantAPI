using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
	public interface IRestaurantService
	{
		RestaurantDto GetById(int id);
		PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
		int Create(CreateRestaurantDto dto);
		void Delete(int id);
		void Modify(int id, ModifyRestaurantDto dto);
	}

	public class RestaurantService : IRestaurantService
	{
		private readonly RestaurantDBContext _dbContext;
		private readonly IMapper _mapper;
		private readonly ILogger<RestaurantService> _logger;
		private readonly IAuthorizationService _autorizationService;
		private readonly IUserContextService _userContextService;

		public RestaurantService(RestaurantDBContext dbContext,IMapper mapper, ILogger<RestaurantService> logger,
			IAuthorizationService authorizationService,IUserContextService userContextService)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_logger = logger;
			_autorizationService = authorizationService;
			_userContextService = userContextService;
		}
	    public RestaurantDto GetById(int id)
	    {
		    var restaurant = _dbContext
			    .Restaurants
			    .Include(r => r.Address)
			    .Include(r => r.Dishes)
			    .FirstOrDefault(r => r.Id == id);
			if (restaurant is null) throw new NotFoundException("Restaurant not found");


			var result = _mapper.Map<RestaurantDto>(restaurant);
			return result;


	    }

	    public void Delete(int id)
	    {
			_logger.LogError($"Restaurant with id: {id} DELETE action invoked");
		    var restaurant = _dbContext
			    .Restaurants
			    .FirstOrDefault(r => r.Id == id);
			if (restaurant is null) throw new NotFoundException("Restaurant not found");
			var autorizationResult = _autorizationService.AuthorizeAsync(_userContextService.User, restaurant,
				new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
			if (!autorizationResult.Succeeded)
			{
				throw new ForbidException();
			}

			_dbContext.Restaurants.Remove(restaurant);
		    _dbContext.SaveChanges();

		    


	    }
		//IEnumerable ---> PagedResult
	    public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
	    {
		    var baseQuery = _dbContext
			    .Restaurants
			    .Include(r => r.Address)
			    .Include(r => r.Dishes)
			    .Where(r => query.SearchedPhrase == null ||
			                (r.Name.ToLower().Contains(query.SearchedPhrase.ToLower()) ||
			                 r.Description.ToLower().Contains(query.SearchedPhrase.ToLower())));
		    if (!string.IsNullOrEmpty(query.SortBy))
		    {
			    var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>()
			    {
				    {nameof(Restaurant.Name), r => r.Name},
				    {nameof(Restaurant.Description), r => r.Description},
				    {nameof(Restaurant.Category), r => r.Category},
				};
			    var selectedColumn = columnsSelector[query.SortBy];

			    baseQuery = query.SortDirection == SortDirection.ASC
				    ? baseQuery.OrderBy(r => r.Name)
				    : baseQuery.OrderByDescending(r => r.Category);
		    }
			//EF tworzy kod sql, ktory pobierze z bazy restauracje i zwroci w tej zmiennej
			var restaurants = baseQuery
			    .Skip(query.PageRange * (query.PageNumber-1))
			    .Take(query.PageRange)
			    .ToList();
			 var totalItemsCount =baseQuery.Count();
		    //var restaurantsDtos = restaurants.Select(r => new RestaurantDto()
		    //{
		    // Name = r.Name,
		    // Category = r.Category,
		    // City = r.Address.City
		    //}); Jeden sposob
		    var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants); // z jakiego typu w nawiasach ostrych na jaki chcemy zmapowac
		    var result =
			    new PagedResult<RestaurantDto>(restaurantsDtos, totalItemsCount, query.PageRange, query.PageNumber);
		    return result;

	    }

	    public int Create(CreateRestaurantDto dto)
	    {
		    var restaurant = _mapper.Map<Restaurant>(dto);
		    restaurant.CreatedById = _userContextService.GetUserId;
		    _dbContext.Restaurants.Add(restaurant);
		    _dbContext.SaveChanges();
		    return restaurant.Id;
	    }

	    //Bool dlatego by sprawdzac czy dany zasob zostal znaleziony na serwerze
	    public void Modify(int id, ModifyRestaurantDto dto)
	    {
		    
			var restaurant = _dbContext
				.Restaurants
				.FirstOrDefault(r => r.Id == id);
			if (restaurant is null) throw new NotFoundException("Restaurant not found");

			var autorizationResult = _autorizationService.AuthorizeAsync(_userContextService.User, restaurant,
				new ResourceOperationRequirement(ResourceOperation.Update)).Result;
			if (!autorizationResult.Succeeded)
			{
				throw new ForbidException();
			}

			
			restaurant.Name = dto.Name;
			restaurant.Description = dto.Description;
			restaurant.HasDelivery = dto.HasDelivery;
			_dbContext.SaveChanges();

			


	    }
	}
}
