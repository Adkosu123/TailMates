using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TailMates.Data.Models;
using static TailMates.Data.Common.ValidationConstants.ApplicationUser;

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
				.HasOne(u => u.ManagedShelter)
				.WithMany()
				.HasForeignKey(u => u.ManagedShelterId)
				.IsRequired(false)
				.OnDelete(DeleteBehavior.NoAction);
            
		}
	}
}
