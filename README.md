# Light.Contracts

A lightweight, zero-dependency Result Pattern library for .NET Standard 2.0+ / C# 7.3+.

Provides a consistent, predictable way to return success/failure from services and APIs — **without hidden exceptions from implicit operators**.

---

## Features

- **Result Pattern** — `Result`, `Result<T>`, `PagedResult<T>`
- **Smart ResultCode** — class-based enum with `Name`, `HttpStatus`, `IsSuccess`
- **Zero dependency** — no JSON library required; `Status` field auto-excluded from serialization
- **No hidden throws** — all implicit operators are safe (return null/default instead of throwing)
- **Paging built-in** — `Paged<T>`, `PagedResult<T>`, `ToPaged()`, `ToPagedResult()`
- **Serialization-friendly** — clean JSON output, `Code` setter supports deserialization
- **.NET Standard 2.0** — compatible with .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5+

---

## Installation

```
dotnet add package Light.Contracts
```

---

## Quick Start

```csharp
using Light.Contracts;
using Light.Extensions;

// Success with data
Result<User> result = Result<User>.Success(user);

// Error without data
Result<User> result = Result<User>.NotFound("User not found.");

// Implicit conversion — no need for Result<T>.Success()
Result<string> result = "hello";      // → Success
Result<string> result = (string)null; // → Error (no throw)

// Extract data — developer checks IsSuccess first
if (result.IsSuccess)
{
    User user = result;  // implicit Result<T> → T
}

// Non-generic result
Result result = Result.Success("Operation completed.");

// HTTP status mapping
HttpStatusCode status = result.ToHttpStatusCode();  // → 200

// Paging
var paged = list.ToPagedResult(pageNumber: 1, pageSize: 10);
```

---

## Core Classes

### ResultCode

Class-based smart enum. Identity is based on `Name`.

```csharp
// Built-in codes
ResultCode.Success      // "success",      200, IsSuccess = true
ResultCode.BadRequest   // "bad_request",  400
ResultCode.Unauthorized // "unauthorized", 401
ResultCode.Forbidden    // "forbidden",    403
ResultCode.NotFound     // "not_found",    404
ResultCode.Conflict     // "conflict",     409
ResultCode.Error        // "error",        500
ResultCode.Unknown      // "unknown",      500

// Custom codes
var rateLimited = new ResultCode("rate_limited", 429);

// Equality based on Name
new ResultCode("test", 200) == new ResultCode("test", 500)  // true

// Implicit string conversion
string code = ResultCode.Success;  // → "success"
string code = (ResultCode)null;    // → null (no throw)

// FromName — for deserialization
ResultCode.FromName("not_found")   // → ResultCode.NotFound (singleton)
ResultCode.FromName("custom")      // → new ResultCode("custom", 500)
ResultCode.FromName(null)          // → ResultCode.Unknown
```

### IResult / IResult\<T\>

Read-only interfaces. All properties are **get-only**.

```csharp
public interface IResult
{
    string RequestId { get; }
    string Code { get; }
    bool IsSuccess { get; }
    string Message { get; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}
```

### ResultBase

Abstract base class for all result types.

| Member | Type | Serialized | Description |
|--------|------|------------|-------------|
| `RequestId` | `string` | ✅ | Lazy-generated GUID |
| `Status` | `ResultCode` | ❌ **Field** | Not serialized by any JSON library |
| `Code` | `string` | ✅ | Getter reads `Status.Name`, setter calls `FromName()` |
| `IsSuccess` | `bool` | ✅ | Computed from `Status.IsSuccess` |
| `Message` | `string` | ✅ | Default `""` |

### Result

Non-generic result. Factory methods:

```csharp
Result.Success("message")
Result.BadRequest("message")
Result.Unauthorized("message")
Result.Forbidden("message")
Result.NotFound("message")
Result.Conflict("message")
Result.Error("message")
Result.From(customCode, "message")  // throws if customCode is null
```

### Result\<T\>

Generic result with `Data`. Factory methods + implicit operators:

```csharp
// Factories
Result<T>.Success(data, "message")  // null data → Error result (no throw)
Result<T>.NotFound("message")
Result<T>.Error("message")
Result<T>.From(customCode, "message")

// Implicit operators — NONE throw
Result<string> r = "hello";          // T → Result<T>: Success
Result<string> r = (string)null;     // T → Result<T>: Error (no throw)
string value = r;                    // Result<T> → T: returns .Data
Result simple = r;                   // Result<T> → Result: preserves RequestId
Result<string> typed = simple;       // Result → Result<T>: preserves RequestId
```

---

## Implicit Operators — Behavior Matrix

