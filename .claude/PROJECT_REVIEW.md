# Project Review — Lightsoft.Result

Full-codebase audit (not diff-based — repo was clean at review time), produced by running the four review agents in `.claude/agents/` (`dotnet-correctness-reviewer`, `dotnet-simplicity-reviewer`, `dotnet-test-quality-reviewer`, `dotnet-api-reviewer`) against `src/Result/`, `tests/UnitTests/`, `samples/WebApi/`, and `README.md`, then de-duplicated and prioritized by hand.

Legend: **P0** = fix before next release (user-facing or breaks the library's core promise) · **P1** = should fix soon · **P2** = refactor/style/nice-to-have.

---

## P0 — Correctness bugs & docs that will actively mislead users

### 1. `README.md:27` installs the wrong package
```
dotnet add package Light.Contracts
```
`Light.Contracts` is only the **C# namespace**. The actual `PackageId` (from `AssemblyName` in `src/Result/Result.csproj:6`, confirmed by the NuGet badge at `README.md:1`) is **`Lightsoft.Result`**. Anyone following the Quick Start verbatim installs the wrong package or gets a "not found" error. **Fix the install command immediately** — this is the single highest-impact finding in the whole review.

### 2. `Result<T> -> T` implicit operator throws on a null `Result<T>` reference — undocumented, contradicts the library's core pitch
`src/Result/Contracts/ResultOfT.cs:65`
```csharp
public static implicit operator T(Result<T> result) => result.Data;
```
`Result<string> r = null; string s = r;` throws `NullReferenceException`. This is **not** one of the two rows already documented as throwing (see #3) — the README's matrix (`README.md:171`) lists this row's null-input column as `--`, implying no null-throw case exists. It does. Guard it:
```csharp
public static implicit operator T(Result<T> result) => result == null ? default : result.Data;
```
Then update the README table to reflect the fix (or, if left as-is, document the null-instance-throw explicitly and add a test — see Test Coverage below).

### 3. Top-level "no hidden throws" claims contradict the library's own documented behavior table
`README.md:17` and `:177` state implicit operators are "safe" / "never throw custom exceptions" without qualification, but the Behavior Matrix (`README.md:172-173`) explicitly documents `Result<T> -> Result` and `Result -> Result<T>` as throwing `NullReferenceException` on a null instance (confirmed in code at `ResultOfT.cs:68-73` and `:76-81`). This is intentional/documented behavior, but the unqualified marketing-style claims elsewhere in the README are misleading. Reword the Features bullet and "Design principle" line to something like *"implicit operators never throw custom exceptions; converting a **null result instance** still throws the standard `NullReferenceException` — see the Behavior Matrix"* so the claim and the table agree.

---

## P1 — Real inconsistencies worth fixing

### 4. `IsFailed(this IResult)` throws on a null receiver; its neighbor `ToHttpStatusCode` doesn't
`src/Result/Extensions/ResultExtensions.cs:11-14` vs `:16-22` (same file, back to back):
```csharp
public static bool IsFailed(this IResult result) => !result.IsSuccess;          // throws on null
public static HttpStatusCode ToHttpStatusCode(this IResult result) { ... }      // null-safe, falls back to 500
```
Pick one philosophy and apply it consistently. Given the library's stated design, the null-safe version is almost certainly what's intended:
```csharp
public static bool IsFailed(this IResult result) => result == null || !result.IsSuccess;
```

### 5. `ToPaged` throws `ArgumentNullException` on a null list; `ToPagedResult` returns a graceful Error result for the same input
`src/Result/Extensions/ResultExtensions.cs:24-34` (silent Error) vs `:59-63` (throws). Both behaviors are individually documented in the README, but two sibling "turn a list into paging output" methods disagreeing on null-handling philosophy is a real trap for callers who learn the convention from one and assume it applies to the other. Recommend making `ToPaged` null-safe too (return an empty/default `Paged<T>` or align both to throw) rather than leaving them split.

### 6. Integer overflow in paging math can silently return the wrong page instead of an empty one
`src/Result/Extensions/ResultExtensions.cs:44` and `:73` (duplicated in both methods):
```csharp
.Skip((pageNumber - 1) * pageSize)
```
`pageNumber`/`pageSize` are only lower-bound clamped (`< 1`), never upper-bound clamped. A large `pageNumber` (easily reachable from `samples/WebApi/Controllers/ResultController.cs:70`, which passes raw query-string ints straight through) can overflow `int`, wrap negative, and `Skip` with a negative count skips zero elements — silently returning **page 1's data mislabeled as a far-future page**, not an empty result. Consider `checked` arithmetic or clamping `pageNumber` against a sane upper bound (or against computed `TotalPages`).

### 7. `Result<T>` constructor normalizes a null `message` on the error branch but not the success branch
`src/Result/Contracts/ResultOfT.cs:9-22`:
```csharp
if (data == null) { Message = string.IsNullOrEmpty(message) ? "Data is null." : message; }  // guarded
else               { Message = message; }                                                    // NOT guarded
```
`Result<T>.Success("ok", null)` leaves `Message == null`, breaking the implicit "Message defaults to `""`" invariant elsewhere (`ResultBase.cs:38`). Same unguarded pass-through exists in the non-generic `Result.cs` factories. Normalize `message ?? ""` consistently.

### 8. `RequestId` lazy-init isn't thread-safe
`src/Result/Contracts/ResultBase.cs:9-18` — two threads reading `RequestId` on the same instance before it's first set can each generate and briefly observe a different GUID; the instance settles on whichever was written last, so a value a caller already read/logged can silently stop matching later reads. Low likelihood in typical request-scoped usage, but worth a `lock` or `Guid.NewGuid()` computed once in the constructor if results are ever shared across threads.

### 9. `Result<T>.Success(data, message)` can silently return a *failure* result
`src/Result/Contracts/ResultOfT.cs:33-34` delegates to the constructor described in #7, so `Result<T>.Success(null)` returns `IsSuccess == false`, `Status == ResultCode.Error` — a method named `Success` can produce a non-success result with no compiler signal. This is documented in the README but worth a `<remarks>` doc-comment on the method itself, since the method name alone is actively misleading at the call site.

### 10. `PagedResult<T>` is missing the factory-method surface `Result`/`Result<T>` both have
`src/Result/Contracts/PagedResult.cs` has no `NotFound`/`BadRequest`/`Error`/`From(...)` factories, unlike its siblings. The only way to build a non-success `PagedResult<T>` today is an ad-hoc object initializer (as done internally in `ResultExtensions.cs:29-33`). Either add the parallel factories or note in README that `PagedResult<T>` is success-oriented by design.

### 11. `PagedResult<T>`'s null-data constructor can't customize its error message
`src/Result/Contracts/PagedResult.cs:9-21` hardcodes `"Data is null."` with no `message` parameter, unlike `Result<T>`'s equivalent constructor (`ResultOfT.cs:9-22`) which accepts an override. Add `string message = ""` for parity.

### 12. `ToHttpStatusCode` only works correctly for `ResultBase`-derived `IResult`s
`src/Result/Extensions/ResultExtensions.cs:16-22` — `IResult` is a public interface third parties can implement directly. A non-`ResultBase` implementer always maps to 500 regardless of its actual `Code`, even though `result.Code` is available on the interface and `ResultCode.FromName(result.Code)` could resolve it generically.

### 13. `IPage.cs` contradicts the README's "all get-only" claim for paging interfaces
`src/Result/Contracts/IPage.cs:5-6` — `PageNumber`/`PageSize` are `{ get; set; }`, not get-only, unlike `IPaged`/`IPaged<T>` which genuinely are. Either fix the README wording (`README.md:184`) or make `IPage` get-only if mutability wasn't intentional.

---

## P2 — Refactor / simplify / rename

### Duplicated pagination pipeline (refactor)
`src/Result/Extensions/ResultExtensions.cs` — `ToPagedResult(list, pageNumber, pageSize)` (24-49) and `ToPaged(list, pageNumber, pageSize)` (59-78) share an identical clamp → materialize → `Skip`/`Take` block; the two `IPage`-overloads (51-57, 80-86) are likewise byte-for-byte duplicates of each other. Extract one shared private helper so the two public methods (and any future one) can't drift out of sync — this is also *why* finding #6's overflow bug and the clamp logic exist in two places instead of one.

### `PagedResult<T>` reinvents `Result<T>`'s null-check branch (refactor)
`src/Result/Contracts/PagedResult.cs:9-21` duplicates the "null → Error, else → Success" logic that `Result<T>`'s constructor (`ResultOfT.cs:9-22`) already implements, with the message-customization gap noted in #11. Consider having `PagedResult<T>` delegate to (or share a helper with) `Result<T>`'s logic instead of a parallel implementation.

### Unused `virtual` modifiers, applied inconsistently (simplify)
`RequestId` (`ResultBase.cs:9`), `Message` (`ResultBase.cs:38`), `Result<T>.Data` (`ResultOfT.cs:30`) are all `virtual` with **zero overrides anywhere in the repo** (checked `src/`, `samples/`, `tests/`). `PagedResult.cs:29`'s `Data` property is plain (non-virtual), so the codebase already disagrees with itself about whether `Data` should be overridable. Drop `virtual` from all three unless a concrete subclassing need exists.

### `PropertyOrderAttribute` is public API with no consumer anywhere (simplify)
`src/Result/Contracts/PropertyOrderAttribute.cs` — nothing in `src/Result/` reflects over it; the only references are tautological tests (`ResultTests.cs:306-321`, which just assert the attribute's own metadata) and a filename mention in the README. It currently does nothing for a consumer who applies it. Either wire it into a real ordering routine the library ships, or remove it.

### Style drift: expression-bodied vs. block-bodied trivial members (style)
`src/Result/Contracts/ResultCode.cs` consistently uses expression-bodied one-liners (`ToString() => Name;`), but `ResultBase.cs:25,35` (`Code`, `IsSuccess` getters) and `Paged.cs:23-24` (`HasPreviousPage`, `HasNextPage`) use verbose block bodies for equally trivial expressions. Convert to `=>` form to match the convention `ResultCode.cs` already sets.

### Rename: `Paged` constructor parameter `data` → `records`
`src/Result/Contracts/Paged.cs:31` — the constructor parameter is named `data` but is stored into a property named `Records` (`:39`), breaking the pattern every other type in the library follows (a `data` param maps to a `Data` property, e.g. `Result<T>`, `PagedResult<T>`). Rename the parameter to `records` to match `IPaged<T>.Records` and avoid readers hunting for a nonexistent `Data` property on `Paged<T>`.

### `README.md` "Project Structure" paths don't match the real layout (doc drift)
`README.md:303,314` show a tree rooted at `Result.Contracts/`/`Result.Extensions/`; actual paths are `src/Result/Contracts/` and `src/Result/Extensions/`. Cosmetic, but worth fixing alongside the other README items above.

### Pin `<LangVersion>` explicitly
Neither `Result.csproj` nor `Directory.Build.props` sets `<LangVersion>`, so C# 7.3 compatibility currently relies on the *implicit* default inferred from `TargetFramework=netstandard2.0`. Nothing violates it today, but a future SDK upgrade could silently raise the effective language version with no project-level guard catching an accidental C#8+ addition. Add `<LangVersion>7.3</LangVersion>` to `Result.csproj` to make the README's compatibility promise (`README.md:322-323`) enforced by the build, not just by convention.

---

## Test coverage gaps (concrete, ready to hand to `dotnet-test-writer`)

The library's headline promise is "implicit operators never throw" — the two most important gaps are the ones that leave that exact promise unverified:

1. **`Result<T> -> Result` and `Result -> Result<T>` null-instance throws are documented in README's Behavior Matrix but have zero test coverage.** Add:
   ```csharp
   LightAssert.ShouldThrow<NullReferenceException>(() => { Result<string> t = null; Result r = t; });
   LightAssert.ShouldThrow<NullReferenceException>(() => { Result r = null; Result<string> t = r; });
   ```
2. **`Status = null` defensive guards** (`ResultBase.Code`, `ResultBase.IsSuccess`, `ResultExtensions.ToHttpStatusCode`) are never actually exercised — existing tests hit the `ResultCode.Unknown` default, not the `Status == null` branch itself. Add a test that explicitly sets `result.Status = null` (it's a public mutable field) and asserts the fallbacks.
3. **`ToPaged`'s clamp logic is untested** — only `ToPagedResult_Should_Clamp_Invalid_Values` exists; there's no `ToPaged` equivalent, even though the two methods duplicate (not share) the clamp logic.
4. **No empty-list paging test** for either `ToPaged` or `ToPagedResult` (only null-list and 10-item-list are covered).
5. **No uneven/partial-last-page test** — all existing paging tests use evenly-divisible totals; nothing proves `Skip`/`Take` and `Math.Ceiling`-based `TotalPages` agree on a partial last page.
6. **No test for `pageNumber` beyond the last available page** — related to finding #6 above; document/verify the actual (current) behavior with a test either way.
7. **`Result<T>.From(status, message)` success path is untested** — only the null-throw path is covered; the non-generic `Result.From` success path is tested but its generic sibling isn't.
8. **The non-`IList<T>` materialization branch** (`!(list is IList<T> materialized) => list.ToList()`) is dead as far as the test suite is concerned — every paging test uses a `List<int>`. Add a test with a lazy `IEnumerable<T>` (e.g. `Enumerable.Range(...).Where(...)`).
9. Minor: a couple of assertions are weaker than they look —
   - `Serialize_Should_Not_Include_Status_Field` checks for absence of `"HttpStatus"`/`"Name"` substrings rather than `"Status"` directly.
   - `ToPagedResult_Null_List_Should_Return_Error` doesn't check `Message`/`Data`, unlike the analogous `Result<T>` null-data test.
   - `ToPagedResult_Should_Clamp_Invalid_Values` tests both `pageNumber` and `pageSize` clamping from one call; splitting into two isolates which rule broke on a future regression.
10. Minor style drift: two raw `Assert.That` calls in `PropertyOrderAttribute_Should_Have_Correct_Usage` (`ResultTests.cs:318-319`) could use `.ShouldNotBeNull()` / `.ShouldBe(...)` instead, for consistency with the rest of the fixture.

---

## Sample project (`samples/WebApi`) cleanup

- `Controllers/ApiControllerBase.cs:6-8,17` — XML doc comments that restate the class name or are empty (`/// <returns></returns>`). Remove or fill with real content.
- `Controllers/ResultController.cs:61-66` — `FindValue()` always returns `Data: null` (`_list` holds `"0".."19"`, filtered for `"A"`, which never matches) — as a reference sample it demonstrates nothing useful. Replace with a real success/not-found example.
- `Controllers/ResultController.cs:104-135` — `ResultService` (a static demo helper unrelated to ASP.NET routing) lives inside `ResultController.cs`. Move it to its own file to match the one-type-per-file convention the rest of the repo follows.

---

## Summary checklist

- [x] **P0** Fix README install command (`Light.Contracts` → `Lightsoft.Result`)
- [x] **P0** Guard `Result<T> -> T` implicit operator against a null `Result<T>` instance
- [x] **P0** Reconcile README's unqualified "no hidden throws" claims with its own Behavior Matrix
- [x] **P1** Make `IsFailed` null-safe (or intentionally leave it and document why it differs from `ToHttpStatusCode`)
- [x] **P1** Align `ToPaged`/`ToPagedResult` null-list philosophy — `ToPaged` is now null-safe (empty `Paged<T>`), matching `ToPagedResult`'s graceful pattern
- [x] **P1** Clamp/guard paging math against integer overflow
- [x] **P1** Normalize null `message` on the `Result<T>` success branch
- [x] **P1** Consider thread-safety for `RequestId` lazy-init — double-checked locking + `volatile` (the explicit `RequestId` setter remains an unguarded escape hatch, a pre-existing tradeoff, not newly introduced)
- [x] **P1** Add `PagedResult<T>` factory methods and message-customization parity, or document the asymmetry — decided **not** to add `BadRequest`/`NotFound`/etc. factories; `PagedResult<T>` stays success-oriented by design (a paging query returns data or a null-data Error, not arbitrary failure statuses). Message-customization parity (`message` param on the data constructor) was kept.
- [x] **P1** Make `ToHttpStatusCode` work for non-`ResultBase` `IResult` implementers, or document the limitation
- [x] **P1** Fix `IPage` get-only claim in README (or make the interface get-only) — kept `IPage` mutable (interface unchanged); README's Paging section reworded to state `IPage` is intentionally mutable while `IPaged`/`IPaged<T>` are get-only
- [x] **P2** Extract shared pagination helper to remove duplication — `PagedExtensions.cs` now has a private `Slice<T>` helper shared by `ToPaged`/`ToPagedResult`; the `IPage`-overload duplication was also resolved by extension methods delegating to the primary overload
- [ ] **P2** Drop unused `virtual` modifiers; remove or wire up `PropertyOrderAttribute`
- [ ] **P2** Align expression-bodied style across `ResultBase.cs`/`Paged.cs`
- [ ] **P2** Rename `Paged` constructor parameter `data` → `records`
- [ ] **P2** Fix README "Project Structure" paths; pin `<LangVersion>7.3</LangVersion>`
- [ ] Add the ~10 missing tests listed above (start with the two null-instance NRE tests — they're the only untested rows of the README's own documented contract)
- [ ] Clean up `samples/WebApi` per the three items above
