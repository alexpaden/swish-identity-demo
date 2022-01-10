using System.Collections.Generic;
using System.Threading.Tasks;
using SwishIdentity.Data.Models;

namespace SwishIdentity.Data.Repositories.Manager
{
    public interface IManagerRepo 
    {
        Task<ManagerModel> GetManagerByUserId(string userId);
        
        Task<ManagerModel> GetManagerByManagerId(string mangId);

        Task<bool> Post(ManagerModel manager);
        
        Task<bool> Update(ManagerModel manager);

        Task<IList<ManagerModel>> GetAllPendingManagers();
    }
}