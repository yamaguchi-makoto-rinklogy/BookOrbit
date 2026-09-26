namespace BookOrbit.Domain.Acquisitions;

public class PurchaseInfo
{
    public DateOnly PurchasedAt { get; private set; }
    
    public decimal? Price { get; private set; }
    
    public string? Store { get; private set; }

    private PurchaseInfo()
    {
    }

    public PurchaseInfo(
        DateOnly purchasedAt,
        decimal? price,
        string? store)
    {
        if (price < 0)
        {
            throw new ArgumentException(
                "購入価格は0円以上である必要があります。",
                nameof(Price));
        }
        
        PurchasedAt = purchasedAt;
        Price = price;
        Store = store;
    }
}