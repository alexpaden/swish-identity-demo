using System.Threading.Tasks;
using SwishIdentity.Data.Models;

namespace SwishIdentity.Data.Repositories.ProfileHistory
{
    public interface IProfHistoryRepo
    {
        Task<string> ProfileToJson(ProfileModel profile);

        Task<bool> Update(ProfileModel profile);

        Task<string> GetHistoryJsonByProfile(ProfileModel profile);
    }
}