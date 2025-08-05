using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TailMates.Data.Models;
using static TailMates.Data.Common.ValidationConstants.Breed;


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
				//Dog
				new Breed { Id = 1, Name = "Labrador Retriever", SpeciesId = 1 }, 
                new Breed { Id = 3, Name = "Poodle", SpeciesId = 1 },
                new Breed { Id = 7, Name = "German Shepherd", SpeciesId = 1 },
                new Breed { Id = 8, Name = "Golden Retriever", SpeciesId = 1 },

				
				//Cat
                new Breed { Id = 2, Name = "Siamese", SpeciesId = 2 },          
                new Breed { Id = 4, Name = "Maine Coon", SpeciesId = 2 },  
                new Breed { Id = 9, Name = "Persian", SpeciesId = 2 },  
                new Breed { Id = 10, Name = "Domestic Shorthair", SpeciesId = 2 },  
				

				//Bird
                new Breed { Id = 5, Name = "Parrot", SpeciesId = 3 },  
                new Breed { Id = 11, Name = "Parakeet", SpeciesId = 3 },  
                new Breed { Id = 12, Name = "Finch", SpeciesId = 3 },  
                new Breed { Id = 13, Name = "Canary", SpeciesId = 3 },  


				//Rabbit
                new Breed { Id = 6, Name = "Dutch", SpeciesId = 4 },             
                new Breed { Id = 14, Name = "Mini Lop", SpeciesId = 4 },               // Rabbit
                new Breed { Id = 15, Name = "Netherland Dwarf", SpeciesId = 4 },              // Rabbit
                new Breed { Id = 16, Name = "Flemish Giant", SpeciesId = 4 },   
				
				// Fish
				new Breed { Id = 17, Name = "Goldfish", SpeciesId = 5 },
		        new Breed { Id = 18, Name = "Betta Fish", SpeciesId = 5 },
		        new Breed { Id = 19, Name = "Guppy", SpeciesId = 5 },
		        new Breed { Id = 20, Name = "Angelfish", SpeciesId = 5 },

				//Hamster
				new Breed { Id = 21, Name = "Syrian Hamster", SpeciesId = 6 },
		        new Breed { Id = 22, Name = "Dwarf Hamster", SpeciesId = 6 },
		        new Breed { Id = 23, Name = "Roborovski Dwarf Hamster", SpeciesId = 6 },


				 // Snake 
                new Breed { Id = 24, Name = "Corn Snake", SpeciesId = 7 },
                new Breed { Id = 25, Name = "Ball Python", SpeciesId = 7 },
                new Breed { Id = 26, Name = "King Snake", SpeciesId = 7 },
		        
                // Turtle 
                new Breed { Id = 27, Name = "Red-Eared Slider", SpeciesId = 8 },
                new Breed { Id = 28, Name = "Box Turtle", SpeciesId = 8 },
                new Breed { Id = 29, Name = "Painted Turtle", SpeciesId = 8 }
			};
		}
	}
}
