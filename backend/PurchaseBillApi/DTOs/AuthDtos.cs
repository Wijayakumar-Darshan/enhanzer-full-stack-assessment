namespace PurchaseBillApi.DTOs;

/// <summary>What the Angular login form sends to our backend.</summary>
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>What our backend sends back to Angular after a successful login.</summary>
public class LoginResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public List<UserLocationDto> UserLocations { get; set; } = new();
}

public class UserLocationDto
{
    public string Location_Code { get; set; } = string.Empty;
    public string Location_Name { get; set; } = string.Empty;
}

/// <summary>Shape of the outbound request to the external POS API (Task 1 spec).</summary>
public class PosApiRequest
{
    public string API_Action { get; set; } = "GetLoginData";
    public string Device_Id { get; set; } = "D001";
    public string Sync_Time { get; set; } = string.Empty;
    public string Company_Code { get; set; } = string.Empty;
    public PosApiBody API_Body { get; set; } = new();
}

public class PosApiBody
{
    public string Username { get; set; } = string.Empty;
    public string Pw { get; set; } = string.Empty;
}

/// <summary>
/// Real shape returned by the external POS API's generic Invoke endpoint.
/// The actual login result lives inside Response_Body, whose shape differs
/// between failure (Doc_Msg) and success (expected to carry User_Locations).
/// </summary>
public class PosApiResponse
{
    public int? Status_Code { get; set; }
    public string? Sync_Time { get; set; }
    public string? Message { get; set; }
    public List<System.Text.Json.JsonElement>? Response_Body { get; set; }
}
