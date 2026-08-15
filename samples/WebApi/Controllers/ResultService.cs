using Light.Contracts;

namespace WebApi.Controllers
{
    public static class ResultService
    {
        public static Result GetSuccess()
        {
            return Result.Success();
        }

        public static Result GetError()
        {
            return Result.Error();
        }

        public static Result<int> GetSuccessData()
        {
            return Result<int>.Success(1);
        }

        public static Result<int> GetErrorData()
        {
            return Result.Error("Error when get ID");
        }

        public static string GetId()
        {
            return Result<string>.Success(Guid.NewGuid().ToString());
        }

        public static string GetIdButError()
        {
            return Result<string>.Error();
        }
    }
}
