namespace Light.Contracts
{
    public class Result<T> : ResultBase, IResult<T>
    {
        public Result() : base() { }

        protected internal Result(T data, string message)
        {
            if (data == null)
            {
                Code = ResultCode.unknown.ToString();
            }
            else
            {
                Code = ResultCode.success.ToString();
                Succeeded = true;
                Message = message;
                Data = data;
            }
        }

        protected internal Result(ResultCode code, string message)
        {
            Code = code.ToString();
            Message = message;
        }

        public static implicit operator T(Result<T> result) => result.Data;

        public static implicit operator Result(Result<T> result) => new Result
        {
            Code = result.Code,
            Succeeded = result.Succeeded,
            Message = result.Message
        };

        public T Data { get; set; }

        public static Result<T> Success(T data, string message = "") =>
            new Result<T>(data, message);

        public static Result<T> BadRequest(string message = "") =>
            new Result<T>(ResultCode.bad_request, message);

        public static Result<T> Forbidden(string message = "") =>
            new Result<T>(ResultCode.forbidden, message);

        public static Result<T> Unauthorized(string message = "") =>
            new Result<T>(ResultCode.unauthorized, message);

        public static Result<T> NotFound(string message = "") =>
            new Result<T>(ResultCode.not_found, message);

        public static Result<T> Error(string message = "") =>
            new Result<T>(ResultCode.error, message);
    }
}