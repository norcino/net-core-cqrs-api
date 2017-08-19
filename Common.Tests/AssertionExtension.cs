using System;
using System.Collections;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests
{
    /// <summary>
    /// Add assertion extensions
    /// </summary>
    public static class AssertExtension
    {
        /// <summary>
        /// Veriry that two objects have the same properties, this will ignore the comparison for objects and collections
        /// </summary>
        /// <param name="this"></param>
        /// <param name="second"></param>
        /// <param name="exclusions"></param>
        public static void ShouldHaveSameProperties(this object @this, object second, params string[] exclusions)
        {
            Assert.IsNotNull(@this);
            Assert.IsNotNull(second);

            foreach (var propertyInfo in @this.GetType().GetProperties())
            {
                // Exclude properties
                if (exclusions != null && ((IList) exclusions).Contains(propertyInfo.Name)) continue;

                // Ignore Objects and Collections
                if (propertyInfo.PropertyType.GetTypeInfo().IsValueType || propertyInfo.PropertyType == typeof(string))
                {
                    var firstValue = @this.GetType().GetProperty(propertyInfo.Name).GetValue(@this, null);
                    var secondValue = second.GetType().GetProperty(propertyInfo.Name).GetValue(second, null);

                    if (firstValue is DateTime)
                    {
                        TimeSpan difference = (DateTime) firstValue - (DateTime) secondValue;
                        Assert.IsTrue(difference < TimeSpan.FromSeconds(1));
                        continue;
                    }

                    Assert.AreEqual(firstValue, secondValue,
                        string.Format("Assert failure for Property '{0}' ({1})", propertyInfo.Name, @this.GetType()));
                }
            }
        }
    }
}