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
                Status = ResultCode.NotFound;
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
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data),
                    "Use NotFound() or Error() instead of Success(null).");
            return new Result<T>(data, message);
        }

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

        // T → Result<T>
        public static implicit operator Result<T>(T data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data),
                    "Cannot implicitly convert null to Result<T>. "
                    + "Use NotFound() or Error() instead.");
            return new Result<T>(data);
        }

        // Result<T> → T
        public static implicit operator T(Result<T> result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result),
                    "Cannot extract data from a null Result<T>.");
            if (!result.IsSuccess)
                throw new InvalidOperationException(
                    "Cannot extract data from a failed result. "
                    + "Code: " + result.Code + ", Message: " + result.Message);
            if (result.Data == null)
                throw new InvalidOperationException(
                    "Cannot extract data: Data is null despite success status. "
                    + "Code: " + result.Code);
            return result.Data;
        }

        // Result<T> → Result (preserves RequestId)
        public static implicit operator Result(Result<T> result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result),
                    "Cannot implicitly convert null Result<T> to Result.");
            return new Result
            {
                RequestId = result.RequestId,
                Status = result.Status,
                Message = result.Message
            };
        }

        // Result → Result<T> (preserves RequestId)
        public static implicit operator Result<T>(Result result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result),
                    "Cannot implicitly convert null Result to Result<T>.");
            return new Result<T>
            {
                RequestId = result.RequestId,
                Status = result.Status,
                Message = result.Message
            };
        }
    }
}
