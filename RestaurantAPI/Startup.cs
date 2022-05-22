using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using RestaurantAPI.Services;
using RestaurantAPI.Authorization;

namespace RestaurantAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var authenticationSettings = new AuthenticationSettings();
			//services.AddSingleton<>()  Dana zale�no�c utworzona tylko raz w ca�ym cyklu zycia aplikacji
			// services.AddScoped<>();		// Jeden obiekt przy ka�dym zapytaniu od clienta
					// AddTransient Nowe obiekty tworzone gdy odwolujemy sie do konstruktora
			Configuration.GetSection("Authentication").Bind(authenticationSettings);
			services.AddSingleton(authenticationSettings);
			services.AddAuthentication(option =>
			{
				option.DefaultAuthenticateScheme = "Bearer";
				option.DefaultScheme = "Bearer";
				option.DefaultChallengeScheme = "Bearer";
			}).AddJwtBearer(cfg =>
			{
				cfg.RequireHttpsMetadata = false;
				cfg.SaveToken = true;
				cfg.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidIssuer = authenticationSettings.JwtIssuer,
					ValidAudience = authenticationSettings.JwtIssuer,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
				};
			});
			services.AddAuthorization(options =>
			{
				options.AddPolicy("HasNationality",builder => builder.RequireClaim("Nationality"));
				options.AddPolicy("Atleast20",builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
				options.AddPolicy("CreatedAtLeast2Restaurants", builder => builder.AddRequirements(new MultipleRestaurantsRequirement(2)));
			});
			services.AddScoped<IAuthorizationHandler, MultipleRestaurantsRequirementHandler>();
			services.AddScoped<IAuthorizationHandler, MinimumAgeHandler>();
			services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
			services.AddControllers().AddFluentValidation();
			services.AddDbContext<RestaurantDBContext>();
			services.AddScoped<RestaurantSeeder>();
			services.AddAutoMapper(this.GetType().Assembly); // w Kontrolerze nie powinno byc problemu ze wstrzykiwaniem zaleznosci do IMappera
			services.AddScoped<IRestaurantService,RestaurantService>();
			services.AddScoped<IDishService, DishService>();
			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<ErrorHandlingMiddleware>();
			services.AddScoped<RequestTimeMiddleware>();
			services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
			services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
			services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
			services.AddScoped<IUserContextService, UserContextService>();
			services.AddHttpContextAccessor(); //Umo�liwia wstrzynkecie accessora do serwisu
			services.AddSwaggerGen();
			services.AddCors(options =>
			{
				options.AddPolicy("FrontEndClient", builder =>

					builder.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowAnyOrigin() //.WithOrigins("http://localhost:42-00")
					//.WithOrigins(Configuration["AllowedOrigins"])
					);
			});


		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env,RestaurantSeeder seeder)
		{
			app.UseResponseCaching();
			app.UseStaticFiles();
			app.UseCors("FrontEndClient");
			seeder.Seed();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMiddleware<ErrorHandlingMiddleware>(); // Wywo�anie musi by� przed HttpRedirection
			app.UseMiddleware<RequestTimeMiddleware>();
			app.UseAuthentication();
			app.UseHttpsRedirection();
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
			});

			app.UseRouting();
			app.UseAuthorization();
			
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
