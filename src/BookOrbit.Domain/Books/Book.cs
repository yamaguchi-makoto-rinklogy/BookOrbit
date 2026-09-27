using BookOrbit.Domain.Acquisitions;

namespace BookOrbit.Domain.Books;

public class Book
{
    public Guid Id { get; private set; }
    
    public Isbn? Isbn { get; private set; }
    
    public string Title { get; private set; }
    
    public string? Subtitle { get; private set; }
    
    public string? Publisher { get; private set; }

    public DateOnly? PublishedDate { get; private set; }

    public int? PageCount { get; private set; }

    public string? Description { get; private set; }

    public string? ThumbnailUrl { get; private set; }

    public BookStatus Status { get; private set; }

    public Priority Priority { get; private set; }

    public DateOnly? WishlistedAt { get; private set; }

    public DateOnly? StartedAt { get; private set; }

    public DateOnly? FinishedAt { get; private set; }

    private Book()
    {
        // EF Core用
        Title = string.Empty;
    }
    
    public Book(
        string title,
        Isbn? isbn = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentNullException(
                "タイトルは必須です。",
                nameof(title));
        }
        
        Id = Guid.NewGuid();
        Title = title;
        Isbn = isbn;

        Status = BookStatus.Wishlist;
        Priority = Priority.Medium;
        WishlistedAt = DateOnly.FromDateTime(DateTime.Today);
    }

    private readonly List<Acquisition> _acquisitions = [];
    
    public IReadOnlyCollection<Acquisition> Acquisitions 
        => _acquisitions;

    public void StartReading(DateOnly startDate)
    {
        if (Status != BookStatus.Unread)
        {
            throw new InvalidOperationException(
                "未読の本のみ読書開始できます。");
        }
        
        Status = BookStatus.Reading;
        StartedAt ??= startDate;
    }

    public void Pause()
    {
        if (Status != BookStatus.Reading)
        {
            throw new InvalidOperationException(
                "読書中の本のみ中断できます。");
        }
        
        Status = BookStatus.Paused;
    }
    
    public void Resume()
    {
        if (Status != BookStatus.Paused)
        {
            throw new InvalidOperationException(
                "中断中の本のみ再開できます。");
        }
        
        Status = BookStatus.Reading;
    }
    
    public void Complete(DateOnly finishedAt)
    {
        if (Status != BookStatus.Reading)
        {
            throw new InvalidOperationException(
                "読書中の本のみ読了できます。");
        }
        
        Status = BookStatus.Completed;
        FinishedAt = finishedAt;
    }
    
    public void MarkAsPurchased(
        BookFormat format,
        DateOnly purchasedAt,
        decimal? price,
        string? store)
    {
        if (Status != BookStatus.Wishlist)
        {
            throw new InvalidOperationException(
                "Wishlist本のみ購入済みに変更できます。");
        }

        var acquisition = Acquisition.CreatePurchase(
            Id,
            format,
            purchasedAt,
            price,
            store);
        
        _acquisitions.Add(acquisition);
        
        Status = BookStatus.Unread;
    }

    public void MarkAsBorrowed(
        AcquisitionType borrowingType,
        BookFormat format,
        string borrowedFrom,
        DateOnly borrowedAt,
        DateOnly? dueDate)
    {
        if (Status != BookStatus.Wishlist)
        {
            throw new InvalidOperationException(
                "Wishlist本のみ借用済みに変更できます");
        }

        var acquisition = Acquisition.CreateBorrowing(
            Id,
            borrowingType,
            format,
            borrowedFrom,
            borrowedAt,
            dueDate);

        _acquisitions.Add(acquisition);
        
        Status = BookStatus.Unread;
    }

    public void Return(DateOnly returnedAt)
    {
        var borrowing = _acquisitions
            .LastOrDefault(x =>
                x.Type is AcquisitionType.BorrowedLibrary
                    or AcquisitionType.BorrowedPerson
                && x.BorrowingInfo is not null
                && !x.BorrowingInfo.IsReturned);

        if (borrowing is null)
        {
            throw new InvalidOperationException(
                "返却対象の借用情報がありません。");
        }
        
        borrowing.Return(returnedAt);
    }
}