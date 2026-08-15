---
name: dotnet-test
description: Build the Result.slnx solution and run the NUnit test suite for Lightsoft.Result, reporting failures clearly. Use whenever asked to build, test, or verify the project compiles/passes, or before reporting a code change in src/Result or tests/UnitTests as complete.
---

# dotnet-test

Build and test this repo (`Lightsoft.Result`) and summarize the result.

## Steps

1. Restore + build the whole solution first, so compile errors are caught before test execution:
   ```
   dotnet build Result.slnx
   ```
   If this fails, stop and report the build errors — don't run tests against a broken build.

2. Run the unit tests with normal verbosity so individual failures are visible:
   ```
   dotnet test tests/UnitTests/UnitTests.csproj --logger "console;verbosity=normal"
   ```

3. If any test fails, read the failing test method in `tests/UnitTests/*.cs` and the source it exercises in `src/Result/` before proposing a fix — don't guess from the assertion message alone.

4. Report: pass/fail count, and for failures, the test name + one-line reason. Don't paste the full raw dotnet output back verbatim unless asked.

## Notes

- `src/Result/Result.csproj` has `GeneratePackageOnBuild=True`, so a plain build also produces a `.nupkg` under `src/Result/bin/` — that's expected, not a side effect to "fix."
- `tests/UnitTests` targets `net10.0` and uses NUnit (not xUnit) — don't suggest xUnit-style attributes (`[Fact]`, `[Theory]`) here.
- If `dotnet` isn't on PATH or the SDK version doesn't match, report that clearly rather than trying unrelated workarounds.
