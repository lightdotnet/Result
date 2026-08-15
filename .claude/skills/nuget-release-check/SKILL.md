---
name: nuget-release-check
description: Pre-flight checklist before packing/publishing a new Lightsoft.Result NuGet release — version bump, README sync, zero-dependency check, build/pack verification. Use when asked to prepare, check readiness for, or cut a new release/version of the package.
---

# nuget-release-check

Read-only-first checklist before a `Lightsoft.Result` release. Report status per item; only make edits the user actually asks for (e.g. don't bump the version yourself unless asked to).

## Checklist

1. **Version.** Read `<Version>` in `src/Result/Result.csproj`. Confirm it was actually bumped from what's on NuGet.org / the last git tag for a release — compare against `git tag` and/or `git log -p -- src/Result/Result.csproj` for the previous value. Follow semver: breaking API change → major, additive public API → minor, fix-only → patch.

2. **Zero dependencies held.** Confirm `src/Result/Result.csproj`'s `<ItemGroup>` still has no `PackageReference` beyond what `Directory.Build.props` injects (`Microsoft.SourceLink.GitHub`, dev-only via `PrivateAssets="All"`). If a real dependency snuck in, that's a hard stop — flag it, this library's core promise is zero-dependency.

3. **Target framework unchanged (or intentionally changed).** `netstandard2.0` in `Result.csproj` — confirm it wasn't accidentally narrowed/widened. A TFM change is itself a compatibility-relevant release note item.

4. **Build + pack clean.**
   ```
   dotnet build Result.slnx -c Release
   dotnet test tests/UnitTests/UnitTests.csproj -c Release
   ```
   `GeneratePackageOnBuild=True` on `Result.csproj` means the Release build already produces the `.nupkg` under `src/Result/bin/Release/` — confirm it exists and its filename version matches step 1.

5. **README in sync.** Diff recent `src/Result/` changes against `README.md`'s Core Classes / Implicit Operators / Explicit Throws / Project Structure sections — these are what NuGet.org renders as the package description, so drift here is user-facing.

6. **Icon/license/metadata present.** `icon.png` at repo root, `PackageLicenseExpression=MIT` in the csproj, `Authors`/`Description` still accurate.

Summarize as a short pass/fail list, not prose — this is meant to be scanned quickly before someone runs `dotnet nuget push`.
