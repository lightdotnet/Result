# Lightsoft.Result

Zero-dependency Result Pattern library for .NET Standard 2.0+ / C# 7.3+ (NuGet: `Lightsoft.Result`, namespaces `Light.Contracts` / `Light.Extensions`). Provides `Result`, `Result<T>`, `PagedResult<T>` with a class-based smart enum `ResultCode`, without hidden exceptions from implicit operators.

## Layout

- `src/Result/` — the library itself (`Result.csproj`, targets `netstandard2.0`). This is what gets packed and published to NuGet.
  - `Contracts/` — `Result`, `Result<T>`, `ResultBase`, `ResultCode`, `IResult`/`IResult<T>`, paging types (`Paged`, `Paged<T>`, `PagedResult<T>`, `IPage`, `IPaged`)
  - `Extensions/` — `ResultExtensions` (`IsFailed`, `ToHttpStatusCode`), `PagedExtensions` (`ToPaged`, `ToPagedResult`)
- `tests/UnitTests/` — NUnit tests (targets `net10.0`), one `[TestFixture]` class per source type
- `samples/WebApi/` — ASP.NET Core sample showing consumer usage (`ActionResultExtensions`, `ResultController`)

## Core design rules (do not violate)

1. **Zero runtime dependencies.** Never add a package reference to `src/Result/Result.csproj`. No JSON library — `Status` is a plain field so it's excluded from serialization by convention, not attributes.
2. **Implicit operators never throw.** `null` input to an implicit conversion must produce an Error result or `null`/`default` output — never a custom exception. Only explicit methods with required parameters throw (`ArgumentNullException` for `new ResultCode(null)`, `Result.From(null)`, `ToPaged(null IPage)`, etc. — note `ToPaged(null list)`/`ToPagedResult(null list)` are null-safe, not throwing, since the list is the data being operated on, not a required parameter object). If you add an implicit operator, follow the existing behavior matrix in `README.md`.
3. **`src/Result` stays C# 7.3 / netstandard2.0 compatible.** No nullable reference type annotations, switch expressions, using declarations, records, or other C# 8+/9+ only syntax in that project. (`tests/UnitTests` targets net10.0 and has `#nullable disable` per-file — that's fine, it's not shipped.)
4. **`ResultCode` identity is by `Name`**, not reference — equality, `==`, and `FromName` all key off the string name. Built-in codes are `public static readonly` singletons.
5. Public API changes to `src/Result` are NuGet package surface (current version `2.1.0.0` in the csproj) — treat removals/signature changes as breaking and call it out explicitly.

## Conventions

- Factory-method style: static `Success`/`Error`/`NotFound`/etc. methods on `Result` and `Result<T>`, mirroring the `ResultCode` built-ins. Adding a new built-in code means updating `ResultCode.cs`, `Result.cs`, `Result<T>` (`ResultOfT.cs`), and usually `README.md` together (see the `add-result-code` skill).
- Test naming: `Method_Scenario_Expected` inside a `[TestFixture]` class named `<Type>Tests`. Assertions use the fluent helpers in `tests/UnitTests/LightAssert.cs` (`.ShouldBe(...)`, `.ShouldBeTrue()`, `.ShouldBeNull()`, `.ShouldThrow<T>(...)`, etc.) instead of raw `Assert.That`.
- `README.md` is the canonical public-facing doc for the package — keep its tables/examples in sync with any API change in `src/Result`.

## Build & test

```
dotnet build Result.slnx
dotnet test tests/UnitTests/UnitTests.csproj
```