| Operator | null input | Behavior |
|----------|-----------|----------|
| `T → Result<T>` | null | Error result (`Code = "error"`) |
| `Result<T> → T` | — | Returns `.Data` (may be null/default) |
| `Result<T> → Result` | null | NullReferenceException (standard .NET) |
| `Result → Result<T>` | null | NullReferenceException (standard .NET) |
| `ResultCode → string` | null | Returns `null` |
| `PagedResult<T> → Paged<T>` | null | Returns `null` |

> **Design principle:** Implicit operators **never throw custom exceptions**. Developer checks `IsSuccess` before accessing `Data`.

---

## Paging

```csharp
// Interfaces (all get-only)
IPage       → PageNumber, PageSize
IPaged      → TotalPages, TotalRecords, HasNextPage, HasPreviousPage
IPaged<T>   → Records

// Classes
Paged       → implements IPaged
Paged<T>    → implements IPaged<T>, inherits Paged
PagedResult<T> → ResultBase + IResult<Paged<T>>

// Usage
var paged = list.ToPaged(pageNumber: 1, pageSize: 10);
var result = list.ToPagedResult(pageNumber: 1, pageSize: 10);

// Implicit conversion
Paged<int> data = pagedResult;  // null → null (no throw)

// TotalPages calculation
// PageSize > 0: Math.Ceiling(TotalRecords / PageSize)
// PageSize <= 0: 0

// Invalid values auto-clamped
list.ToPagedResult(0, -1);  // → pageNumber=1, pageSize=10
```

---

## Extensions

```csharp
using Light.Extensions;

// IsFailed — opposite of IsSuccess
result.IsFailed();  // true if !IsSuccess

// ToHttpStatusCode — maps Status.HttpStatus to System.Net.HttpStatusCode
result.ToHttpStatusCode();  // Success → 200, NotFound → 404, etc.

// ToPagedResult — from IEnumerable<T>
list.ToPagedResult(pageNumber, pageSize);
list.ToPagedResult(iPage);
nullList.ToPagedResult();  // → Error result (no throw)

// ToPaged — from IEnumerable<T>
list.ToPaged(pageNumber, pageSize);
list.ToPaged(iPage);
```

---

## Serialization

`Status` is a **public field** — not serialized by `System.Text.Json` or `Newtonsoft.Json` by default.

### JSON Output

```json
{
  "RequestId": "6da2dcec-6292-4030-994f-b8a467c1681f",
  "Code": "success",
  "IsSuccess": true,
  "Message": "",
  "Data": { ... }
}
```

> No `Status`, `HttpStatus`, or `Name` fields leak into JSON.

### Deserialization

`Code` setter automatically restores `Status` via `ResultCode.FromName()`:

```csharp
var json = JsonSerializer.Serialize(result);
var restored = JsonSerializer.Deserialize<Result>(json);

restored.Code;      // "not_found"
restored.Status;    // ResultCode.NotFound (singleton)
restored.IsSuccess; // false
```

---

## Explicit Throws

Only **explicit method calls** with **required parameters** throw:

| Method | When | Exception |
|--------|------|-----------|
| `new ResultCode(null)` | name is null | `ArgumentNullException` |
| `Result.From(null)` | status is null | `ArgumentNullException` |
| `Result<T>.From(null)` | status is null | `ArgumentNullException` |
| `ToPaged(null list)` | list is null | `ArgumentNullException` |
| `ToPagedResult(null IPage)` | page is null | `ArgumentNullException` |
| `ToPaged(null IPage)` | page is null | `ArgumentNullException` |

> Implicit operators **never** throw custom exceptions.

---

## Web API Usage

```csharp
[ApiExplorerSettings(IgnoreApi = true)]
public virtual IActionResult Ok<T>(T data)
{
    var result = data as IResult ?? Result<T>.Success(data);
    return result.ToActionResult();
    // data null → Error result (no throw)
    // data valid → Success result
}
```

---

## Project Structure

```
Light.Contracts/
├── ResultCode.cs          — Smart enum with built-in codes
├── IResult.cs             — IResult, IResult<T> interfaces
├── ResultBase.cs          — Abstract base (Status field, Code property)
├── Result.cs              — Non-generic result
├── ResultOfT.cs           — Generic Result<T> with implicit operators
├── IPage.cs               — IPage interface
├── IPaged.cs              — IPaged, IPaged<T> interfaces
├── Paged.cs               — Paged, Paged<T> classes
├── PagedResult.cs         — PagedResult<T>
├── PropertyOrder.cs       — [PropertyOrder] attribute
├── HttpStatusMapper.cs    — ToHttpStatusCode() extension
└── ResultExtensions.cs    — IsFailed, ToPaged, ToPagedResult
```

---

## Target Framework

- **.NET Standard 2.0** (`netstandard2.0`)
- **C# 7.3** compatible
- **Zero external dependencies**

---

## License

MIT
