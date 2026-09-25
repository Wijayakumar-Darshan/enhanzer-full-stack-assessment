using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBillApi.DTOs;
using PurchaseBillApi.Services;

namespace PurchaseBillApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseBillController : ControllerBase
{
    private readonly IPurchaseBillService _purchaseBillService;

    public PurchaseBillController(IPurchaseBillService purchaseBillService)
    {
        _purchaseBillService = purchaseBillService;
    }

    /// <summary>GET api/purchasebill - lists saved purchase orders (header + items), newest first.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _purchaseBillService.GetAllAsync();
        return Ok(orders);
    }

    /// <summary>POST api/purchasebill - Task 1: saves the purchase order using the fields
    /// available in the UI (every item row currently on screen) and persists it to SQL.</summary>
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SavePurchaseOrderRequest request)
    {
        if (request?.Items == null || request.Items.Count == 0)
        {
            return BadRequest("At least one item is required to save a purchase order.");
        }

        foreach (var item in request.Items)
        {
            if (string.IsNullOrWhiteSpace(item.ItemName) || item.Qty <= 0)
            {
                return BadRequest("Every item needs a name and a positive quantity.");
            }
        }

        var username = User.FindFirstValue(ClaimTypes.Name);
        var saved = await _purchaseBillService.SaveOrderAsync(request, username);
        return CreatedAtAction(nameof(GetAll), new { id = saved.Id }, saved);
    }
}