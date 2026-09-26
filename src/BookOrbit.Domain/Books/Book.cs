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

    public void StartReading(DateOnly startDate)
    {
        if (Status != BookStatus.Unread)
        {
            throw new InvalidOperationException(
                "未読の本のみ読書開始できます。");
        }
        
        Status = BookStatus.Reading;
        StartedAt ??= StartedAt;
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
}