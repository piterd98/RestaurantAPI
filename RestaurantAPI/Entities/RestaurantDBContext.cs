using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace RestaurantAPI.Entities
{
    public class RestaurantDBContext : DbContext
    {
	    private string _connectionString =
		    "Server=DESKTOP-9AH5PNU\\SQLEXPRESS;Database=RestaurantDB;Trusted_Connection=True;MultipleActiveResultSets=true";

		public DbSet<Restaurant> Restaurants { get; set; }
	    public DbSet<Address> Addresses { get; set; }

	    public DbSet<Dish> Dishes { get; set; }

	    protected override void OnModelCreating(ModelBuilder modelBuilder)
	    {
		    modelBuilder.Entity<Restaurant>()
			    .Property(r => r.Name)
			    .IsRequired()
			    .HasMaxLength(25);

		    modelBuilder.Entity<Dish>()
			    .Property(d => d.Name)
			    .IsRequired();

		    modelBuilder.Entity<Address>()
			    .Property(a => a.Street)
			    .IsRequired()
			    .HasMaxLength(50);
		    modelBuilder.Entity<Address>()
			    .Property(aC => aC.City)
			    .IsRequired()
			    .HasMaxLength(50);




	    }

	    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	    {
		    optionsBuilder.UseSqlServer(_connectionString);
	    }
    }
}
