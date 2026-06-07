using Vesa.DTOs.Auth;

namespace Vesa.Services.Interfaces;

public interface IAuthService
{
    Task<(bool success, AuthResponse? data, string? error)> RegisterAsync(RegisterRequest request);
    Task<(bool success, AuthResponse? data, string? error)> LoginAsync(LoginRequest request);
}
