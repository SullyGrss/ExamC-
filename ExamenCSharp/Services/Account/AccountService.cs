
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ExamenCSharp.Models;
using ExamenCSharp.Roles;
using ExamenCSharp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ExamenCSharp.Services
{
    public class AccountService : IAccount, IJwt
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        private readonly SignInManager<User> _signInManager;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, IServiceProvider serviceProvider, IConfiguration configuration, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        public async Task<List<Role>?> GetRolesAsync(User usr)
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            var user = await _userManager.FindByIdAsync(usr.Id);
            if(user == null) return null;

            var currentRoles = await _userManager.GetRolesAsync(user);
            List<Role> rolesToReturn = new();
            foreach(var role in currentRoles){
                Role r = (Role)Enum.Parse(typeof(Role), role);
                rolesToReturn.Add(r);
            }
            return rolesToReturn;
        }
        public async Task<User?> GetAccountAsync(string email = "", string id = "")
        {
            if(string.IsNullOrEmpty(email) && string.IsNullOrEmpty(id)) return null;
            
            User? user = null;

            if(!string.IsNullOrEmpty(email)) user = await _userManager.FindByEmailAsync(email);
            if(!string.IsNullOrEmpty(id)) user = await _userManager.FindByIdAsync(id);
            
            return user == null ? null : user;
        }
        public async Task<bool> ChangeRoleToUserAsync(User usr, Role role)
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var user = await _userManager.FindByIdAsync(usr.Id);
            if(user == null) return false;

            var roleExist = await roleManager.RoleExistsAsync(role.ToString());
            if(!roleExist) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            foreach (var currentRole in currentRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, currentRole);
            }

            var result = await _userManager.AddToRoleAsync(user, role.ToString());
            return result.Succeeded ? true : false;
        }

        public async Task<bool> CreateAccountAsync(User usr, string password)
        {
            usr.Nom = usr.Nom.ToUpper();
            var result = await _userManager.CreateAsync(usr, password);
            if(!result.Succeeded) return false;

            await ChangeRoleToUserAsync(usr, Role.Client);
            return true;
        }
        public async Task<bool> UpdateAccountAsync(User accountToChange, User newData)
        {
            //A faire plus tard
            var user = await _userManager.FindByIdAsync(accountToChange.Id);
            if(user == null) return false;


            return true;
        }
        public async Task<bool> DeleteAccountAsync(User usr){
            var user = await _userManager.FindByIdAsync(usr.Id);
            if(user == null) return false;

            var result = await _userManager.DeleteAsync(usr);
            return result.Succeeded ? true : false;
        }

        public Role? GetRoleAsync(string role){

            try
            {
                return (Role?)Enum.Parse(typeof(Role), role);
            }
            catch(Exception)
            {
                return null;
            }
        }
        
    #pragma warning disable CS8600, CS8604
        public async Task<string> GenerateToken(User user, string password){

            if(await GetAccountAsync(user.Email) == null) throw new Exception("Utilisateur non existant");

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if(!result.Succeeded) throw new Exception("Email ou mot de passe incorrect !"); 

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    #pragma warning restore CS8600, CS8604
    }
}