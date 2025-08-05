using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TailMates.Data.Models;
using static TailMates.Data.Common.ValidationConstants.AdoptionApplication;

namespace TailMates.Data.Configuration
{
	public class AdoptionApplicationConfiguration : IEntityTypeConfiguration<AdoptionApplication>
	{
		public void Configure(EntityTypeBuilder<AdoptionApplication> entity)
		{
			entity
				.Property(a => a.ApplicationDate)
				.IsRequired();

			entity
				.Property(a => a.Status)
				.IsRequired()
				.HasConversion<string>();

			entity
				.Property(a => a.ApplicantNotes)
				.HasMaxLength(ApplicantNotesMaxLength);

			entity
				.Property(a => a.AdminNotes)
				.HasMaxLength(AdminNotesMaxLength);

			entity
				.HasQueryFilter(a => a.Pet.IsDeleted == false);

			entity
				.Property(a => a.ApplicantId)
				.IsRequired()
				.HasMaxLength(450);

			entity
				.HasOne(a => a.ApplicationUser)
				.WithMany()
				.HasForeignKey(a => a.ApplicantId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
