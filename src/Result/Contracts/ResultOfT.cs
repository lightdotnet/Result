using System;

namespace Light.Contracts
{
    public class Result<T> : ResultBase, IResult<T>
    {
        public Result() { }

        protected internal Result(T data, string message = "")
        {
            if (data == null)
            {
                Status = ResultCode.Error;
                Message = string.IsNullOrEmpty(message) ? "Data is null." : message;
            }
            else
            {
                Status = ResultCode.Success;
                Message = message;
                Data = data;
            }
        }

        protected internal Result(ResultCode status, string message = "")
        {
            Status = status;
            Message = message;
        }

        public virtual T Data { get; set; }

        // ── Built-in factories ──────────────────
        public static Result<T> Success(T data, string message = "")
            => new Result<T>(data, message);

        public static Result<T> BadRequest(string message = "")
            => new Result<T>(ResultCode.BadRequest, message);

        public static Result<T> Forbidden(string message = "")
            => new Result<T>(ResultCode.Forbidden, message);

        public static Result<T> Unauthorized(string message = "")
            => new Result<T>(ResultCode.Unauthorized, message);

        public static Result<T> NotFound(string message = "")
            => new Result<T>(ResultCode.NotFound, message);

        public static Result<T> Conflict(string message = "")
            => new Result<T>(ResultCode.Conflict, message);

        public static Result<T> Error(string message = "")
            => new Result<T>(ResultCode.Error, message);

        public static Result<T> From(ResultCode status, string message = "")
            => new Result<T>(
                status ?? throw new ArgumentNullException(nameof(status)),
                message);

        // ── Implicit operators ──────────────────

        // T → Result<T> (null → Error, follows Result pattern)
        public static implicit operator Result<T>(T data) => new Result<T>(data);

        // Result<T> → T (returns Data directly, developer checks IsSuccess; null instance → default)
        public static implicit operator T(Result<T> result) => result == null ? default : result.Data;

        // Result<T> → Result (preserves RequestId)
        public static implicit operator Result(Result<T> result) => new Result
        {
            RequestId = result.RequestId,
            Status = result.Status,
            Message = result.Message
        };

        // Result → Result<T> (preserves RequestId)
        public static implicit operator Result<T>(Result result) => new Result<T>
        {
            RequestId = result.RequestId,
            Status = result.Status,
            Message = result.Message
        };
    }
}
