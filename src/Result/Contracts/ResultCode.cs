using System;

namespace Light.Contracts
{
    public class ResultCode : IEquatable<ResultCode>
    {
        public string Name { get; }
        public int HttpStatus { get; }
        public bool IsSuccess { get; }

        public ResultCode(string name, int httpStatus = 500, bool isSuccess = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            HttpStatus = httpStatus;
            IsSuccess = isSuccess;
        }

        // ── Built-in codes ──────────────────────────
        public static readonly ResultCode Unknown = new ResultCode("unknown", 500);
        public static readonly ResultCode Success = new ResultCode("success", 200, true);
        public static readonly ResultCode BadRequest = new ResultCode("bad_request", 400);
        public static readonly ResultCode Unauthorized = new ResultCode("unauthorized", 401);
        public static readonly ResultCode Forbidden = new ResultCode("forbidden", 403);
        public static readonly ResultCode NotFound = new ResultCode("not_found", 404);
        public static readonly ResultCode Conflict = new ResultCode("conflict", 409);
        public static readonly ResultCode Error = new ResultCode("error", 500);

        // ── FromName (for deserialization) ──────────
        public static ResultCode FromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return Unknown;
            if (name == Success.Name) return Success;
            if (name == BadRequest.Name) return BadRequest;
            if (name == Unauthorized.Name) return Unauthorized;
            if (name == Forbidden.Name) return Forbidden;
            if (name == NotFound.Name) return NotFound;
            if (name == Conflict.Name) return Conflict;
            if (name == Error.Name) return Error;
            if (name == Unknown.Name) return Unknown;
            return new ResultCode(name);
        }

        // ── Equality ────────────────────────────────
        public override string ToString() => Name;

        public bool Equals(ResultCode other)
        {
            if (other is null) return false;
            return Name == other.Name;
        }

        public override bool Equals(object obj) => obj is ResultCode other && Equals(other);

        public override int GetHashCode() => Name != null ? Name.GetHashCode() : 0;

        public static bool operator ==(ResultCode left, ResultCode right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(ResultCode left, ResultCode right) => !(left == right);

        public static implicit operator string(ResultCode code) => code?.Name;
    }
}
