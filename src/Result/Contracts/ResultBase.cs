using System;

namespace Light.Contracts
{
    public abstract class ResultBase : IResult
    {
        private readonly object _requestIdLock = new object();
        private volatile string _requestId;

        public string RequestId
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

        public bool IsSuccess => !(Status is null) && Status.IsSuccess;

        public string Message { get; set; } = "";

        // Shared "no data → Error, has data → Success" resolution used by Result<T> and PagedResult<T> constructors.
        private protected static void ResolveDataStatus(bool hasData, string message, string nullMessage, out ResultCode status, out string resolvedMessage)
        {
            if (!hasData)
            {
                status = ResultCode.Error;
                resolvedMessage = string.IsNullOrEmpty(message) ? nullMessage : message;
            }
            else
            {
                status = ResultCode.Success;
                resolvedMessage = message ?? "";
            }
        }
    }
}
