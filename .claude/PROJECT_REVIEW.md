# Project Review — Lightsoft.Result

Full-codebase re-audit (not diff-based), produced by running the four review agents in `.claude/agents/` (`dotnet-correctness-reviewer`, `dotnet-simplicity-reviewer`, `dotnet-test-quality-reviewer`, `dotnet-api-reviewer`) against `src/Result/`, `tests/UnitTests/`, `samples/WebApi/`, and `README.md`, then de-duplicated and prioritized by hand.

This supersedes all previous reviews: every finding from prior passes has been independently re-verified as fixed on the current `init-claude` branch (94/94 tests passing, clean build) and removed from this document.

**Current status: no open findings.** The sections below are kept as a record of decisions made during the last review cycle, in case any get revisited.

---

## Decisions made this cycle

- **Version bump**: kept at `2.1.0.0` (not bumped to `3.0.0.0`) — user's explicit call, on the basis that this cycle's changes are internal/behavioral (constructor dedup, comment-only clarifications) rather than a `Result`/`Result<T>` structural change. `.claude/CLAUDE.md` rule 5 stays in sync.
- **`PagedResult<T>`/`Result<T>` constructor duplication**: fixed via a shared `private protected static ResolveDataStatus(...)` helper on `ResultBase` (assembly-internal only, not `protected` — narrowed after an API review pass flagged the initial `protected` modifier as leaking unintended API surface to external subclassers).
- **`Data` mutability**: attempted get-only, **reverted**. `Result<T>`/`PagedResult<T>` have a public parameterless constructor (needed by the implicit `Result<T> <-> Result` operators' object-initializer pattern), so `System.Text.Json`'s reflection deserializer relies on a public `Data` setter to populate it on round-trip — get-only silently broke deserialization (`Data` stayed `null`) with no exception, caught by the pre-existing `Deserialize_ResultT_Should_Restore` test. Can't fix via `[JsonConstructor]` without violating the zero-dependency rule (that attribute isn't inbox on netstandard2.0). `Data` stays `{ get; set; }`; the desync risk (`result.Data = null` after construction not flipping `Status`) is documented as a caveat in the README instead of fixed structurally.
- **`ResultCode.FromName("")`**: left as-is (collapses to `Unknown`, same as `null`) — intentional, to avoid an "invisible" custom code with no visible name. Documented with a code comment; existing tests already encode this as expected behavior.
- **Test coverage gaps closed**: `ResultCode.Equals(object)`, `ResultBase.Code` setter's custom/unknown-code path, and `ResultCode` JSON deserialization with missing fields (plus a matching README Explicit Throws row) all now have direct test coverage.
