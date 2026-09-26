# 開発ロードマップ

## 1. 方針

最初から全機能を作らず、縦に薄く完成させてから拡張する。

---

## Phase 0: Solution作成

作成対象:

```text
BookOrbit.sln

src/
├─ BookOrbit.Api
├─ BookOrbit.Application
├─ BookOrbit.Domain
└─ BookOrbit.Infrastructure

tests/
└─ BookOrbit.Domain.Tests
```

完了条件:

- 各ProjectがBuildできる
- Project Referenceが正しい
- APIが起動する

---

## Phase 1: Book CRUD

実装:

- Book Entity
- BookStatus
- Priority
- Isbn
- BookOrbitDbContext
- BookConfiguration
- BookRepository
- CreateBook
- GetBook
- GetBooks
- UpdateBook
- DeleteBook
- BooksController

API:

```text
GET    /api/books
GET    /api/books/{id}
POST   /api/books
PUT    /api/books/{id}
DELETE /api/books/{id}
```

完了条件:

- PostgreSQLへBookを登録できる
- 一覧・詳細取得できる
- 更新・削除できる

---

## Phase 2: Wishlist / 入手管理

実装:

- Wishlist
- Acquisition
- PurchaseInfo
- BorrowingInfo
- PurchaseBook
- BorrowBook
- ReturnBook

API:

```text
POST /api/wishlist
POST /api/books/{id}/purchase
POST /api/books/{id}/borrow
POST /api/books/{id}/return
```

完了条件:

- Wishlist登録
- 購入登録
- 借用登録
- 返却登録
- 返却期限超過判定

---

## Phase 3: 読書状態

実装:

- StartReading
- PauseReading
- ResumeReading
- CompleteReading
- Domain Tests

状態遷移:

```text
Unread → Reading
Reading → Paused
Paused → Reading
Reading → Completed
```

完了条件:

- 不正な状態遷移をDomainで拒否できる
- 正常な状態遷移をAPIから実行できる

---

## Phase 4: Google Books API

実装:

- IBookSearchService
- GoogleBooksClient
- Google Books Response DTO
- SearchBookByIsbn

API:

```text
GET /api/books/search/isbn/{isbn}
```

完了条件:

- ISBNで外部書籍情報を取得できる
- 外部API失敗を適切に扱える
- APIレスポンスをDomainへ直接漏らさない

---

## Phase 5: AI Recommendation

実装:

- IBookRecommendationService
- LLM連携
- 特徴抽出
- 検索キーワード生成
- Books API候補検索
- 自前DBとの照合
- AIランキング
- 推薦理由生成

API:

```text
GET /api/recommendations/from-book/{bookId}
```

完了条件:

- ReadingまたはCompletedの本から推薦できる
- Completed済み書籍を除外できる
- Wishlisted / Unread / Readingの状態を返せる
- 推薦理由を返せる

---

## Phase 6: ReadingLog

実装:

- ReadingLog Entity
- AddReadingLog
- GetReadingLogs

API:

```text
POST /api/books/{bookId}/reading-logs
GET  /api/books/{bookId}/reading-logs
```

完了条件:

- 読書時間
- ページ範囲
- メモ

を記録できる。

---

## Phase 7: Dashboard / Statistics

実装:

```text
GET /api/dashboard
GET /api/statistics
```

集計:

- Wishlist冊数
- 積読冊数
- 読書中冊数
- 借用中冊数
- 返却期限間近
- 返却期限超過
- 月間読了冊数
- 年間読書時間

---

## Phase 8: Test強化

追加:

- Application Tests
- API Integration Tests
- Testcontainers.PostgreSql

重点テスト:

```text
Unread → Reading は成功
Wishlist → Reading は失敗
Reading → Completed は成功
Completed → Completed は失敗
返却済み → Return は失敗
返却期限超過判定
ISBN重複
AI推薦でCompleted除外
```

---

## MVP完成条件

以下のシナリオがAPIだけで完結すること。

```text
ISBN検索
↓
Wishlist登録
↓
購入または借用
↓
積読
↓
読書開始
↓
読書ログ
↓
AIおすすめ取得
↓
おすすめをWishlistへ登録
↓
元の本を読了
↓
借用本なら返却
↓
統計確認
```

---

## MVP後の候補

- 認証
- 複数ユーザー
- 通知
- 返却期限リマインド
- Kindle連携
- Amazon購入履歴連携
- AIチャット
- 推薦フィードバック
- 「興味なし」学習
- 月次読書レポート
