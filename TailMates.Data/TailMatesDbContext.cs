namespace TailMates.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
	using System.Reflection;
	using TailMates.Data.Models;

	public class TailMatesDbContext : IdentityDbContext
    {
        public TailMatesDbContext(DbContextOptions<TailMatesDbContext> options)
            : base(options)
        {

        }

		public virtual DbSet<Pet> Pets { get; set; } = null!;
		public virtual DbSet<Breed> Breeds { get; set; } = null!;
		public virtual DbSet<Shelter> Shelters { get; set; } = null!;
		public virtual DbSet<Species> Species { get; set; } = null!;
		public virtual DbSet<AdoptionApplication> AdoptionApplications { get; set; } = null!;
		public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		}
	}
}
