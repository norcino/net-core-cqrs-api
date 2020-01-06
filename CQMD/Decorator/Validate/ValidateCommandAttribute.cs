using System;

namespace Service.Common.CommandAttributes
{
    public class ValidateCommandAttribute : Attribute
    {
        public bool Validate { get; set; }

        public ValidateCommandAttribute()
        {
            Validate = true;
        }
    }
}
