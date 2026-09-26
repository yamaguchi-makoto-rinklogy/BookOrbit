# アーキテクチャ設計

## 1. 方針

学習用途を考慮し、過剰に複雑化しないClean Architecture寄りの構成とする。

```text
BookOrbit.sln

src/
├─ BookOrbit.Api
├─ BookOrbit.Application
├─ BookOrbit.Domain
└─ BookOrbit.Infrastructure

tests/
├─ BookOrbit.Domain.Tests
├─ BookOrbit.Application.Tests
└─ BookOrbit.Api.Tests
```

---

## 2. 依存関係

```text
BookOrbit.Api
   │
   ├──────────────→ BookOrbit.Application
   │
   └──────────────→ BookOrbit.Infrastructure
                         │
                         ▼
BookOrbit.Application ──→ BookOrbit.Domain
                         ▲
                         │
BookOrbit.Infrastructure ┘
```

Domainは最内層であり、外部技術に依存しない。

---

## 3. BookOrbit.Domain

```text
BookOrbit.Domain/

├─ Books/
│   ├─ Book.cs
│   ├─ BookStatus.cs
│   ├─ Priority.cs
│   └─ Isbn.cs
│
├─ Acquisitions/
│   ├─ Acquisition.cs
│   ├─ AcquisitionType.cs
│   ├─ BookFormat.cs
│   ├─ PurchaseInfo.cs
│   └─ BorrowingInfo.cs
│
├─ Reading/
│   └─ ReadingLog.cs
│
├─ Exceptions/
│   ├─ DomainException.cs
│   └─ InvalidBookStatusException.cs
│
└─ BookOrbit.Domain.csproj
```

---

## 4. BookOrbit.Application

```text
BookOrbit.Application/

├─ Books/
│   ├─ CreateBook/
│   ├─ GetBook/
│   ├─ GetBooks/
│   ├─ UpdateBook/
│   └─ DeleteBook/
│
├─ Wishlist/
│   ├─ AddToWishlist/
│   └─ RemoveFromWishlist/
│
├─ Acquisitions/
│   ├─ PurchaseBook/
│   ├─ BorrowBook/
│   └─ ReturnBook/
│
├─ Reading/
│   ├─ StartReading/
│   ├─ PauseReading/
│   ├─ ResumeReading/
│   ├─ CompleteReading/
│   └─ AddReadingLog/
│
├─ ExternalBooks/
│   └─ SearchBookByIsbn/
│
├─ Recommendations/
│   └─ GetRecommendationsFromBook/
│
├─ Abstractions/
│   ├─ Persistence/
│   │   ├─ IBookRepository.cs
│   │   └─ IUnitOfWork.cs
│   │
│   └─ ExternalServices/
│       ├─ IBookSearchService.cs
│       └─ IBookRecommendationService.cs
│
└─ BookOrbit.Application.csproj
```

---

## 5. BookOrbit.Infrastructure

```text
BookOrbit.Infrastructure/

├─ Persistence/
│   ├─ BookOrbitDbContext.cs
│   ├─ Configurations/
│   │   ├─ BookConfiguration.cs
│   │   ├─ AcquisitionConfiguration.cs
│   │   ├─ BorrowingInfoConfiguration.cs
│   │   ├─ PurchaseInfoConfiguration.cs
│   │   └─ ReadingLogConfiguration.cs
│   ├─ Repositories/
│   │   └─ BookRepository.cs
│   └─ Migrations/
│
├─ ExternalBooks/
│   ├─ GoogleBooksClient.cs
│   ├─ GoogleBooksOptions.cs
│   └─ Models/
│       └─ GoogleBooksResponse.cs
│
├─ Recommendations/
│   ├─ LlmBookRecommendationService.cs
│   ├─ LlmOptions.cs
│   └─ Models/
│       └─ RecommendationResponse.cs
│
├─ DependencyInjection.cs
│
└─ BookOrbit.Infrastructure.csproj
```

---

## 6. BookOrbit.Api

```text
BookOrbit.Api/

├─ Controllers/
│   ├─ BooksController.cs
│   ├─ WishlistController.cs
│   ├─ ReadingController.cs
│   ├─ RecommendationsController.cs
│   ├─ DashboardController.cs
│   └─ StatisticsController.cs
│
├─ Contracts/
│   ├─ Books/
│   ├─ Acquisitions/
│   └─ Reading/
│
├─ Middleware/
│   └─ ExceptionHandlingMiddleware.cs
│
├─ Program.cs
├─ appsettings.json
└─ BookOrbit.Api.csproj
```

Controllerは薄く保つ。

避けること:

- ControllerからEF Coreを直接呼ぶ
- ControllerからGoogle Books APIを直接呼ぶ
- ControllerからLLMを直接呼ぶ
- Controllerに複雑なビジネスルールを書く

---

## 7. 外部サービス

```text
Application
    │
    ├─ IBookRepository
    ├─ IBookSearchService
    └─ IBookRecommendationService
            │
            ▼
Infrastructure
    ├─ BookRepository
    │      └─ EF Core / PostgreSQL
    │
    ├─ GoogleBooksClient
    │      └─ Google Books API
    │
    └─ LlmBookRecommendationService
           └─ LLM API
```

---

## 8. DI

`Program.cs`:

```csharp
builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);
```

Infrastructure:

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BookOrbitDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Database")));

        services.AddScoped<IBookRepository, BookRepository>();

        services.AddHttpClient<
            IBookSearchService,
            GoogleBooksClient>();

        services.AddScoped<
            IBookRecommendationService,
            LlmBookRecommendationService>();

        return services;
    }
}
```

---

## 9. NuGet方針

### Domain

原則追加なし。

### Infrastructure

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.Extensions.Http

### API

- Microsoft.AspNetCore.OpenApi
- Swashbuckle.AspNetCore

### Test

- xunit
- FluentAssertions
- Microsoft.NET.Test.Sdk

将来的に:

- Testcontainers.PostgreSql

---

## 10. 名前空間

```text
BookOrbit.Domain.Books
BookOrbit.Domain.Acquisitions
BookOrbit.Domain.Reading

BookOrbit.Application.Books.CreateBook
BookOrbit.Application.Acquisitions.BorrowBook
BookOrbit.Application.Recommendations

BookOrbit.Infrastructure.Persistence
BookOrbit.Infrastructure.ExternalBooks

BookOrbit.Api.Controllers
```
