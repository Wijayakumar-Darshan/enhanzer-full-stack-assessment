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

public class LocationDto
{
    public string Location_Code { get; set; } = string.Empty;
    public string Location_Name { get; set; } = string.Empty;
}
