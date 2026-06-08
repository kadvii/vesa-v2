using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Vesa.DTOs.Auth;
using Vesa.Models;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class AuthService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration) : IAuthService
{
    public async Task<(bool success, AuthResponse? data, string? error)> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return (false, null, "Email is already registered.");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            NationalId = request.NationalId,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join(" ", result.Errors.Select(e => e.Description));
            return (false, null, errorMsg);
        }

        // Ensure Applicant role exists
        if (!await roleManager.RoleExistsAsync("Applicant"))
        {
            await roleManager.CreateAsync(new IdentityRole("Applicant"));
        }

        await userManager.AddToRoleAsync(user, "Applicant");

        // Login after register
        return await LoginAsync(new LoginRequest { Email = request.Email, Password = request.Password });
    }

    public async Task<(bool success, AuthResponse? data, string? error)> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return (false, null, "Invalid email or password.");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return (false, null, "Invalid email or password.");

        var roles = await userManager.GetRolesAsync(user);
        var expiryHours = configuration.GetValue<int>("Jwt:ExpiryHours", 8);
        var expiresAt = DateTime.UtcNow.AddHours(expiryHours);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FullName)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return (true, new AuthResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Roles = roles
        }, null);
    }

    public async Task<ProfileResponse?> GetProfileAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return null;

        return new ProfileResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            NationalId = user.NationalId,
            DateOfBirth = user.DateOfBirth,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<(bool success, string? error)> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return (false, "User not found.");

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.DateOfBirth = request.DateOfBirth;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join(" ", result.Errors.Select(e => e.Description));
            return (false, errorMsg);
        }

        return (true, null);
    }

    public async Task<(bool success, string? error)> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return (false, "User not found.");

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join(" ", result.Errors.Select(e => e.Description));
            return (false, errorMsg);
        }

        return (true, null);
    }
}
