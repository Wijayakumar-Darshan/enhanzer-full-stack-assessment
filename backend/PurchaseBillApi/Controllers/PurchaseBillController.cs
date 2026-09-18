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

    /// <summary>GET api/purchasebill - list existing items (optional, for reload/demo purposes).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _purchaseBillService.GetAllAsync();
        return Ok(items);
    }

    /// <summary>POST api/purchasebill - Task 2: add a purchase bill line item.</summary>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] PurchaseBillItemDto item)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(item.ItemName) || item.Qty <= 0)
        {
            return BadRequest("Item name and a positive quantity are required.");
        }

        var saved = await _purchaseBillService.AddAsync(item);
        return CreatedAtAction(nameof(GetAll), new { id = saved.Id }, saved);
    }
}
