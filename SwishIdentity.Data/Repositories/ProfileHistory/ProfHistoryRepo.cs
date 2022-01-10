using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SwishIdentity.Data.Models;
using SwishIdentity.Tools.DependencyService;

namespace SwishIdentity.Data.Repositories.ProfileHistory
{
    [Service]
    public class ProfHistoryRepo : IProfHistoryRepo
    {
        private readonly SwishDbContext _context;
        private readonly UserManager<SwishUser> _usr;

        public ProfHistoryRepo(SwishDbContext context, UserManager<SwishUser> usr)
        {
            _context = context;
            _usr = usr;
        }

        public async Task<bool> Update(ProfileModel profile)
        {
            var profHistory = await _context.ProfileHistories.FirstOrDefaultAsync(
                o => o.Profile == profile);
            if (profHistory != default)
            {
                var history = profHistory.ProfileHistory;
                var newHistory = await ProfileToJson(profile);
                var concatHistory = string.Concat(history, ", ", newHistory);
                profHistory.ProfileHistory = concatHistory;
                _context.Entry(profHistory).State = EntityState.Modified;
                await Save();
                return true;
            }

            if (await Post(profile)) return true;
            ;
            return false;
        }

        public async Task<string> GetHistoryJsonByProfile(ProfileModel profile)
        {
            var profHistory = await _context.ProfileHistories.FirstOrDefaultAsync(
                o => o.Profile == profile);
            return profHistory.ProfileHistory;
        }

        public async Task<string> ProfileToJson(ProfileModel profile)
        {
            var roles = await _usr.GetRolesAsync(profile.User);
            var combindedRoles = string.Join(",", roles);

            var jsonObject = new JObject
            {
                {"ProfileId", profile.ProfileId},
                {"FullName", profile.Name},
                {"Country", profile.Country},
                {"ImgFrontUrl", profile.ImgFrontUrl},
                {"ImgRearUrl", profile.ImgRearUrl},
                {"Datetime", DateTime.Now},
                {"Roles", combindedRoles},
                {"SharedEmail", profile.SharedEmail}
            };
            Console.WriteLine(jsonObject);
            return jsonObject.ToString();
        }

        private async Task<bool> Post(ProfileModel profile)
        {
            try
            {
                var jsonHistory = await ProfileToJson(profile);
                var profHistory = new ProfileHistoryModel();
                profHistory.Profile = profile;
                profHistory.ProfileHistory = jsonHistory;
                await _context.ProfileHistories.AddAsync(profHistory);
                await Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<int> Save()
        {
            var result = await _context.SaveChangesAsync();
            return result;
        }
    }
}