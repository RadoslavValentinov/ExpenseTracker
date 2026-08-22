using ExpenseTracker.API.DTOs;
using ExpenseTracker.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }


    // =========================
    // REGISTER
    // =========================

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Password is required.");
        }

        var existingUser =
            await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            return BadRequest("User already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result =
            await _userManager.CreateAsync(
                user,
                dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(
                result.Errors.Select(e => e.Description));
        }

        return Ok(new
        {
            message = "User registered successfully."
        });
    }


    // =========================
    // LOGIN
    // =========================

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Password is required.");
        }

        var user =
            await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var passwordValid =
            await _userManager.CheckPasswordAsync(
                user,
                dto.Password);

        if (!passwordValid)
        {
            return Unauthorized("Invalid email or password.");
        }


        // =========================
        // CREATE JWT
        // =========================

        var jwtKey =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT Key is not configured.");

        var jwtIssuer =
            _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT Issuer is not configured.");

        var jwtAudience =
            _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT Audience is not configured.");


        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id),

            new Claim(
                ClaimTypes.Name,
                user.UserName ?? user.Email ?? string.Empty),

            new Claim(
                ClaimTypes.Email,
                user.Email ?? string.Empty)
        };


        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));


        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);


        var token =
            new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);


        var tokenString =
            new JwtSecurityTokenHandler()
                .WriteToken(token);


        return Ok(new
        {
            message = "Login successful.",
            token = tokenString,
            expiresAt = token.ValidTo
        });
    }
}