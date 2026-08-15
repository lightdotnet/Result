---
name: add-result-code
description: Scaffold a new built-in ResultCode (e.g. "too many requests" / 429) end-to-end across ResultCode.cs, Result.cs, ResultOfT.cs, README.md, and tests. Use whenever asked to add a new result/error code, status, or factory method to the Lightsoft.Result library.
---

# add-result-code

Adding one new built-in `ResultCode` touches five places in this repo. Missing any one of them leaves the codebase inconsistent (this is exactly what `dotnet-api-reviewer` checks for), so do all of them in one pass.

Ask the user for (if not already given): the code **name** (snake_case string, e.g. `"too_many_requests"`), the **HTTP status** (int), and whether it's a **success** code (default false).

## Steps

1. **`src/Result/Contracts/ResultCode.cs`**
   - Add `public static readonly ResultCode <PascalCaseName> = new ResultCode("<snake_case_name>", <httpStatus>[, true]);` next to the other built-ins, keeping them roughly in HTTP-status order.
   - Add a matching branch in `FromName`: `if (name == <PascalCaseName>.Name) return <PascalCaseName>;`

2. **`src/Result/Contracts/Result.cs`**
   - Add a factory method mirroring the existing ones exactly:
     ```csharp
     public static Result <PascalCaseName>(string message = "")
         => new Result(ResultCode.<PascalCaseName>, message);
     ```

3. **`src/Result/Contracts/ResultOfT.cs`**
   - Add the parallel generic factory method for `Result<T>` (read the file first — match whatever signature pattern the other non-Success factories use there, e.g. `NotFound`/`Error`).

4. **`README.md`**
   - Add the new code to the `ResultCode` built-in codes list.
   - Add the new factory to the `Result` factory methods list (and `Result<T>` if applicable).

5. **Tests** — hand off to (or, if asked to do it yourself, follow the conventions of) the `dotnet-test-writer` agent:
   - `ResultCodeTests.cs`: assert the new code's `Name`/`HttpStatus`/`IsSuccess`, and that `ResultCode.FromName("<snake_case_name>")` returns the singleton.
   - `ResultTests.cs` (and the `Result<T>` test file if one exists): assert the new factory method sets `Status` correctly, matching the pattern of `All_Factories_Should_Return_Correct_Status`.

6. Run `dotnet build Result.slnx` and `dotnet test tests/UnitTests/UnitTests.csproj` to confirm everything compiles and passes.

## Constraints

- Keep `src/Result/*.cs` changes C# 7.3 / netstandard2.0 compatible (see root `CLAUDE.md`).
- Do not add a package reference. This is plain C#, no dependency needed.
- This is a public API addition (new public static members) — additive, not breaking, but still worth calling out in the summary since it ships in the NuGet package.
