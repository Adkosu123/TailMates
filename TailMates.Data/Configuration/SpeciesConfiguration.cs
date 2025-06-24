using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using static TailMates.GCommon.ValidationConstants.Species;

namespace TailMates.Data.Configuration
{
	public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
	{
		public void Configure(EntityTypeBuilder<Species> entity)
		{
			entity
				.Property(s => s.Name)
				.IsRequired()
				.HasMaxLength(SpeciesNameMaxLength);

			entity
				.HasIndex(s => s.Name)
				.IsUnique();

			entity
				.HasMany(s => s.Pets)
				.WithOne(p => p.Species)
				.HasForeignKey(p => p.SpeciesId)
				.OnDelete(DeleteBehavior.NoAction);

			entity.HasMany(s => s.Breeds)
				.WithOne(b => b.Species)
				.HasForeignKey(b => b.SpeciesId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
