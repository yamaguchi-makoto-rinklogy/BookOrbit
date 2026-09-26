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
}