using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SwishIdentity.Tools.DependencyService;

namespace SwishIdentity.Data.VerificationRoleManager
{
    public interface IVerificationRoleManager
    {
        private Task<bool> RoleExistOrCreate(string roleName)
        {
            throw new NotImplementedException();
        }

        Task<bool> MakeUserOnlyThisRole(SwishUser user, string roleName);
        Task<bool> MakeUserManager(SwishUser user);
        Task<string> RolesToStringByUser(SwishUser user);
    }

    [Service]
    public class VerificationRoleManager : IVerificationRoleManager
    {
        private const string AlienRole = "ALIEN";
        private const string PendingRole = "PENDING";
        private const string UnverifiedRole = "UNVERIFIED";
        private const string VerifiedRole = "VERIFIED";
        private const string ManagerRole = "MANAGER";
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<SwishUser> _signInManager;
        private readonly UserManager<SwishUser> _userManager;

        public VerificationRoleManager(
            UserManager<SwishUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<SwishUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<bool> MakeUserOnlyThisRole(SwishUser user, string roleName)
        {
            var result = await RoleExistOrCreate(roleName);
            if (!result) return false;
            try
            {
                if (await _userManager.IsInRoleAsync(user, AlienRole))
                    await _userManager.RemoveFromRoleAsync(user, AlienRole);

                if (await _userManager.IsInRoleAsync(user, PendingRole))
                    await _userManager.RemoveFromRoleAsync(user, PendingRole);

                if (await _userManager.IsInRoleAsync(user, UnverifiedRole))
                    await _userManager.RemoveFromRoleAsync(user, UnverifiedRole);

                if (await _userManager.IsInRoleAsync(user, VerifiedRole))
                    await _userManager.RemoveFromRoleAsync(user, VerifiedRole);

                await _userManager.AddToRoleAsync(user, roleName);
                await _signInManager.RefreshSignInAsync(user);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> MakeUserManager(SwishUser user)
        {
            var result = await RoleExistOrCreate(ManagerRole);
            if (!result) return false;
            try
            {
                await _userManager.AddToRoleAsync(user, ManagerRole);
                await _signInManager.RefreshSignInAsync(user);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<string> RolesToStringByUser(SwishUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleString = string.Join(",", roles);
            return roleString;
        }

        private async Task<bool> RoleExistOrCreate(string roleName)
        {
            roleName = roleName.ToUpper();
            var existence = await _roleManager.RoleExistsAsync(roleName);
            if (existence) return true;
            switch (roleName)
            {
                case AlienRole:
                    roleName = AlienRole;
                    break;
                case PendingRole:
                    roleName = PendingRole;
                    break;
                case UnverifiedRole:
                    roleName = UnverifiedRole;
                    break;
                case VerifiedRole:
                    roleName = VerifiedRole;
                    break;
                case ManagerRole:
                    roleName = ManagerRole;
                    break;
                default:
                    return false;
            }

            var newRole = new IdentityRole {Name = roleName};
            var result = await _roleManager.CreateAsync(newRole);
            if (result.Succeeded) return true;
            return false;
        }
    }
}