using System.Collections.Generic;
using System.Threading.Tasks;
using SwishIdentity.Data.Models;

namespace SwishIdentity.Data.Repositories.Profile
{
    public interface IProfileRepo 
    {
        Task<ProfileModel> GetProfileByUserId(string id);

        Task<ProfileModel> GetProfileByProfileId(string profId);

        
        Task<IList<ProfileModel>> GetAllProfiles();
        
        Task<IList<ProfileModel>> GetPendingProfiles();
        
        Task<bool> Post(ProfileModel profile);
        
        void Update(ProfileModel profile);
    }
}