using PurchaseBillApi.DTOs;

namespace PurchaseBillApi.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}
