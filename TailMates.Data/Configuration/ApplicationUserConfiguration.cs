using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using static TailMates.GCommon.ValidationConstants.ApplicationUser;

namespace TailMates.Data.Configuration
{
	public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> entity)
		{
			entity
				.Property(e => e.FirstName)
				.HasMaxLength(ApplicationUserFirstNameMaxLength);

			entity
				.Property(e => e.LastName)
				.HasMaxLength(ApplicationUserLastNameMaxLength);

			entity
				.HasMany(u => u.UserAdoptionApplications)
				.WithOne(a => a.ApplicationUser)
				.HasForeignKey(a => a.ApplicantId)
				.OnDelete(DeleteBehavior.NoAction);
            
			entity
			.HasData(this.CreateDefaultAdminUser());
		}
		
		private ApplicationUser CreateDefaultAdminUser()
		{
			ApplicationUser defaultUser = new ApplicationUser
			{
				Id = "f1a36e3c-bfa8-4a53-ab16-916d395ca40b",
				UserName = "admin@tailmates.com",
				NormalizedUserName = "ADMIN@TAILMATES.COM",
				Email = "admin@tailmates.com",
				FirstName = "Admin",
				LastName = "Administrator",
				NormalizedEmail = "ADMIN@TAILMATES.COM",
				EmailConfirmed = true,
				PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(
					new ApplicationUser { UserName = "admin@tailmates.com" },
					"Admin123!")
			};
			return defaultUser;
		}

	}
}
