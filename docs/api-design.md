# API設計

## 1. 基本方針

Base URL:

```text
/api
```

Content-Type:

```http
Content-Type: application/json
```

内部IDはUUIDを使用する。

日付は `YYYY-MM-DD`、日時はISO 8601形式を使用する。

---

## 2. 共通HTTPステータス

| Status | 用途 |
|---|---|
| 200 | 正常取得・更新 |
| 201 | 新規登録成功 |
| 204 | 削除成功 |
| 400 | 入力値不正 |
| 404 | 対象なし |
| 409 | 状態競合 |
| 422 | ビジネスルール違反 |
| 500 | サーバー内部エラー |
| 502 | 外部APIエラー |

---

## 3. 共通エラーレスポンス

```json
{
  "code": "BOOK_NOT_FOUND",
  "message": "指定された書籍が存在しません。",
  "details": null
}
```

---

## 4. Books

### 一覧取得

```http
GET /api/books
```

Query:

- keyword
- status
- priority
- acquisitionType
- format
- tag
- borrowedOnly
- overdueOnly
- sort
- order
- page
- pageSize

### 詳細取得

```http
GET /api/books/{bookId}
```

### 新規登録

```http
POST /api/books
```

Request:

```json
{
  "isbn": "9784873115658",
  "title": "リーダブルコード",
  "subtitle": null,
  "authors": [
    "Dustin Boswell",
    "Trevor Foucher"
  ],
  "publisher": "オライリー・ジャパン",
  "publishedDate": "2012-06-23",
  "pageCount": 260,
  "description": null,
  "thumbnailUrl": null
}
```

### 更新

```http
PUT /api/books/{bookId}
```

### 削除

```http
DELETE /api/books/{bookId}
```

---

## 5. ISBN検索

```http
GET /api/books/search/isbn/{isbn}
```

Response:

```json
{
  "isbn": "9784873115658",
  "title": "リーダブルコード",
  "authors": [
    "Dustin Boswell",
    "Trevor Foucher"
  ],
  "publisher": "オライリー・ジャパン",
  "publishedDate": "2012-06-23",
  "pageCount": 260,
  "description": "...",
  "categories": [
    "Computers"
  ],
  "thumbnailUrl": "https://example.com/book.jpg",
  "source": "GoogleBooks"
}
```

---

## 6. Wishlist

### 追加

```http
POST /api/wishlist
```

Request:

```json
{
  "bookId": "550e8400-e29b-41d4-a716-446655440000",
  "priority": "High",
  "discoverySource": "Bookstore",
  "memo": "次に読みたい"
}
```

### 一覧

```http
GET /api/wishlist
```

### 削除

```http
DELETE /api/wishlist/{bookId}
```

Book自体は削除しない。

---

## 7. 購入

```http
POST /api/books/{bookId}/purchase
```

Request:

```json
{
  "purchasedAt": "2026-09-20",
  "price": 2420,
  "store": "ジュンク堂",
  "bookFormat": "Paper"
}
```

処理:

```text
Wishlist
↓
Purchase登録
↓
Acquisition作成
↓
Status = Unread
```

---

## 8. 借用

```http
POST /api/books/{bookId}/borrow
```

Request:

```json
{
  "borrowingType": "BorrowedLibrary",
  "borrowedFrom": "名古屋市図書館",
  "borrowedAt": "2026-09-20",
  "dueDate": "2026-10-04",
  "bookFormat": "Paper"
}
```

`dueDate` は任意。

---

## 9. 借用中一覧

```http
GET /api/books/borrowed
```

例:

```http
GET /api/books/borrowed?dueWithinDays=3
```

期限超過:

```http
GET /api/books/borrowed?status=overdue
```

---

## 10. 返却

```http
POST /api/books/{bookId}/return
```

Request:

```json
{
  "returnedAt": "2026-09-20"
}
```

---

## 11. 読書状態

### 開始

```http
POST /api/books/{bookId}/start-reading
```

### 一時停止

```http
POST /api/books/{bookId}/pause
```

### 再開

```http
POST /api/books/{bookId}/resume
```

### 読了

```http
POST /api/books/{bookId}/complete
```

---

## 12. 読書ログ

### 登録

```http
POST /api/books/{bookId}/reading-logs
```

Request:

```json
{
  "readingDate": "2026-09-20",
  "minutes": 45,
  "startPage": 120,
  "endPage": 155,
  "memo": "依存関係逆転の章まで"
}
```

### 一覧

```http
GET /api/books/{bookId}/reading-logs
```

---

## 13. ダッシュボード

```http
GET /api/dashboard
```

Response例:

```json
{
  "wishlistCount": 18,
  "unreadCount": 27,
  "readingCount": 3,
  "borrowedCount": 4,
  "dueSoonCount": 2,
  "overdueCount": 1,
  "completedThisMonth": 5
}
```

---

## 14. 統計

```http
GET /api/statistics
```

Query:

- year
- month

---

## 15. AIレコメンド

```http
GET /api/recommendations/from-book/{bookId}
```

Query:

- limit

処理:

```text
Book取得
↓
LLMで特徴抽出
↓
検索キーワード生成
↓
Books API検索
↓
候補取得
↓
自前Book DBと照合
↓
Completed除外
↓
LLMでランキング・理由生成
↓
Response
```

Response例:

```json
{
  "sourceBook": {
    "bookId": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Clean Architecture"
  },
  "recommendations": [
    {
      "isbn": "978xxxxxxxxxx",
      "title": "A Philosophy of Software Design",
      "authors": ["John Ousterhout"],
      "score": 92,
      "reason": "現在読んでいる本で扱われる設計原則を、より具体的な設計判断へ発展させられるためです。",
      "libraryStatus": "NotRegistered"
    }
  ]
}
```

---

## 16. ルールベース推薦

### 次に読む本

```http
GET /api/recommendations/next-to-read
```

対象: `Unread`

### 次に買う本

```http
GET /api/recommendations/next-to-buy
```

対象: `Wishlist`

---

## 17. API一覧

```text
/api

├─ books
│   ├─ GET    /
│   ├─ POST   /
│   ├─ GET    /{bookId}
│   ├─ PUT    /{bookId}
│   ├─ DELETE /{bookId}
│   ├─ GET    /search/isbn/{isbn}
│   ├─ POST   /{bookId}/purchase
│   ├─ POST   /{bookId}/borrow
│   ├─ POST   /{bookId}/return
│   ├─ POST   /{bookId}/start-reading
│   ├─ POST   /{bookId}/pause
│   ├─ POST   /{bookId}/resume
│   ├─ POST   /{bookId}/complete
│   ├─ GET    /{bookId}/reading-logs
│   └─ POST   /{bookId}/reading-logs
│
├─ wishlist
│   ├─ GET    /
│   ├─ POST   /
│   └─ DELETE /{bookId}
│
├─ recommendations
│   ├─ GET /from-book/{bookId}
│   ├─ GET /next-to-read
│   └─ GET /next-to-buy
│
├─ dashboard
│   └─ GET /
│
├─ statistics
│   └─ GET /
│
└─ tags
    └─ GET /
```
