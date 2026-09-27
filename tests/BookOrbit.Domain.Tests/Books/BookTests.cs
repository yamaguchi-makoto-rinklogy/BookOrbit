using BookOrbit.Domain.Acquisitions;
using BookOrbit.Domain.Books;
using FluentAssertions;

namespace BookOrbit.Domain.Tests.Books;

public class BookTests
{
    [Fact]
    public void NewBook_ShouldBeWishlist()
    {
        var book = new Book("テスト本");

        book.Status.Should().Be(BookStatus.Wishlist);
        book.Priority.Should().Be(Priority.Medium);
        book.WishlistedAt.Should().NotBeNull();
    }

    [Fact]
    public void StartReading_FromUnread_ShouldChangeStatusToReading()
    {
        var book = new Book("テスト本");
        
        //　今は購入処理がないので、後でこのテストは調整する
    }

    [Fact]
    public void StartReading_FromWishlist_ShouldThrowException()
    {
        var book = new Book("テスト本");

        var act = () =>
            book.StartReading(new DateOnly(2026, 9, 26));
        
        act.Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public void MarkAsPurchased_FromWishlist_ShouldChangeUnread()
    {
        var book = new Book("テスト本");
        
        book.MarkAsPurchased(
            BookFormat.Paper,
            new DateOnly(2026, 9, 26),
            2000,
            "佐藤書店");
        
        Assert.Equal(BookStatus.Unread, book.Status);
        Assert.Single(book.Acquisitions);
    }

    [Fact]
    public void MarkAsBorrowed_FromWishlist_ShouldChangeUnread()
    {
        var book = new Book("テスト本");
        
        book.MarkAsBorrowed(
            AcquisitionType.BorrowedLibrary,
            BookFormat.Paper,
            "市立図書館",
            new DateOnly(2026, 9, 26),
            new DateOnly(2026, 10, 10));
        
        Assert.Equal(BookStatus.Unread, book.Status);
        Assert.Single(book.Acquisitions);
        
        var acquisition = book.Acquisitions.Single();
        
        Assert.Equal(
            AcquisitionType.BorrowedLibrary,
            acquisition.Type);

        Assert.NotNull(acquisition.BorrowingInfo);
        Assert.Equal(
            "市立図書館",acquisition.BorrowingInfo.BorrowedFrom);
    }

    [Fact]
    public void Return_BorrowedBook_ShouldSetReturnedAt()
    {
        var book = new Book("テスト本");
            
        book.MarkAsBorrowed(
            AcquisitionType.BorrowedLibrary,
            BookFormat.Paper,
            "市立図書館",
            new DateOnly(2026, 9, 26),
            new DateOnly(2026, 10, 10));
        
        var returnedAt = new DateOnly(2026, 10, 5);
        
        book.Return(returnedAt);
        
        var acquisition = book.Acquisitions.Single();
        
        Assert.NotNull(acquisition.BorrowingInfo);
        Assert.True(acquisition.BorrowingInfo.IsReturned);
        Assert.Equal(
            returnedAt,acquisition.BorrowingInfo.ReturnedAt);
    }
    
    [Fact]
    public void Return_AlreadyReturnedBook_ShouldThrowException()
    {
        var book = new Book("テスト本");

        book.MarkAsBorrowed(
            AcquisitionType.BorrowedLibrary,
            BookFormat.Paper,
            "市立図書館",
            new DateOnly(2026, 9, 27),
            new DateOnly(2026, 10, 11));

        book.Return(new DateOnly(2026, 10, 5));

        Assert.Throws<InvalidOperationException>(() =>
            book.Return(new DateOnly(2026, 10, 6)));
    }

    [Fact]
    public void Return_BeforeBorrowedAt_ShouldThrowException()
    {
        var book = new Book("テスト本");

        book.MarkAsBorrowed(
            AcquisitionType.BorrowedPerson,
            BookFormat.Paper,
            "友人",
            new DateOnly(2026, 9, 27),
            null);

        Assert.Throws<ArgumentException>(() =>
            book.Return(new DateOnly(2026, 9, 26)));
    }
}