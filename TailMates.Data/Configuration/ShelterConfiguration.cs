using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
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
		}
	}
}
