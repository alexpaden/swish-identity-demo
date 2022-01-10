using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SwishIdentity.Data.Models;
using SwishIdentity.Tools.DependencyService;

namespace SwishIdentity.Data.Repositories.Manager
{
    [Service]
    public class ManagerRepo : IManagerRepo
    {
        private readonly SwishDbContext _context;
        private UserManager<SwishUser> _usr;

        public ManagerRepo(SwishDbContext context, UserManager<SwishUser> usr)
        {
            _context = context;
            _usr = usr;
        }

        public async Task<ManagerModel> GetManagerByUserId(string userId)
        {
            var manager = await _context.Managers.FirstOrDefaultAsync(
                x => x.UserId == userId);
            return manager;
        }

        public async Task<ManagerModel> GetManagerByManagerId(string mangId)
        {
            var guidMangId = Guid.Parse(mangId);
            var manager = await _context.Managers.FirstOrDefaultAsync(o =>
                o.ManagerId == guidMangId);
            return manager;
        }

        public async Task<IList<ManagerModel>> GetAllPendingManagers()
        {
            IList<ManagerModel> managers = await _context.Managers
                .Where(p => p.ManagerEnabled == false)
                .Select(p => p)
                .ToListAsync();
            return managers;
        }

        public async Task<bool> Post(ManagerModel manager)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            await _context.Managers.AddAsync(manager);
            var result = await Save();
            return result >= 1;
        }

        public async Task<bool> Update(ManagerModel manager)
        {
            _context.Entry(manager).State = EntityState.Modified;
            var result = await Save();
            return result != 0;
        }

        private async Task<int> Save()
        {
            var result = await _context.SaveChangesAsync();
            return result;
        }
    }
}