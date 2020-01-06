﻿using System.Collections.Generic;

namespace Common.Validation
{
    public class ValidationEntry
    {
        /// <summary>
        /// Creates a new ValidationEntry.
        /// </summary>
        public ValidationEntry(string propertyName, string error) : this(propertyName, error, null)
        {
        }

        /// <summary>
        /// Creates a new ValidationEntry.
        /// </summary>
        public ValidationEntry(string propertyName, string error, object attemptedValue)
        {
            PropertyName = propertyName;
            ErrorMessage = error;
            AttemptedValue = attemptedValue;
        }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// The error message
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// The property value that caused the failure.
        /// </summary>
        public object AttemptedValue { get; private set; }

        /// <summary>
        /// Custom state associated with the failure.
        /// </summary>
        public object CustomState { get; set; }

        /// <summary>
        /// Custom severity level associated with the failure.
        /// </summary>
        public Severity Severity { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the formatted message arguments.
        /// These are values for custom formatted message in validator resource files
        /// Same formatted message can be reused in UI and with same number of format placeholders
        /// Like "Value {0} that you entered should be {1}"
        /// </summary>
        public object[] FormattedMessageArguments { get; set; }

        /// <summary>
        /// Gets or sets the formatted message placeholder values.
        /// Similar placeholders are defined in fluent validation library (check documentation)
        /// </summary>
        public Dictionary<string, object> FormattedMessagePlaceholderValues { get; set; }

        /// <summary>
        /// The resource name used for building the message
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// Creates a textual representation of the failure.
        /// </summary>
        public override string ToString()
        {
            return ErrorMessage;
        }
    }
}
