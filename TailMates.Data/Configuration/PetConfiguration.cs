using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using static TailMates.GCommon.ValidationConstants.Pet;
using TailMates.Data.Models.Enums;

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

			entity
				.HasData(this.GenerateSeedPets());
		}

		private List<Pet> GenerateSeedPets()
		{
			List<Pet> seedPets = new List<Pet>()
			{

				new Pet
				{
					Id = 1,
					Name = "Rexy",
					Age = 3,
					Description = "A friendly and energetic dog looking for a loving home.",
					ImageUrl = "https://th.bing.com/th/id/OIP.3J2q-ML2eSU3xPhgV4ez0AHaE8?r=0&rs=1&pid=ImgDetMain",
					Gender = PetGender.Male,
					IsAdopted = false,
					DateListed = DateTime.UtcNow,
					IsDeleted = false,
					ShelterId = 1,
					SpeciesId = 1,
					BreedId = 1

				},
				new Pet
				{
					Id = 2,
					Name = "Whiskers",
					Age = 2,
					Description = "A curious and playful cat who loves cuddles.",
					ImageUrl = "https://th.bing.com/th/id/OIP.cvg_MdgYsY9-fKD5eV8SpgHaE5?r=0&rs=1&pid=ImgDetMain",
                    Gender = PetGender.Female, 
                    IsAdopted = false,
					DateListed = new DateTime(2024, 1, 20),
					IsDeleted = false,
					ShelterId = 1,
					SpeciesId = 2,
                    BreedId = 2 

				},
				new Pet
				{
					Id = 3,
					Name = "Buddy",
					Age = 1,
					Description = "A small, fluffy dog with a big personality, great with kids.",
					ImageUrl = "https://www.thesprucepets.com/thmb/93mObWq1NDdH6dZkibYq0XO4ZaU=/2121x1414/filters:no_upscale():max_bytes(150000):strip_icc()/Pomeranianstandingonroad-7defae876b0f44589279e188c95666ea.jpg", 
                    Gender = PetGender.Male,
					IsAdopted = false,
					DateListed = new DateTime(2024, 2, 10),
					IsDeleted = false,
					ShelterId = 2, 
                    SpeciesId = 1,
					BreedId = 3 

				},
				new Pet
				{
					Id = 4,
					Name = "Shadow",
					Age = 4,
					Description = "A shy but loving black cat, needs a quiet home.",
					ImageUrl = "https://th.bing.com/th/id/R.1480d0e8449c4676cb50b3d691f89b39?rik=sDKV6DJKqVKHUw&pid=ImgRaw&r=0",
					Gender = PetGender.Female,
					IsAdopted = false,
					DateListed = new DateTime(2024, 3, 1),
					IsDeleted = false,
					IsApproved = false, 
                    ShelterId = 1,
					SpeciesId = 2,
					BreedId = 4 
                }
			};

			return seedPets;
		}
	}
}
