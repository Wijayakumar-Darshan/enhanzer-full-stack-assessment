using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchaseBillApi.Models;

[Table("Location_Details")]
public class LocationDetail
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Location_Code { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Location_Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Username { get; set; }

    public DateTime Created_At { get; set; } = DateTime.UtcNow;
}

[Table("Purchase_Bill_Item")]
public class PurchaseBillItem
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Item_Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Batch { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Standard_Cost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Standard_Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Margin { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Qty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Free_Qty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total_Cost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total_Selling { get; set; }

    public DateTime Created_At { get; set; } = DateTime.UtcNow;
}

[Table("Purchase_Order")]
public class PurchaseOrder
{
    public int Id { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Net_Amount { get; set; }

    public DateTime Created_At { get; set; } = DateTime.UtcNow;

    public ICollection<PurchaseOrderItem> Items { get; set; }
        = new List<PurchaseOrderItem>();
}

[Table("Purchase_Order_Item")]
public class PurchaseOrderItem
{
    public int Id { get; set; }

    public int Purchase_Order_Id { get; set; }

    [Required, MaxLength(100)]
    public string Item_Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Batch { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Standard_Cost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Standard_Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Margin { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Qty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Free_Qty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total_Cost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total_Selling { get; set; }

    public DateTime Created_At { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(Purchase_Order_Id))]
    public PurchaseOrder? PurchaseOrder { get; set; }
}


