using Microsoft.EntityFrameworkCore;
using PurchaseBillApi.Data;
using PurchaseBillApi.DTOs;
using PurchaseBillApi.Models;

namespace PurchaseBillApi.Services;

public interface IPurchaseBillService
{
    Task<List<PurchaseOrderDto>> GetAllAsync();
    Task<PurchaseOrderDto> SaveOrderAsync(SavePurchaseOrderRequest request, string? username);
    Task<List<LatestOrderWidgetDto>> GetLatestOrdersAsync(int take = 5);
    Task<List<OldestItemWidgetDto>> GetOldestItemsAsync(int take = 10);
    Task<List<ItemGroupWidgetDto>> GetItemsGroupedAsync();
}

/// <summary>Handles Task 1 (save/retrieve purchase orders) and the 3 dashboard widget queries.
/// Total Cost / Total Selling / Net Amount are all recomputed server-side so the API stays the
/// source of truth even though the UI also calculates them live for instant feedback.</summary>
public class PurchaseBillService : IPurchaseBillService
{
    private readonly AppDbContext _db;

    public PurchaseBillService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PurchaseOrderDto>> GetAllAsync()
    {
        return await _db.PurchaseOrders
            .Include(o => o.Items)
            .OrderByDescending(o => o.Id)
            .Select(o => MapOrder(o))
            .ToListAsync();
    }

    /// <summary>Task 1: saves the whole purchase order (header + every line item currently in
    /// the UI table) in a single call using the fields supplied by the UI.</summary>
    public async Task<PurchaseOrderDto> SaveOrderAsync(SavePurchaseOrderRequest request, string? username)
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            throw new ArgumentException("At least one item is required to save a purchase order.");
        }

        var order = new PurchaseOrder
        {
            Username = username,
            Created_At = DateTime.UtcNow
        };

        decimal netAmount = 0;
        decimal totalQty = 0;

        foreach (var item in request.Items)
        {
            // Total Cost = (Standard Cost x Qty) - Discount%
            // Total Selling = Standard Price x Qty
            var totalCost = (item.StandardCost * item.Qty) * (1 - (item.Discount / 100m));
            var totalSelling = item.StandardPrice * item.Qty;

            order.Items.Add(new PurchaseBillItem
            {
                Item_Name = item.ItemName,
                Batch = item.Batch,
                Standard_Cost = item.StandardCost,
                Standard_Price = item.StandardPrice,
                Margin = item.Margin,
                Qty = item.Qty,
                Free_Qty = item.FreeQty,
                Discount = item.Discount,
                Total_Cost = totalCost,
                Total_Selling = totalSelling,
                Created_At = DateTime.UtcNow
            });

            netAmount += totalSelling;
            totalQty += item.Qty;
        }

        order.Net_Amount = netAmount;
        order.Total_Items = order.Items.Count;
        order.Total_Qty = totalQty;

        _db.PurchaseOrders.Add(order);
        await _db.SaveChangesAsync();

        return MapOrder(order);
    }

    /// <summary>Widget 1 (Table View): latest 5 purchase orders - ID, Net Amount, No. of Items.</summary>
    public async Task<List<LatestOrderWidgetDto>> GetLatestOrdersAsync(int take = 5)
    {
        return await _db.PurchaseOrders
            .OrderByDescending(o => o.Created_At)
            .ThenByDescending(o => o.Id)
            .Take(take)
            .Select(o => new LatestOrderWidgetDto
            {
                Id = o.Id,
                NetAmount = o.Net_Amount,
                NoOfItems = o.Total_Items
            })
            .ToListAsync();
    }

    /// <summary>Widget 2 (List View): oldest 10 purchase order items - Purchase Order ID,
    /// Item Name, No. of Quantity.</summary>
    public async Task<List<OldestItemWidgetDto>> GetOldestItemsAsync(int take = 10)
    {
        return await _db.PurchaseBillItems
            .OrderBy(i => i.Created_At)
            .ThenBy(i => i.Id)
            .Take(take)
            .Select(i => new OldestItemWidgetDto
            {
                PurchaseOrderId = i.Purchase_Order_Id,
                ItemName = i.Item_Name,
                Quantity = i.Qty
            })
            .ToListAsync();
    }

    /// <summary>Widget 3 (Donut Chart): all items grouped by Item Name and No. of Quantity.</summary>
    public async Task<List<ItemGroupWidgetDto>> GetItemsGroupedAsync()
    {
        return await _db.PurchaseBillItems
            .GroupBy(i => i.Item_Name)
            .Select(g => new ItemGroupWidgetDto
            {
                ItemName = g.Key,
                TotalQuantity = g.Sum(i => i.Qty)
            })
            .OrderByDescending(g => g.TotalQuantity)
            .ToListAsync();
    }

    private static PurchaseOrderDto MapOrder(PurchaseOrder o) => new()
    {
        Id = o.Id,
        NetAmount = o.Net_Amount,
        TotalItems = o.Total_Items,
        TotalQty = o.Total_Qty,
        CreatedAt = o.Created_At,
        Items = o.Items.Select(i => new PurchaseBillItemDto
        {
            Id = i.Id,
            ItemName = i.Item_Name,
            Batch = i.Batch,
            StandardCost = i.Standard_Cost,
            StandardPrice = i.Standard_Price,
            Margin = i.Margin,
            Qty = i.Qty,
            FreeQty = i.Free_Qty,
            Discount = i.Discount,
            TotalCost = i.Total_Cost,
            TotalSelling = i.Total_Selling
        }).ToList()
    };
}