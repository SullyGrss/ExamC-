using ExamenCSharp.DTO;
using ExamenCSharp.Models;
using ExamenCSharp.Services;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
public class ReservationService : IReservation 
{
    private readonly ApplicationDbContext _db;
    private AccountService _accountService;
    IStringLocalizer<ReservationController> _localizer;
    public ReservationService(ApplicationDbContext db, AccountService accountService, IStringLocalizer<ReservationController> localizer)
    {
        _db = db;
        _accountService = accountService;
         _localizer = localizer;
    }

    public async Task AjouterNouveauService(string name){
        if(_db.Services.Any(x => x.NomService.ToLower() == name.ToLower())) throw new Exception(_localizer["Error_ServiceDouble"]);

        await _db.Services.AddAsync(new Service()
        {
            NomService = name
        });

        await _db.SaveChangesAsync();
    }

    public async Task<List<ServiceDTO>> GetAllServicesAsync()
    {
        List<ServiceDTO> services = new List<ServiceDTO>();
        var servicesDb = await _db.Services.ToListAsync();
        foreach(var service in servicesDb)
        {
            services.Add(new ServiceDTO()
            {
                Id = service.Id,
                NomService = service.NomService,
            });
        }
        return services;
    }
    public async Task<Service?> GetServiceAsync(string name){
        var value = await  _db.Services.FirstOrDefaultAsync(x => x.NomService == name);
        return value;
    }
    public async Task<List<InterventionDTO>> GetReservationsAsync(User user)
    {
        var list = await _db.Interventions.Include(x => x.Techniciens).Include(x => x.Client).Where(x => x.Techniciens.Any(i => i.Id == user.Id)).ToListAsync();
        List<InterventionDTO> interventionDTOs = new();
        foreach(var intervention in list)
        {
    #pragma warning disable CS8602 // Dereference of a possibly null reference.
    #pragma warning disable CS8601 // Possible null reference assignment.
            List<UserDTO> tchs = new();
            foreach(var t in intervention.Techniciens.ToList())
            {
                tchs.Add(new UserDTO()
                {
                    Nom = t.Nom,
                    Prenom = t.Prenom,
                    Email = t.Email
                });
            }

            interventionDTOs.Add(new InterventionDTO(){
                Id = intervention.Id,
                Reservation = intervention.Reservation,
                Techniciens = tchs,
                Type = new ServiceDTO()
                {
                    NomService = intervention.Type.NomService
                },
                Client = new UserDTO()
                {
                    Nom = intervention.Client.Nom,
                    Prenom = intervention.Client.Prenom,
                    Email = intervention.Client.Email
                }


            });
    #pragma warning restore CS8601 // Possible null reference assignment.
    #pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        return interventionDTOs;
    }
    public async Task<Intervention?> ReserverAsync(Intervention intervention, string emailReserviste, List<string> emailsTechnicien)
    {
        if(emailsTechnicien.Count() <= 0) return null;

        List<User> techniciens = new();
        foreach(var emailTechnicien in emailsTechnicien){
            var tchn = await _accountService.GetAccountAsync(email: emailTechnicien);
            if(tchn == null) continue;

            var roleUsr = await _accountService.GetRolesAsync(tchn);
            
            if(roleUsr == null || !roleUsr.Any(x => x.ToString().ToLower() == "technicien")) continue;
            techniciens.Add(tchn);
        }

        var usr = await _accountService.GetAccountAsync(email: emailReserviste);
        if(usr == null) return null;

        intervention.Client = usr;
        intervention.Techniciens = techniciens;
        _db.Interventions.Add(intervention);
        await _db.SaveChangesAsync();

        return intervention;
    }
    public async Task ModifierReservationAsync(int id, Intervention intervention)
    {
        await Task.Delay(0);
        return;
    }
    public async Task AnnulerReservationAsync(int id)
    {
        var reservation = _db.Interventions.FirstOrDefault(x => x.Id == id);
        if(reservation == null) return;

        _db.Interventions.Remove(reservation);
        await _db.SaveChangesAsync();
    }
}