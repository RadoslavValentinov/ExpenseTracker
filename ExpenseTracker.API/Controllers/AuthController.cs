using ExpenseTracker.API.DTOs;
using ExpenseTracker.Core.Models;
using Microsoft.AspNetCore.Authorization;
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
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;


    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }


    // =========================
    // REGISTER
    // =========================

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterDto dto)
    {
        if (dto == null)
        {
            return BadRequest(
                "Invalid registration data.");
        }


        var email =
            dto.Email?.Trim();


        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(
                "Email is required.");
        }


        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(
                "Password is required.");
        }


        var existingUser =
            await _userManager.FindByEmailAsync(
                email);


        if (existingUser != null)
        {
            return BadRequest(
                "User already exists.");
        }


        var user =
            new ApplicationUser
            {
                UserName = email,
                Email = email
            };


        var result =
            await _userManager.CreateAsync(
                user,
                dto.Password);


        if (!result.Succeeded)
        {
            return BadRequest(
                result.Errors
                    .Select(e => e.Description));
        }


        return Ok(new
        {
            message =
                "User registered successfully."
        });
    }


    // =========================
    // LOGIN
    // =========================

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto dto)
    {
        if (dto == null)
        {
            return BadRequest(
                "Invalid login data.");
        }


        var email =
            dto.Email?.Trim();


        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(
                "Email is required.");
        }


        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(
                "Password is required.");
        }


        var user =
            await _userManager.FindByEmailAsync(
                email);


        if (user == null)
        {
            return Unauthorized(
                "Invalid email or password.");
        }


        // =========================
        // PASSWORD / SIGN-IN
        // =========================

        var signInResult =
            await _signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                lockoutOnFailure: true);


        if (!signInResult.Succeeded)
        {
            return Unauthorized(
                "Invalid email or password.");
        }


        // =========================
        // JWT CONFIGURATION
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


        if (jwtKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT Key must contain at least 32 characters.");
        }


        // =========================
        // CLAIMS
        // =========================

        var claims =
            new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id),

                new Claim(
                    ClaimTypes.Name,
                    user.UserName
                        ?? user.Email
                        ?? string.Empty),

                new Claim(
                    ClaimTypes.Email,
                    user.Email
                        ?? string.Empty)
            };


        // =========================
        // SIGNING
        // =========================

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtKey));


        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);


        // =========================
        // TOKEN
        // =========================

        var expiresAt =
            DateTime.UtcNow.AddHours(2);


        var token =
            new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);


        var tokenString =
            new JwtSecurityTokenHandler()
                .WriteToken(token);


        // =========================
        // RESPONSE
        // =========================

        return Ok(new
        {
            message =
                "Login successful.",

            token =
                tokenString,

            expiresAt
        });
    }
}