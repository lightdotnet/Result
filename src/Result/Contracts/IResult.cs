namespace Light.Contracts
{
    public interface IResult
    {
        string RequestId { get; set; }

        string Code { get; }

        bool Succeeded { get; }

        string Message { get; }
    }

    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }
}