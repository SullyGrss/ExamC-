using ExamenCSharp.Models;
using Microsoft.AspNetCore.Mvc;
using ExamenCSharp.Services;
using Microsoft.Extensions.Localization;
using ExamenCSharp.Roles;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase {

    private readonly AccountService _accountService;
    IStringLocalizer<ReservationController> _localizer;

    public AccountController(AccountService accountService, IStringLocalizer<ReservationController> localizer)
    {
        _accountService = accountService;
        _localizer = localizer;
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        try {
            var user = await _accountService.GetAccountAsync(email: username);
            if (user == null) return BadRequest(_localizer["Error_InvalidCredentials"]);
            var token = await _accountService.GenerateToken(user, password);

            return Ok(new { Token = token});
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    

    [HttpPost("register")]
    public async Task<IActionResult> Register(string lastName, string firstName, string email, string password)
    {
        if(string.IsNullOrEmpty(lastName)) return BadRequest(_localizer["Validation_EmptyLastName"]);
        if(string.IsNullOrEmpty(firstName)) return BadRequest(_localizer["Validation_EmptyFirstName"]);
        if(string.IsNullOrEmpty(email)) return BadRequest(_localizer["Validation_EmptyEmail"]);
        if(string.IsNullOrEmpty(password)) return BadRequest(_localizer["Validation_EmptyPassword"]);

        var user = new User()
        {
            UserName = email,
            Email = email,
            Nom = lastName,
            Prenom = firstName,
        };
        var response = await _accountService.CreateAccountAsync(user, password);
        return response == false ? BadRequest(_localizer["Error_AccountCreationFailed"]) : Ok(_localizer["Success_AccountCreated"]);

    }


    [HttpPost("register-technicien")]
    public async Task<IActionResult> RegisterTechnicien(string lastName, string firstName, string email, string password)
    {
        if(string.IsNullOrEmpty(lastName)) return BadRequest(_localizer["Validation_EmptyLastName"]);
        if(string.IsNullOrEmpty(firstName)) return BadRequest(_localizer["Validation_EmptyFirstName"]);
        if(string.IsNullOrEmpty(email)) return BadRequest(_localizer["Validation_EmptyEmail"]);
        if(string.IsNullOrEmpty(password)) return BadRequest(_localizer["Validation_EmptyPassword"]);

        var user = new User()
        {
            UserName = email,
            Email = email,
            Nom = lastName,
            Prenom = firstName,
        };
        var response = await _accountService.CreateAccountAsync(user, password);
        if(response == false)
        {
            return BadRequest(_localizer["Error_AccountCreationFailed"]);
        }
        await ChangeRole(email, Role.Technicien);
        return Ok(_localizer["Success_AccountCreated"]);

    }

    [HttpPost("modifier-role")]
    public async Task<IActionResult> ChangeRole(string email, Role role)
    {
        if(string.IsNullOrEmpty(email)) return BadRequest(_localizer["Validation_EmptyLastName"]);

        var user = await _accountService.GetAccountAsync(email);
        if(user == null) return BadRequest(_localizer["Error_NotEmpty"]);


        var result = await _accountService.ChangeRoleToUserAsync(user, role);
        if(result) return Ok();
        else return BadRequest();


    }
}