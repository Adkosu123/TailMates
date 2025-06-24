using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using static TailMates.GCommon.ValidationConstants.Breed;


namespace TailMates.Data.Configuration
{
	public class BreedConfiguration : IEntityTypeConfiguration<Breed>
	{
		public void Configure(EntityTypeBuilder<Breed> entity)
		{
			entity
				.Property(b => b.Name)
				.IsRequired()
				.HasMaxLength(BreedNameMaxLength);

			entity
				.HasIndex(b => new { b.Name, b.SpeciesId })
				.IsUnique();

			entity
				.HasMany(b => b.Pets)
				.WithOne(p => p.Breed)
				.HasForeignKey(p => p.BreedId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
