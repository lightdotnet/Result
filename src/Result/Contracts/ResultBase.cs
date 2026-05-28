using System;

namespace Light.Contracts
{
    public abstract class ResultBase : IResult
    {
        private string _requestId;

        public virtual string RequestId
        {
            get
            {
                if (_requestId == null)
                    _requestId = Guid.NewGuid().ToString();
                return _requestId;
            }
            set { _requestId = value; }
        }

        public virtual ResultCode Status { get; set; } = ResultCode.Unknown;

        public string Code
        {
            get { return Status != null ? Status.Name : ResultCode.Unknown.Name; }
        }

        public bool IsSuccess
        {
            get { return Status != null && Status.IsSuccess; }
        }

        public virtual string Message { get; set; } = "";
    }
}
