using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ExamenCSharp.Models;
using ExamenCSharp.Roles;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using ExamenCSharp.Services;

public class DatabaseSeeder : IDatabaseSeeder {
    private readonly UserManager<User> _userManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly AccountService _accountService;

    private readonly ApplicationDbContext _db;

    public DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger, UserManager<User> userManager, AccountService accountService, ApplicationDbContext db)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
         _userManager = userManager;
         _accountService = accountService;
         _db = db;
    }


    public async Task SeedAsync()
    {
        await CreateRolesAsync();
        await CreateDefaultUsers();
        await CreateDefaultServices();
    }

    private async Task CreateDefaultServices()
    {
        var serviceJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Services", "DatabaseSeeder", "default_services.json"));
        var services = JsonSerializer.Deserialize<List<string>>(serviceJson);
        if(services == null || services.Count == 0) return;

        foreach(var service in services)
        {
            if(!_db.Services.Any(x => x.NomService == service))
            {
                _db.Services.Add(new Service() { NomService = service });
            }
        }
        await _db.SaveChangesAsync();
    }
    private async Task CreateDefaultUsers()
    {
        var usersJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Services", "DatabaseSeeder", "default_users.json"));
        var users = JsonSerializer.Deserialize<List<DefaultUser>>(usersJson);

        if(users == null || users.Count == 0) return;

        foreach(var user in users)
        {
              var existingUser = await _userManager.FindByEmailAsync(user.Email);
              if(existingUser != null) { _logger.LogError($"L'utilisateur {user.Email} existe déja en bdd");  continue; }

              var newUser = new User
                {
                    UserName = user.Username,
                    Email = user.Email,
                    Nom = user.Nom,
                    Prenom = user.Prenom
                };

                if(!await _accountService.CreateAccountAsync(newUser, user.Password)) continue;

                var roleExist = await _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>().RoleExistsAsync(user.Role);
                if(!roleExist) { _logger.LogError($"Le rôle {user.Role} n'existe pas."); continue; }

                if(!await _accountService.ChangeRoleToUserAsync(newUser, (Role)Enum.Parse(typeof(Role), user.Role)))  _logger.LogInformation($"Utilisateur {newUser.UserName} n'a pas pu être ajouté avec le rôle {user.Role}.");
                 _logger.LogInformation($"Utilisateur {newUser.UserName} ajouté avec le rôle {user.Role}.");


        }

 
    }

    private async Task CreateRolesAsync()
    {
        
    #pragma warning disable CS8600, CS8604
        var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach(var role in Enum.GetValues(typeof(Role)))
        {
            
            string roleName = role.ToString();
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                _logger.LogInformation($"Rôle {roleName} créé");
            }
        }
        #pragma warning restore CS8600, CS8604
    }


}