using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Services.Core.Interfaces
{
   public interface IPetService
    {
        Task<IEnumerable<PetViewModel>> GetAllPetsAsync();
	}
}
