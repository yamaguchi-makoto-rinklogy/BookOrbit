# ドメイン設計

## 1. 方針

中心となるAggregate Rootは `Book` とする。

Domain層にはビジネスルールのみを置き、以下への依存を持たせない。

- ASP.NET Core
- EF Core
- PostgreSQL
- Google Books API
- LLM API
- HttpClient

---

## 2. Entity一覧

### Book

Aggregate Root。

主な責務:

- 読書状態の保持
- 読書状態遷移
- 入手情報との関連
- 読書ログとの関連

代表メソッド:

```csharp
book.StartReading(startedAt);
book.Pause();
book.Resume();
book.Complete(finishedAt);
```

---

### Acquisition

本をどのように入手したかを表す。

主な情報:

- BookId
- AcquisitionType
- BookFormat
- AcquiredAt
- PurchaseInfo
- BorrowingInfo

---

### PurchaseInfo

購入した本の詳細。

- PurchasedAt
- Price
- Store

---

### BorrowingInfo

借用した本の詳細。

- BorrowedFrom
- BorrowedAt
- DueDate
- ReturnedAt

代表メソッド:

```csharp
borrowing.Return(returnedAt);
```

代表判定:

```csharp
borrowing.IsReturned
borrowing.IsOverdue(today)
borrowing.IsDueSoon(today, 3)
```

---

### ReadingLog

個々の読書記録。

- ReadingDate
- Minutes
- StartPage
- EndPage
- Memo

ビジネスルール:

- Minutes > 0
- EndPage >= StartPage

---

## 3. Enum

### BookStatus

```csharp
public enum BookStatus
{
    Wishlist,
    Unread,
    Reading,
    Paused,
    Completed
}
```

### Priority

```csharp
public enum Priority
{
    Low,
    Medium,
    High
}
```

### AcquisitionType

```csharp
public enum AcquisitionType
{
    Purchased,
    BorrowedLibrary,
    BorrowedPerson,
    Gift,
    Other
}
```

### BookFormat

```csharp
public enum BookFormat
{
    Paper,
    Kindle,
    OtherEBook,
    Audiobook
}
```

---

## 4. Value Object

### Isbn

ISBNはValue Objectとして扱う。

最低限以下を担当する。

- ハイフン除去
- 空白除去
- 10桁 / 13桁の形式確認

将来的にはチェックデジット検証も追加する。

例:

```csharp
public sealed record Isbn
{
    public string Value { get; }

    public Isbn(string value)
    {
        var normalized = value
            .Replace("-", "")
            .Replace(" ", "");

        if (normalized.Length is not 10 and not 13)
        {
            throw new ArgumentException(
                "ISBNは10桁または13桁で指定してください。");
        }

        Value = normalized;
    }

    public override string ToString() => Value;
}
```

---

## 5. 状態遷移ルール

### 読書開始

```text
Unread → Reading
```

それ以外からの開始は許可しない。

### 一時停止

```text
Reading → Paused
```

### 再開

```text
Paused → Reading
```

### 読了

```text
Reading → Completed
```

### 購入・借用

Wishlistまたは未入手Bookに対して入手情報を追加し、読書状態をUnreadへ移す。

---

## 6. Application UseCase

```text
Books
├─ CreateBook
├─ GetBook
├─ GetBooks
├─ UpdateBook
└─ DeleteBook

Wishlist
├─ AddToWishlist
└─ RemoveFromWishlist

Acquisitions
├─ PurchaseBook
├─ BorrowBook
└─ ReturnBook

Reading
├─ StartReading
├─ PauseReading
├─ ResumeReading
├─ CompleteReading
└─ AddReadingLog

ExternalBooks
└─ SearchBookByIsbn

Recommendations
└─ GetRecommendationsFromBook
```

---

## 7. Application Interface

### IBookRepository

```csharp
public interface IBookRepository
{
    Task<Book?> FindByIdAsync(
        Guid bookId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Book book,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}
```

### IBookSearchService

```csharp
public interface IBookSearchService
{
    Task<ExternalBook?> SearchByIsbnAsync(
        string isbn,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ExternalBook>> SearchAsync(
        IReadOnlyList<string> keywords,
        CancellationToken cancellationToken);
}
```

### IBookRecommendationService

```csharp
public interface IBookRecommendationService
{
    Task<IReadOnlyList<BookRecommendation>> RecommendAsync(
        Book sourceBook,
        IReadOnlyList<ExternalBook> candidates,
        CancellationToken cancellationToken);
}
```

---

## 8. AI推薦の責務分離

Application:

- 推薦ユースケースのオーケストレーション
- Book取得
- 候補の重複除外
- LibraryStatus付与

Infrastructure:

- Google Books API呼び出し
- LLM API呼び出し

Domain:

- AIや外部APIを認識しない
