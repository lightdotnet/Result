# Result Wrapper
Simple result wrapper pattern

### Default Result Codes
You can change the default result codes by set `Code` property in the Result class.
```
public enum ResultCode
{
    unknown,
    success,
    bad_request,
    unauthorized,
    forbidden,
    not_found,
    conflict,
    error,
}
```

### Success Result
```
Result.Success(string message = "");

Result<T>.Success(T data, string message = "");
```

### HttpStatusCode mapper
```
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
```

### Extensions
- [more](./src/Result/Extensions)
