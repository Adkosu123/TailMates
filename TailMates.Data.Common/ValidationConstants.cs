using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Data.Common
{
   public static class ValidationConstants
    {
        public static class Pet 
        {
            public const int PetNameMaxLength = 30;
            public const int PetNameMinLength = 2;
            public const int PetDescriptionMaxLength = 500;
            public const int PetDescriptionMinLength = 50;
            public const int PetAgeMinValue = 1;
            public const int PetAgeMaxValue = 30;
		}

        public static class Species
		{
			public const int SpeciesNameMaxLength = 50;
			public const int SpeciesNameMinLength = 2;
		}

        public static class Breed 
		{
            public const int BreedNameMaxLength = 60;
		}

		public static class  Shelter
		{
            public const int ShelterNameMaxLength = 80;
            public const int ShelterNameMinLength = 5;
            public const int ShelterAddressMaxLength = 40;
            public const int ShelterAddressMinLength = 3;
            public const int ShelterPhoneNumberMaxLength = 15;
            public const int ShelterEmailMaxLength = 70;
		}

        public static class AdoptionApplication 
        {
            public const int ApplicantNotesMaxLength = 1000;
            public const int AdminNotesMaxLength = 1000;
        }

        public static class ApplicationUser 
        {
            public const int ApplicationUserFirstNameMaxLength = 50;
            public const int ApplicationUserLastNameMaxLength = 50;
		}
	}
}
