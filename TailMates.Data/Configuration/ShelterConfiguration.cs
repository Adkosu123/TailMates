using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using static TailMates.GCommon.ValidationConstants.Shelter; 

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
					IsDeleted = false,

				},
				new Shelter
				{
					  Id = 2,
					Name = "Sunshine Sanctuary",
					Address = "456 Sunny Blvd, Green Valley",
					PhoneNumber = "555-333-4444",
					Email = "contact@sunshinesanctuary.org",
					IsDeleted = false,

				},
			};

			return seedShelters;
		}
	}
}
