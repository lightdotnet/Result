using Light.Contracts;
using System.Net;

namespace Light.Extensions
{
    public static class ResultExtensions
    {
        public static bool IsFailed(this IResult result)
        {
            return result == null || !result.IsSuccess;
        }

        public static HttpStatusCode ToHttpStatusCode(this IResult result)
        {
            if (result == null)
                return HttpStatusCode.InternalServerError;

            if (result is ResultBase rb && rb.Status != null)
                return (HttpStatusCode)rb.Status.HttpStatus;

            return (HttpStatusCode)ResultCode.FromName(result.Code).HttpStatus;
        }
    }
}
