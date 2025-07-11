namespace Light.Contracts
{
    public class Result : ResultBase
    {
        public Result() { }

        protected internal Result(ResultCode code)
        {
            Code = code.ToString();
            Succeeded = code == ResultCode.success;
        }

        protected internal Result(ResultCode code, string message) : this(code)
        {
            Message = message;
        }

        public static Result Success(string message = "") =>
            new Result(ResultCode.success, message);

        public static Result BadRequest(string message = "") =>
            new Result(ResultCode.bad_request, message);

        public static Result Forbidden(string message = "") =>
            new Result(ResultCode.forbidden, message);

        public static Result Unauthorized(string message = "") =>
            new Result(ResultCode.unauthorized, message);

        public static Result NotFound(string message = "") =>
            new Result(ResultCode.not_found, message);

        public static Result Conflict(string message = "") =>
            new Result(ResultCode.conflict, message);

        public static Result Error(string message = "") =>
            new Result(ResultCode.error, message);
    }
}