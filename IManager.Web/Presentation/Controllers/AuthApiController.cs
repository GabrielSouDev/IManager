using IManager.Web.Application.Services;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace IManager.Web.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private JwtTokenService _jwtTokenService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthApiController(JwtTokenService jwtTokenService, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _jwtTokenService = jwtTokenService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded) return Unauthorized();

        var token = await _jwtTokenService.GenerateTokenAsync(user);
        return Ok(new { token });
    }
}
