using Light.Contracts;
using System.Net;

namespace Light.Extensions
{
    public static class HttpStatusMapper
    {
        public static HttpStatusCode ToHttpStatusCode(this IResult result)
        {
            var rb = result as ResultBase;
            if (rb != null && rb.Status != null)
                return (HttpStatusCode)rb.Status.HttpStatus;

            return HttpStatusCode.InternalServerError;
        }
    }
}
