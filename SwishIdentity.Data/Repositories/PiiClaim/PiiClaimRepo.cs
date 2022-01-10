using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SwishIdentity.Data.Models;
using SwishIdentity.Data.Repositories.Manager;
using SwishIdentity.Tools.DependencyService;

namespace SwishIdentity.Data.Repositories.PiiClaim
{
    [Service]
    public class PiiClaimRepo : IPiiClaimRepo, IQueryable
    {
        private readonly SwishDbContext _context;
        private readonly IManagerRepo _mang;

        public PiiClaimRepo(SwishDbContext context, IManagerRepo mang)
        {
            _context = context;
            _mang = mang;
        }

        public async Task<bool> Post(ProfileModel profile, string mangId)
        {
            var manager = await _mang.GetManagerByManagerId(mangId);
            var claim = new PiiClaimModel
            {
                ProfileId = profile.ProfileId,
                ManagerId = manager.ManagerId,
                DateTime = DateTime.Now
            };
            await _context.PiiClaims.AddAsync(claim);
            var result = await Save();
            return result != 0;
        }

        public async Task<bool> DoesClaimExist(string profId, string mangId)
        {
            var guidProfId = Guid.Parse(profId);
            var guidMangId = Guid.Parse(mangId);
            
            var result = _context.PiiClaims
                .Where(o => o.ManagerId == guidMangId)
                .FirstOrDefault(o => o.ProfileId == guidProfId);
            
            return result != default;
        }

        public Task<bool> Update(SwishUser user, string mangId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<ManagerModel>> GetAllManagersByProfileId(Guid profId)
        {
            var result = _context.PiiClaims
                .Where(o => o.ProfileId == profId)
                .Select(p => p.ManagerId)
                .ToList();

            var mangList = new List<ManagerModel>();
            foreach (var x in result)
            {
                var temp = _context.Managers
                    .FirstOrDefault(o => o.ManagerId == x);
                mangList.Add(temp);
            }

            return mangList;
        }

        public async Task<IList<ProfileModel>> GetAllProfilesByManagerId(Guid mangId)
        {
            var result = _context.PiiClaims
                .Where(o => o.ManagerId == mangId)
                .Select(p => p.ProfileId)
                .ToList();

            var profileList = new List<ProfileModel>();
            foreach (var x in result)
            {
                var temp = _context.Profiles
                    .FirstOrDefault(o => o.ProfileId == x);
                profileList.Add(temp);
            }

            return profileList;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Type ElementType { get; }
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        private async Task<int> Save()
        {
            var result = await _context.SaveChangesAsync();
            return result;
        }
    }
}