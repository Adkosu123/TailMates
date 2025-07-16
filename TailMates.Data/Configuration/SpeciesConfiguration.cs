using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using static TailMates.Data.Common.ValidationConstants.Species;

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

			entity
				.HasData(this.GetSeedSpeciesData());
		}

		private IEnumerable<Species> GetSeedSpeciesData()
		{
			return new List<Species>()
			{
				new Species { Id = 1, Name = "Dog" },
				new Species { Id = 2, Name = "Cat" },
				new Species { Id = 3, Name = "Bird" },
				new Species { Id = 4, Name = "Rabbit" }
			};
		}
	}
}
