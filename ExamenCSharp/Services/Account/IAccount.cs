using ExamenCSharp.Models;
using ExamenCSharp.Roles;

namespace ExamenCSharp.Services
{
    public interface IAccount
    {
        public Task<User?> GetAccountAsync(string email = "", string id = "");
        public Task<List<Role>?> GetRolesAsync(User usr);
        public Task<bool> CreateAccountAsync(User usr, string password);
        public Task<bool> ChangeRoleToUserAsync(User usr, Role role);
        public Task<bool> UpdateAccountAsync(User accountToChange, User newData);
        public Task<bool> DeleteAccountAsync(User usr);

        public Role? GetRoleAsync(string role);

    }
}