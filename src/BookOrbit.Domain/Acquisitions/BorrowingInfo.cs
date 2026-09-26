namespace BookOrbit.Domain.Acquisitions;

public class BorrowingInfo
{
    public string BorrowedFrom { get; private set; }
    
    public DateOnly BorrowedAt { get; private set; }
    
    public DateOnly? DueDate { get; private set; }
    
    public DateOnly? ReturnedAt { get; private set; }

    public bool IsReturned =>
        ReturnedAt.HasValue;

    private BorrowingInfo()
    {
        BorrowedFrom = string.Empty;
    }

    public BorrowingInfo(
        string borrowedFrom,
        DateOnly borrowedAt,
        DateOnly dueDate
    )
    {
        if (string.IsNullOrWhiteSpace(borrowedFrom))
        {
            throw new ArgumentNullException(
                "借り先は必須です。",
                nameof(borrowedFrom));
        }
        
        if (dueDate < borrowedAt)
        {
            throw new ArgumentException(
                "返却期限は借用日以降である必要があります。",
                nameof(dueDate));
        }
        
        BorrowedFrom = borrowedFrom;
        BorrowedAt = borrowedAt;
        DueDate = dueDate;
    }

    public bool IsOverdue(DateOnly today)
    {
        return 
            !IsReturned &&
            DueDate.HasValue &&
            DueDate.Value < today;
    }

    public bool IsDueSoon(
        DateOnly today,
        int days = 3)
    {
        if (IsReturned || !DueDate.HasValue)
        {
            return false;
        }

        var remainingDays =
            DueDate.Value.DayNumber - today.DayNumber;
        
        return remainingDays >= 0 && 
               remainingDays <= days;
    }

    public void Return(DateOnly returnedAt)
    {
        if (IsReturned)
        {
            throw new InvalidOperationException(
                "この本はすでに返却済みです。");
        }

        if (returnedAt < BorrowedAt)
        {
            throw new ArgumentException(
                "返却日は借用日以降である必要があります。",
                nameof(returnedAt));
        }
        
        ReturnedAt = returnedAt;
    }
}