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

			entity
				.HasData(this.GetSeedBreedData());
		}

		private IEnumerable<Breed> GetSeedBreedData()
		{
			return new List<Breed>()
			{
				new Breed { Id = 1, Name = "Labrador Retriever", SpeciesId = 1 }, // Dog
                new Breed { Id = 2, Name = "Siamese", SpeciesId = 2 },            // Cat
                new Breed { Id = 3, Name = "Poodle", SpeciesId = 1 },             // Dog
                new Breed { Id = 4, Name = "Maine Coon", SpeciesId = 2 },         // Cat
                new Breed { Id = 5, Name = "Parrot", SpeciesId = 3 },             // Bird
                new Breed { Id = 6, Name = "Dutch", SpeciesId = 4 }               // Rabbit
            };
		}
	}
}
