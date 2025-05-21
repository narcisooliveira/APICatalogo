using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(
        ITokenService tokenService, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.Username!);
        if (user is null)
            return NotFound("User not found");

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password!);
        if (!result) 
            return Unauthorized("Invalid email or password");
        
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new("Id", user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = _tokenService.GenerateToken(claims, _configuration);
        var refreshToken = _tokenService.GenerateRefreshToken();
        int.TryParse(_configuration["JWT:RefreshTokenExpireTime"], out var expiration);
        
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(expiration);
        user.RefreshToken = refreshToken;
        
        await _userManager.UpdateAsync(user);

        return Ok(new
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken,
            expiration = DateTime.UtcNow.AddMinutes(expiration)
        });
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var existingUser = await _userManager.FindByNameAsync(registerDto.Username!);
        if (existingUser is not null)
            return BadRequest("User already exists");
        
        var user = new ApplicationUser
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        
        var result = await _userManager.CreateAsync(user, registerDto.Password!);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User registered successfully");
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(tokenDto.Token!, _configuration);
        var user = await _userManager.FindByIdAsync(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        if (user is null || user.RefreshToken != tokenDto.RefreshToken)
            return Unauthorized("Invalid refresh token");

        var newToken = _tokenService.GenerateToken(principal.Claims, _configuration);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(newToken),
            refreshToken = newRefreshToken
        });
    }
    
    [Authorize(policy: "ExclusiveOnly")]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
            return NotFound("User not found");

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return NoContent();
    }
    
    [Authorize(policy: "MasterOnly")]
    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            _logger.LogInformation("Role already exists");
            return BadRequest("Role already exists");
        }
        
        var role = new IdentityRole
        {
            Name = roleName,
            NormalizedName = roleName.ToUpper()
        };
        
        var result = await _roleManager.CreateAsync(role);
        if (result.Succeeded) 
            return Ok("Role created successfully");
        
        _logger.LogInformation("Error to create role");
        return BadRequest(result.Errors);
    }
    
    [Authorize(policy: "MasterOnly")]
    [HttpPost("add-role-to-user")]
    public async Task<IActionResult> AddRoleToUser(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            _logger.LogInformation("User not found");
            return NotFound("User not found");
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded) 
            return Ok("Role added to user successfully");
        
        _logger.LogInformation("Error to add {username} to role {roleName}", email, roleName);
        return BadRequest(result.Errors);
    }
}