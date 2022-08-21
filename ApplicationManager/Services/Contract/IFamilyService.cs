using ApplicationManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationManager.Services.Contract
{
    public interface IFamilyService
    {
        Task<IEnumerable<Family>> GetFamilyDataAsync(string query);
        Task AddFamilyDataAsync(Family family);
        Task DeleteFamilyDataAsync(string id);
        Task UpdateFamilyDataAsync(Family family);
    }
}
