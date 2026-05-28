using System;

namespace Light.Contracts
{
    public class Result : ResultBase
    {
        public Result() { }

        protected internal Result(ResultCode status, string message = "")
        {
            Status = status;
            Message = message;
        }

        public static Result Success(string message = "")
            => new Result(ResultCode.Success, message);

        public static Result BadRequest(string message = "")
            => new Result(ResultCode.BadRequest, message);

        public static Result Forbidden(string message = "")
            => new Result(ResultCode.Forbidden, message);

        public static Result Unauthorized(string message = "")
            => new Result(ResultCode.Unauthorized, message);

        public static Result NotFound(string message = "")
            => new Result(ResultCode.NotFound, message);

        public static Result Conflict(string message = "")
            => new Result(ResultCode.Conflict, message);

        public static Result Error(string message = "")
            => new Result(ResultCode.Error, message);

        public static Result From(ResultCode status, string message = "")
            => new Result(
                status ?? throw new ArgumentNullException(nameof(status)),
                message);
    }
}
