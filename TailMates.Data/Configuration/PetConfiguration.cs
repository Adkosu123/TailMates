using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using static TailMates.GCommon.ValidationConstants.Pet;

namespace TailMates.Data.Configuration
{
	public class PetConfiguration : IEntityTypeConfiguration<Pet>
	{
		public void Configure(EntityTypeBuilder<Pet> entity)
		{
			entity
				.Property(p => p.Name)
				.IsRequired()
				.HasMaxLength(PetNameMaxLength);

			entity
				.Property(p => p.Age)
				.IsRequired();

			entity
				.Property(p => p.Description)
				.IsRequired()
				.HasMaxLength(PetDescriptionMaxLength);

			entity
				.Property(p => p.ImageUrl)
				.HasMaxLength(2048);

			entity
				.Property(p => p.Gender)
				.IsRequired()
				.HasConversion<string>();

			entity
				.Property(p => p.IsAdopted)
				.IsRequired();

			entity
				.Property(p => p.DateListed)
				.IsRequired();

			entity
				.Property(p => p.IsDeleted)
				.HasDefaultValue(false);

			entity
				.HasQueryFilter(p => p.IsDeleted == false);

			entity
				.HasMany(p => p.AdoptionApplications)
				.WithOne(a => a.Pet)
				.HasForeignKey(a => a.PetId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
