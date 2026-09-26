# ER図

## Mermaid

```mermaid
erDiagram

    BOOKS {
        uuid id PK
        varchar isbn UK
        varchar title
        varchar subtitle
        varchar author
        varchar publisher
        date published_date
        int page_count
        text description
        varchar thumbnail_url
        timestamp created_at
        timestamp updated_at
    }

    WISHLISTS {
        uuid id PK
        uuid book_id FK
        varchar priority
        varchar discovery_source
        text memo
        date wishlisted_at
        timestamp created_at
        timestamp updated_at
    }

    ACQUISITIONS {
        uuid id PK
        uuid book_id FK
        varchar acquisition_type
        varchar book_format
        date acquired_at
        timestamp created_at
        timestamp updated_at
    }

    PURCHASE_INFOS {
        uuid id PK
        uuid acquisition_id FK
        date purchased_at
        decimal price
        varchar store
        timestamp created_at
        timestamp updated_at
    }

    BORROWING_INFOS {
        uuid id PK
        uuid acquisition_id FK
        varchar borrowed_from
        date borrowed_at
        date due_date
        date returned_at
        timestamp created_at
        timestamp updated_at
    }

    READING_LOGS {
        uuid id PK
        uuid book_id FK
        date reading_date
        int minutes
        int start_page
        int end_page
        text memo
        timestamp created_at
        timestamp updated_at
    }

    READING_STATUS_HISTORIES {
        uuid id PK
        uuid book_id FK
        varchar status
        date started_at
        date ended_at
        text note
        timestamp created_at
    }

    RECOMMENDATIONS {
        uuid id PK
        uuid source_book_id FK
        varchar recommended_isbn
        varchar title
        varchar author
        varchar thumbnail_url
        int score
        text reason
        varchar library_status
        timestamp generated_at
    }

    TAGS {
        uuid id PK
        varchar name
        varchar type
        timestamp created_at
        timestamp updated_at
    }

    BOOK_TAGS {
        uuid book_id PK, FK
        uuid tag_id PK, FK
    }

    BOOKS ||--o| WISHLISTS : "wishlist"
    BOOKS ||--o{ ACQUISITIONS : "acquired"
    ACQUISITIONS ||--o| PURCHASE_INFOS : "purchase"
    ACQUISITIONS ||--o| BORROWING_INFOS : "borrow"
    BOOKS ||--o{ READING_LOGS : "reading logs"
    BOOKS ||--o{ READING_STATUS_HISTORIES : "status history"
    BOOKS ||--o{ RECOMMENDATIONS : "source book"
    BOOKS ||--o{ BOOK_TAGS : "has"
    TAGS ||--o{ BOOK_TAGS : "assigned"
```

## 補足

### BOOKS

書籍そのものの基本情報を管理する。

### WISHLISTS

「読みたい」というユーザーとの関係を管理する。

### ACQUISITIONS

本をどのように入手したかを管理する。

MVPでは基本1冊1入手でもよいが、将来的な紙 + Kindleなどの複数入手に対応できるよう1:Nで設計する。

### PURCHASE_INFOS

購入した場合の詳細情報。

### BORROWING_INFOS

借用時の詳細情報。返却期限・返却日もここで管理する。

### READING_LOGS

個々の読書記録。

### READING_STATUS_HISTORIES

状態遷移履歴。MVPで必須ではないが、将来的な分析に利用可能。

### RECOMMENDATIONS

AI推薦結果。MVPでは永続化せずオンデマンド生成でもよい。

### TAGS / BOOK_TAGS

ジャンル・テーマ・キーワードを柔軟に管理する。

## Enum

### BookStatus

- Wishlist
- Unread
- Reading
- Paused
- Completed

### Priority

- Low
- Medium
- High

### AcquisitionType

- Purchased
- BorrowedLibrary
- BorrowedPerson
- Gift
- Other

### BookFormat

- Paper
- Kindle
- OtherEBook
- Audiobook

### LibraryStatus

- NotRegistered
- Wishlisted
- Unread
- Reading
- Completed
