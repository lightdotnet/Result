namespace Light.Contracts
{
    public interface IResult
    {
        string RequestId { get; }
        string Code { get; }
        bool IsSuccess { get; }
        string Message { get; }
    }

    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }
}
