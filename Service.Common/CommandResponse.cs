using Common.Validation;
using System.Collections.Generic;

namespace Service.Common
{
    public class CommandResponse
    {
        public bool Successful { get; set; }

        public List<ValidationEntry> ValidationEntries { get; set; }

        public CommandResponse()
        {
            ValidationEntries = new List<ValidationEntry>();
        }
    }

    public class CommandResponse<TResult> : CommandResponse
    {
        public TResult Result { get; }

        public CommandResponse(TResult result) : base()
        {
            Result = result;
        }
    }
}
