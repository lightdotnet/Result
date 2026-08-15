using System;

namespace Light.Contracts
{
    public abstract class ResultBase : IResult
    {
        private readonly object _requestIdLock = new object();
        private volatile string _requestId;

        public virtual string RequestId
        {
            get
            {
                if (_requestId == null)
                {
                    lock (_requestIdLock)
                    {
                        if (_requestId == null)
                            _requestId = Guid.NewGuid().ToString();
                    }
                }
                return _requestId;
            }
            set { _requestId = value; }
        }

        // Field — not serialized by any JSON library by default
        public ResultCode Status = ResultCode.Unknown;

        public string Code
        {
            get { return !(Status is null) ? Status.Name : ResultCode.Unknown.Name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Status = ResultCode.FromName(value);
            }
        }

        public bool IsSuccess
        {
            get { return !(Status is null) && Status.IsSuccess; }
        }

        public virtual string Message { get; set; } = "";
    }
}
