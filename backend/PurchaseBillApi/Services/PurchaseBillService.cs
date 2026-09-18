using Microsoft.EntityFrameworkCore;
using PurchaseBillApi.Data;
using PurchaseBillApi.DTOs;
using PurchaseBillApi.Models;

namespace PurchaseBillApi.Services;

public interface IPurchaseBillService
{
    Task<List<PurchaseBillItemDto>> GetAllAsync();
    Task<PurchaseBillItemDto> AddAsync(PurchaseBillItemDto item);
}

/// <summary>Handles Task 2: adding a purchase bill line item and computing totals server-side too
/// (the Angular app also computes these live, but we recompute here so the API is the source of truth).</summary>
public class PurchaseBillService : IPurchaseBillService
{
    private readonly AppDbContext _db;

    public PurchaseBillService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PurchaseBillItemDto>> GetAllAsync()
    {
        return await _db.PurchaseBillItems
            .OrderByDescending(i => i.Id)
            .Select(i => new PurchaseBillItemDto
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
            })
            .ToListAsync();
    }

    public async Task<PurchaseBillItemDto> AddAsync(PurchaseBillItemDto item)
    {
        // Total Cost = (Standard Cost x Qty) - Discount%
        // Total Selling = Standard Price x Qty
        var totalCost = (item.StandardCost * item.Qty) * (1 - (item.Discount / 100m));
        var totalSelling = item.StandardPrice * item.Qty;

        var entity = new PurchaseBillItem
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
        };

        _db.PurchaseBillItems.Add(entity);
        await _db.SaveChangesAsync();

        item.Id = entity.Id;
        item.TotalCost = totalCost;
        item.TotalSelling = totalSelling;
        return item;
    }
}
