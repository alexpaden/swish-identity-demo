using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SwishIdentity.Data.Models;
using SwishIdentity.Tools.DependencyService;

namespace SwishIdentity.Data.Repositories.Profile
{
    [Service]
    public class ProfileRepo : IProfileRepo
    {
        private readonly SwishDbContext _context;
        private UserManager<SwishUser> _usr;

        public ProfileRepo(SwishDbContext context, UserManager<SwishUser> usr)
        {
            _context = context;
            _usr = usr;
        }

        public async Task<ProfileModel> GetProfileByUserId(string id)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(
                x => x.UserId == id);
            return profile;
        }

        public async Task<ProfileModel> GetProfileByProfileId(string profId)
        {
            var guidProfId = Guid.Parse(profId);
            var profile = await _context.Profiles.FirstOrDefaultAsync(
                x => x.ProfileId == guidProfId);
            return profile;
        }

        public Task<IList<ProfileModel>> GetPendingProfiles()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<ProfileModel>> GetAllProfiles()
        {
            var profiles = await _context.Profiles.ToListAsync();
            return profiles;
        }

        public async Task<bool> Post(ProfileModel profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            await _context.Profiles.AddAsync(profile);
            var result = await Save();
            return result >= 1;
        }

        public async void Update(ProfileModel profile)
        {
            _context.Entry(profile).State = EntityState.Modified;
            await Save();
        }

        private async Task<int> Save()
        {
            var result = await _context.SaveChangesAsync();
            return result;
        }
    }
}