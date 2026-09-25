namespace PurchaseBillApi.DTOs;

public class PurchaseBillItemDto
{
    public int Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Batch { get; set; } = string.Empty;
    public decimal StandardCost { get; set; }
    public decimal StandardPrice { get; set; }
    public decimal Margin { get; set; }
    public decimal Qty { get; set; }
    public decimal FreeQty { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalSelling { get; set; }
}

/// <summary>The full saved purchase order (header + every line item), returned after a save
/// and used to list order history.</summary>
public class PurchaseOrderDto
{
    public int Id { get; set; }
    public decimal NetAmount { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalQty { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PurchaseBillItemDto> Items { get; set; } = new();
}

/// <summary>Body for POST api/purchasebill - saves every row currently in the UI table as one
/// purchase order in a single call.</summary>
public class SavePurchaseOrderRequest
{
    public List<PurchaseBillItemDto> Items { get; set; } = new();
}

public class LocationDto
{
    public string Location_Code { get; set; } = string.Empty;
    public string Location_Name { get; set; } = string.Empty;
}