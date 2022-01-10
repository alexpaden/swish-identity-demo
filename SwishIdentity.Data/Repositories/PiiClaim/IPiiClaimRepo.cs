using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SwishIdentity.Data.Models;

namespace SwishIdentity.Data.Repositories.PiiClaim
{
    public interface IPiiClaimRepo
    {
        Task<bool> Post(ProfileModel user, string mangId);
        
        Task<bool> Update(SwishUser user, string mangId);

        Task<IList<ManagerModel>> GetAllManagersByProfileId(Guid profId);
        
        Task<IList<ProfileModel>> GetAllProfilesByManagerId(Guid mangId);

        Task<bool> DoesClaimExist(string profileId, string mangId);

    }
}