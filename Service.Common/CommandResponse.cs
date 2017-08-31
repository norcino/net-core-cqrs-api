﻿using Common.Validation;
using System.Collections.Generic;

namespace Service.Common
{
    public interface ICommandResponse
    {
        bool Successful { get; set; }

        List<ValidationEntry> ValidationEntries { get; set; }
    }

    public interface ICommandResponse<out TResult> : ICommandResponse
    {
        TResult Result { get; }
    }

    public class CommandResponse : ICommandResponse
    {
        public bool Successful { get; set; }

        public List<ValidationEntry> ValidationEntries { get; set; }

        public CommandResponse()
        {
            ValidationEntries = new List<ValidationEntry>();
        }
    }

    public class CommandResponse<TResult> : CommandResponse, ICommandResponse<TResult>
    {
        public TResult Result { get; }

        public CommandResponse() : base() { }

        public CommandResponse(TResult result) : base()
        {
            Result = result;
        }
    }
}
