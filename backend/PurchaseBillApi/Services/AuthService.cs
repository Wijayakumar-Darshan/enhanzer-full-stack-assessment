using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PurchaseBillApi.Data;
using PurchaseBillApi.DTOs;
using PurchaseBillApi.Models;

namespace PurchaseBillApi.Services;

/// <summary>
/// Handles Task 1: authenticates the user against the external POS API,
/// persists the returned locations into Location_Details, and issues a
/// JWT so the Angular app can protect the Purchase Bill page.
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(HttpClient httpClient, AppDbContext db, IConfiguration config, ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _db = db;
        _config = config;
        _logger = logger;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResponseDto { Success = false, Message = "Email and password are required." };
        }

        var posRequest = new PosApiRequest
        {
            API_Action = "GetLoginData",
            Device_Id = _config["ExternalPosApi:DeviceId"] ?? "D001",
            Sync_Time = string.Empty,
            Company_Code = request.Email,
            API_Body = new PosApiBody
            {
                Username = request.Email,
                Pw = request.Password
            }
        };

        var externalUrl = _config["ExternalPosApi:BaseUrl"]
            ?? "https://ez-staging-api.azurewebsites.net/api/External_Api/POS_Api/Invoke";

        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync(externalUrl, posRequest);

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("External POS API returned {StatusCode}", httpResponse.StatusCode);
                return new LoginResponseDto { Success = false, Message = "Invalid email or password." };
            }

            var raw = await httpResponse.Content.ReadAsStringAsync();

            // TEMPORARY DEBUG LOGGING: prints the exact shape the external API returns,
            // so we can fix the deserialization to match it. Remove once confirmed working.
            _logger.LogWarning("RAW POS API RESPONSE: {Raw}", raw);
            Console.WriteLine("==== RAW POS API RESPONSE ====");
            Console.WriteLine(raw);
            Console.WriteLine("===============================");

            var posResponse = JsonSerializer.Deserialize<PosApiResponse>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var firstEntry = posResponse?.Response_Body?.Count > 0
                ? posResponse.Response_Body[0]
                : (JsonElement?)null;

            // Failure case: Response_Body[0] has a "Doc_Msg" field describing the error.
            // Uses case-insensitive lookup because different callers of the Invoke endpoint
            // (the real API vs. our own local mock) don't serialize with the same casing.
            if (firstEntry is JsonElement entry &&
                entry.ValueKind == JsonValueKind.Object &&
                TryGetPropertyIgnoreCase(entry, "Doc_Msg", out var docMsgEl))
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = docMsgEl.GetString() ?? "Invalid email or password."
                };
            }

            // Success case: expect Response_Body[0] to carry a User_Locations array.
            if (firstEntry is JsonElement successEntry &&
                successEntry.ValueKind == JsonValueKind.Object &&
                TryGetPropertyIgnoreCase(successEntry, "User_Locations", out var locationsEl) &&
                locationsEl.ValueKind == JsonValueKind.Array)
            {
                var locations = JsonSerializer.Deserialize<List<UserLocationDto>>(
                    locationsEl.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<UserLocationDto>();

                await SaveLocationsAsync(request.Email, locations);

                var token = GenerateJwt(request.Email);

                return new LoginResponseDto
                {
                    Success = true,
                    Message = "Login successful.",
                    Token = token,
                    UserLocations = locations
                };
            }

            // Unknown shape: log it so we can extend the parsing once we see a real success response.
            _logger.LogWarning("Unrecognized POS API response shape: {Raw}", raw);
            return new LoginResponseDto
            {
                Success = false,
                Message = "Unexpected response from the authentication service."
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the external POS API.");
            return new LoginResponseDto { Success = false, Message = "Unable to reach the authentication service. Please try again later." };
        }
    }

    /// <summary>
    /// JsonElement.TryGetProperty is case-sensitive by default (unlike JsonSerializerOptions.
    /// PropertyNameCaseInsensitive, which only affects model binding). Different sources hitting
    /// the same Invoke endpoint shape (the real API vs. our local mock) don't agree on casing,
    /// so we look properties up manually, ignoring case.
    /// </summary>
    private static bool TryGetPropertyIgnoreCase(JsonElement element, string propertyName, out JsonElement value)
    {
        foreach (var prop in element.EnumerateObject())
        {
            if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = prop.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private async Task SaveLocationsAsync(string username, List<UserLocationDto> locations)
    {
        // Remove any previous locations for this user, then insert the fresh set.
        var existing = _db.LocationDetails.Where(l => l.Username == username);
        _db.LocationDetails.RemoveRange(existing);

        foreach (var loc in locations)
        {
            _db.LocationDetails.Add(new LocationDetail
            {
                Location_Code = loc.Location_Code,
                Location_Name = loc.Location_Name,
                Username = username,
                Created_At = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
    }

    private string GenerateJwt(string email)
    {
        var jwtKey = _config["Jwt:Key"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiryMinutes = double.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
