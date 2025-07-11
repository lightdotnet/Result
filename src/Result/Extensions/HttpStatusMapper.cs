using Light.Contracts;
using System.Net;

namespace Light.Extensions
{
    public static class HttpStatusMapper
    {
        public static HttpStatusCode MapHttpStatusCode(this IResult result)
        {
            var code = result.MapResultCode();

            switch (code)
            {
                case ResultCode.success:
                    return HttpStatusCode.OK;
                case ResultCode.bad_request:
                    return HttpStatusCode.BadRequest;
                case ResultCode.unauthorized:
                    return HttpStatusCode.Unauthorized;
                case ResultCode.forbidden:
                    return HttpStatusCode.Forbidden;
                case ResultCode.not_found:
                    return HttpStatusCode.NotFound;
                case ResultCode.conflict:
                    return HttpStatusCode.Conflict;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        }
    }
}
