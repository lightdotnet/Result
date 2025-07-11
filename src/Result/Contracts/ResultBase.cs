using System;

namespace Light.Contracts
{
    public abstract class ResultBase : IResult
    {
        public string RequestId { get; set; } = Guid.NewGuid().ToString();

        public string Code { get; set; }

        public bool Succeeded { get; set; }

        public string Message { get; set; } = "";
    }
}
