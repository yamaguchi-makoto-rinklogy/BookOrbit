# BookOrbit Design Docs

積読・読書管理アプリの設計ドキュメント一式です。

## Documents

- [要件定義](./requirements.md)
- [ER図](./er-diagram.md)
- [API設計](./api-design.md)
- [ドメイン設計](./domain-design.md)
- [アーキテクチャ設計](./architecture.md)
- [開発ロードマップ](./development-roadmap.md)

## 想定技術スタック

- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Google Books API
- LLM API
- xUnit
- OpenAPI / Swagger
- Docker（将来）
- Testcontainers（将来）

## システムコンセプト

> 「読みたい」を集め、「積読」を整理し、「読んだ」を残し、「次の一冊」につなげる。

```text
本を知る
   ↓
Wishlist
   ↓
購入 / 借用
   ↓
積読
   ↓
読書
   ↓
読了
   ↓
AIによる次の本の提案
   ↓
Wishlist
   ↓
...
```
