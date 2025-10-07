using System;

namespace Light.Contracts
{
    public abstract class ResultBase : IResult
    {
        public virtual string RequestId { get; set; } = Guid.NewGuid().ToString();

        public virtual string Code { get; set; }

        public virtual bool Succeeded { get; set; }

        public virtual string Message { get; set; } = "";
    }
}
