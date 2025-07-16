using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using static TailMates.Data.Common.ValidationConstants.Shelter; 

namespace TailMates.Data.Configuration
{
	public class ShelterConfiguration : IEntityTypeConfiguration<Shelter>
	{
		public void Configure(EntityTypeBuilder<Shelter> entity)
		{
			entity
				.Property(s => s.Name)
				.IsRequired()
				.HasMaxLength(ShelterNameMaxLength);

			entity
				.Property(s => s.Address)
				.IsRequired()
				.HasMaxLength(ShelterAddressMaxLength);

			entity
				.Property(s => s.PhoneNumber)
				.HasMaxLength(ShelterPhoneNumberMaxLength);

			entity
				.Property(s => s.Email)
				.HasMaxLength(ShelterEmailMaxLength);

			entity
				.Property(s => s.ImageUrl)
				.HasMaxLength(512);

			entity
				.Property(s => s.Description)
				.HasMaxLength(200);

			entity
				.Property(s => s.IsDeleted)
				.HasDefaultValue(false);

			entity
				.HasQueryFilter(s => s.IsDeleted == false);

			entity.HasMany(s => s.Pets)
				.WithOne(p => p.Shelter)
				.HasForeignKey(p => p.ShelterId)
				.OnDelete(DeleteBehavior.NoAction);

			entity
				.HasData(this.GenerateSeedShelters());
		}

		private List<Shelter> GenerateSeedShelters()
		{
			List<Shelter> seedShelters = new List<Shelter>()
			{

				new Shelter
				{
					Id = 1,
					Name = "Happy Paws Shelter",
					Address = "123 Pet Lane, Animal City",
					PhoneNumber = "555-111-2222",
					Email = "info@happypaws.com",
					Description = "A loving shelter for pets in need, providing food, care, and adoption services.",
					ImageUrl = "https://i.pinimg.com/originals/97/19/9c/97199cdda2fec20471eb88c8da150220.jpg",
					IsDeleted = false,

				},
				new Shelter
				{
					Id = 2,
					Name = "Sunshine Sanctuary",
					Address = "456 Sunny Blvd, Green Valley",
					PhoneNumber = "555-333-4444",
					Email = "contact@sunshinesanctuary.org",
					Description = "A safe haven for abandoned and stray animals, providing shelter and care.",
					ImageUrl = "https://mnpower.com/Content/Images/Company/MPJournal/2017/12202017_01.jpg",
					IsDeleted = false,

				},
			};

			return seedShelters;
		}
	}
}
