namespace BookOrbit.Domain.Acquisitions;

public class Acquisition
{
    public Guid Id { get; private set; }
    
    public Guid BookId { get; private set; }
    
    public AcquisitionType Type { get; private set; }
    
    public BookFormat Format { get; private set; }
    
    public DateOnly AcquiredAt { get; private set; }
    
    public PurchaseInfo? PurchaseInfo { get; private set; }
    
    public BorrowingInfo? BorrowingInfo { get; private set; }

    private Acquisition()
    {
    }

    private Acquisition(
        Guid bookId,
        AcquisitionType type,
        BookFormat format,
        DateOnly acquiredAt)
    {
        Id = Guid.NewGuid();
        BookId = bookId;
        Type = type;
        Format = format;
        AcquiredAt = acquiredAt;
    }

    public static Acquisition CreatePurchase(
        Guid bookId,
        BookFormat format,
        DateOnly purchasedAt,
        decimal? price,
        string? store)
    {
        var acquisition = new Acquisition(
            bookId,
            AcquisitionType.Purchased,
            format,
            purchasedAt);

        acquisition.PurchaseInfo = new PurchaseInfo(
            purchasedAt,
            price,
            store);
        
        return acquisition;
    }

    public static Acquisition CreateBorrowing(
        Guid bookId,
        AcquisitionType borrowingType,
        BookFormat format,
        string borrowedFrom,
        DateOnly borrowedAt,
        DateOnly? dueDate
    )
    {
        if (borrowingType is not AcquisitionType.BorrowedLibrary
            and not AcquisitionType.BorrowedPerson)
        {
            throw new ArgumentException(
                "借用種別はBorrowedLibraryまたはBorrowedPersonである必要があります。",
                nameof(borrowingType));
        }

        var acquisition = new Acquisition(
            bookId,
            borrowingType,
            format,
            borrowedAt);

        acquisition.BorrowingInfo = new BorrowingInfo(
            borrowedFrom,
            borrowedAt,
            dueDate);
        
        return acquisition;
    }

    public void Return(DateOnly returnedAt)
    {
        if (BorrowingInfo is null)
        {
            throw new InvalidOperationException(
                "借用した本ではなないため返却できません");
        }
        
        BorrowingInfo.Return(returnedAt);
    }
}