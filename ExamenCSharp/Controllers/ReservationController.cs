using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ExamenCSharp.Services;
using ExamenCSharp.Models;
using Microsoft.Extensions.Localization;


[Authorize(Roles = "Technicien")]
[ApiController]
[Route("api/reservation")]
public class ReservationController : ControllerBase {

    private readonly AccountService _accountService;
    private readonly ReservationService _reservationService;
    IStringLocalizer<ReservationController> _localizer;
    public ReservationController(AccountService accountService, ReservationService reservationService, IStringLocalizer<ReservationController> localizer) 
    {
        _accountService = accountService;
        _reservationService = reservationService;
        _localizer = localizer;
    }
    
    [AllowAnonymous]
    [HttpGet("recupérer-services")]
    public async Task<IActionResult> GetServices()
    {
        var services = await _reservationService.GetAllServicesAsync();
        return Ok(services);
    }


    [HttpPost("ajouter-service")]
    public async Task<IActionResult> AddService(string name)
    {
        if(string.IsNullOrEmpty(name)) throw new Exception(_localizer["Error_NotEmpty"]);

        await _reservationService.AjouterNouveauService(name);
        return Ok();
    }

    [HttpGet("reservations")]
    public async Task<IActionResult> GetReservations(int? id = null)
    {
        
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;;
        if(string.IsNullOrEmpty(username)) return Unauthorized();

        var user = await _accountService.GetAccountAsync(id: username);
        if(user == null) throw new Exception(_localizer["Validation_UserNotFound"]);
        var reservations = await _reservationService.GetReservationsAsync(user);
        return Ok(reservations);
    }

    
    [HttpPost]
    public async Task<IActionResult> CreerReservation(string emailClient, [FromBody] string[] emailsTechnicien, string typeIntervention, DateTime dateRdv)
    {


        if(dateRdv < DateTime.Now) throw new ArgumentException(_localizer["Validation_InvalidDate"]);
        if(string.IsNullOrEmpty(emailClient)) return BadRequest(_localizer["Error_ClientEmailRequired"]);
        if(emailsTechnicien.Count() < 0) return BadRequest(_localizer["Error_TechnicianRequired"]);
        
        var typeService = await _reservationService.GetServiceAsync(typeIntervention);
        if(typeService == null) throw new Exception(_localizer["Error_ServiceUnknown"]);


        Intervention newIntervension = new()
        {
            Reservation = dateRdv,
            Type = typeService,
            Techniciens = new()

        };
        
        var intervention = await _reservationService.ReserverAsync(newIntervension, emailClient, emailsTechnicien.ToList());

        return intervention == null ? BadRequest() : Ok();
    }
}