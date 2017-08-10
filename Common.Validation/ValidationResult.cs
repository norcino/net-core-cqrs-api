using System.Collections.Generic;
using System.Linq;

namespace Common.Validation
{
    public class ValidationResult
    {
        /// <summary>
        /// Whether validation succeeded
        /// </summary>
        public virtual bool IsValid => Entries.All(e => e.Severity != Severity.Error);

        /// <summary>
        /// A collection of errors
        /// </summary>
        public IList<ValidationEntry> Entries { get; }

        /// <summary>
        /// Creates a new validationResult
        /// </summary>
        public ValidationResult()
        {
            this.Entries = new List<ValidationEntry>();
        }

        /// <summary>
        /// Creates a new ValidationResult from a collection of failures
        /// </summary>
        /// <param name="failures">List of <see cref="ValidationFailure"/> which is later available through <see cref="Entries"/>. This list get's copied.</param>
        /// <remarks>
        /// Every caller is responsible for not adding <c>null</c> to the list.
        /// </remarks>
        public ValidationResult(IEnumerable<ValidationEntry> failures)
        {
            Entries = failures.Where(failure => failure != null).ToList();
        }
    }
}