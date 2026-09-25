


public interface IPurchaseOrderService
{
    Task<PurchaseOrderDto> CreateAsync(
        CreatePurchaseOrderDto request);

    Task<List<LatestPurchaseOrderDto>>
        GetLatestAsync(int count);

    Task<List<OldestPurchaseOrderItemDto>>
        GetOldestItemsAsync(int count);

    Task<List<ItemQuantityDto>>
        GetItemQuantitiesAsync();
}

