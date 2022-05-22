using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Entities
{
	public class RestaurantDBContext : DbContext
    {
	    private string _connectionString ="Server=DESKTOP-9AH5PNU\\SQLEXPRESS;Database=RestaurantDB;Trusted_Connection=True;MultipleActiveResultSets=true";

		public DbSet<Restaurant> Restaurants { get; set; }
	    public DbSet<Address> Addresses { get; set; }

	    public DbSet<Dish> Dishes { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<User> Users { get; set; }

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
		    modelBuilder.Entity<User>()
			    .Property(u => u.Email)
			    .IsRequired();
		    modelBuilder.Entity<Role>()
			    .Property(u => u.Name)
			    .IsRequired();




	    }

	    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	    {
		    optionsBuilder.UseSqlServer(_connectionString);
	    }
    }
}
