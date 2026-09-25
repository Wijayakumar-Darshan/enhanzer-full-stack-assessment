namespace PurchaseBillApi.DTOs;

/// <summary>Widget 1 (Table View): ID, Net Amount, No. of Items for the latest 5 orders.</summary>
public class LatestOrderWidgetDto
{
    public int Id { get; set; }
    public decimal NetAmount { get; set; }
    public int NoOfItems { get; set; }
}

/// <summary>Widget 2 (List View): Purchase Order ID, Item Name, No. of Quantity for the
/// oldest 10 purchase order items.</summary>
public class OldestItemWidgetDto
{
    public int PurchaseOrderId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

/// <summary>Widget 3 (Donut Chart): every item grouped by Item Name with its total quantity.</summary>
public class ItemGroupWidgetDto
{
    public string ItemName { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
}