using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBillApi.Services;

namespace PurchaseBillApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    /// <summary>GET api/location - populates the Batch dropdown in the Purchase Bill form.</summary>
    [HttpGet]
    public async Task<IActionResult> GetLocations()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        var locations = await _locationService.GetLocationsForUserAsync(username);
        return Ok(locations);
    }
}
