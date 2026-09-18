using Microsoft.AspNetCore.Mvc;
using PurchaseBillApi.DTOs;

namespace PurchaseBillApi.Controllers;

/// <summary>
/// FOR LOCAL DEVELOPMENT/TESTING ONLY.
///
/// Mimics the shape of Enhanzer's external POS API
/// (https://ez-staging-api.azurewebsites.net/api/External_Api/POS_Api/Invoke)
/// so you can test the full login -> Location_Details -> Purchase Bill flow
/// before you have a real staging account.
///
/// To use it:
///   1. In appsettings.json, temporarily change ExternalPosApi:BaseUrl to:
///        "https://localhost:7000/api/mock/pos-invoke"
///   2. Restart the backend.
///   3. Log in through the Angular app using:
///        email:    demo@enhanzer.com
///        password: Demo#123
///      (any other email/password combination returns "Invalid Login Details",
///      just like the real API does, so you can also test the failure path.)
///   4. Once Enhanzer sends you real credentials, change ExternalPosApi:BaseUrl
///      back to the real URL before your final submission/recording.
/// </summary>
[ApiController]
[Route("api/mock")]
public class MockPosController : ControllerBase
{
    private const string DemoEmail = "demo@enhanzer.com";
    private const string DemoPassword = "Demo#123";

    [HttpPost("pos-invoke")]
    public IActionResult Invoke([FromBody] PosApiRequest request)
    {
        var emailMatches = string.Equals(request.API_Body.Username, DemoEmail, StringComparison.OrdinalIgnoreCase);
        var passwordMatches = request.API_Body.Pw == DemoPassword;

        if (!emailMatches || !passwordMatches)
        {
            // Mirrors the real API's failure shape exactly.
            return Ok(new
            {
                Status_Code = 200,
                Sync_Time = "",
                Message = "GetLoginData POS API Executed Successfully.",
                Response_Body = new object[]
                {
                    new { Doc_Msg = "Invalid Login Details" }
                }
            });
        }

        // Mirrors the assumed success shape: Response_Body[0].User_Locations.
        return Ok(new
        {
            Status_Code = 200,
            Sync_Time = "",
            Message = "GetLoginData POS API Executed Successfully.",
            Response_Body = new object[]
            {
                new
                {
                    User_Locations = new[]
                    {
                        new { Location_Code = "LOC001", Location_Name = "Main Warehouse" },
                        new { Location_Code = "LOC002", Location_Name = "Colombo Branch" },
                        new { Location_Code = "LOC003", Location_Name = "Kandy Branch" }
                    }
                }
            }
        });
    }
}
