using System.ComponentModel.DataAnnotations;

namespace PurchaseBillApi.DTOs;

public class CreatePurchaseOrderDto
{
    [MinLength(1)]
    public List<CreatePurchaseOrderItemDto> Items { get; set; }
        = new();
}

public class CreatePurchaseOrderItemDto
{
    [Required, MaxLength(100)]
    public string ItemName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Batch { get; set; } = string.Empty;

    public decimal StandardCost { get; set; }

    public decimal StandardPrice { get; set; }

    public decimal Margin { get; set; }

    public decimal Qty { get; set; }

    public decimal FreeQty { get; set; }

    public decimal Discount { get; set; }
}

public class PurchaseOrderDto
{
    public int Id { get; set; }

    public decimal NetAmount { get; set; }

    public int NoOfItems { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PurchaseOrderItemDto> Items { get; set; }
        = new();
}

public class PurchaseOrderItemDto
{
    public int Id { get; set; }

    public int PurchaseOrderId { get; set; }

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

    public DateTime CreatedAt { get; set; }
}

public class LatestPurchaseOrderDto
{
    public int Id { get; set; }

    public decimal NetAmount { get; set; }

    public int NoOfItems { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class OldestPurchaseOrderItemDto
{
    public int PurchaseOrderId { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public decimal Qty { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class ItemQuantityDto
{
    public string ItemName { get; set; } = string.Empty;

    public decimal Qty { get; set; }
}