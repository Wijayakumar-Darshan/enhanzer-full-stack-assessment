using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBillApi.Services;

namespace PurchaseBillApi.Controllers;

/// <summary>Feeds the 3 welcome-dashboard widgets (Table View, List View, Donut Chart).</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IPurchaseBillService _purchaseBillService;

    public DashboardController(IPurchaseBillService purchaseBillService)
    {
        _purchaseBillService = purchaseBillService;
    }

    /// <summary>Widget 1 (Table View): GET api/dashboard/latest-orders
    /// - latest 5 purchase orders added: ID, Net Amount, No. of Items.</summary>
    [HttpGet("latest-orders")]
    public async Task<IActionResult> GetLatestOrders()
    {
        var data = await _purchaseBillService.GetLatestOrdersAsync(5);
        return Ok(data);
    }

    /// <summary>Widget 2 (List View): GET api/dashboard/oldest-items
    /// - oldest 10 purchase order items added: Purchase Order ID, Item Name, No. of Quantity.</summary>
    [HttpGet("oldest-items")]
    public async Task<IActionResult> GetOldestItems()
    {
        var data = await _purchaseBillService.GetOldestItemsAsync(10);
        return Ok(data);
    }

    /// <summary>Widget 3 (Donut Chart): GET api/dashboard/items-summary
    /// - all items grouped by Item Name and No. of Quantity.</summary>
    [HttpGet("items-summary")]
    public async Task<IActionResult> GetItemsSummary()
    {
        var data = await _purchaseBillService.GetItemsGroupedAsync();
        return Ok(data);
    }
}