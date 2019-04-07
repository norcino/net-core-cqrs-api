using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace Common.Tests.FluentAssertion
{
    /// <summary>
    /// Add assertion extensions
    /// </summary>
    public static class AssertFluentExtension
    {
        #region HttpResponse Status Codes
        public static void IsOkHttpResponse(this Assert assert, HttpResponseMessage response)
        {
            Assert.IsNotNull(response);
            if(response.StatusCode != HttpStatusCode.OK)
                throw new AssertFailedException($"Expected Ok (200) status code, but was {response.StatusCode} ({(int)response.StatusCode})");
        }

        public static void IsCreatedHttpResponse(this Assert assert, HttpResponseMessage response)
        {
            Assert.IsNotNull(response);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new AssertFailedException($"Expected Created (201) status code, but was {response.StatusCode} ({(int)response.StatusCode})");
        }

        public static void IsNotFoundHttpResponse(this Assert assert, HttpResponseMessage response)
        {
            Assert.IsNotNull(response);
            if (response.StatusCode != HttpStatusCode.NotFound)
                throw new AssertFailedException($"Expected NotFound (404) status code, but was {response.StatusCode} ({(int)response.StatusCode})");
        }

        public static void IsBadRequestHttpResponse(this Assert assert, HttpResponseMessage response)
        {
            Assert.IsNotNull(response);
            if (response.StatusCode != HttpStatusCode.BadRequest)
                throw new AssertFailedException($"Expected BadRequest (400) status code, but was {response.StatusCode} ({(int)response.StatusCode})");
        }

        public static void HttpResponseStatusCodeIs(this Assert assert, HttpResponseMessage response, HttpStatusCode statuscode)
        {
            Assert.IsNotNull(response);
            if (response.StatusCode != statuscode)
                throw new AssertFailedException($"Expected {statuscode} ({(int)statuscode}) status code, but was {response.StatusCode} ({(int)response.StatusCode})");
        }
        #endregion

        #region Objects
        public static AssertObject<T> This<T>(this Assert assert, T subject)
        {
            return new AssertObject<T>(subject);
        }
   
        public static AssertObject<T> And<T>(this AssertObject<T> assertObject)
        {
            return assertObject;
        }
        public static AssertObject<T> HasValue<T>(this AssertObject<T> assertObject, object value, string message = null)
        {
            Assert.AreEqual(assertObject.Object, value, message);
            return assertObject;
        }
        public static AssertObject<T> IsNotNull<T>(this AssertObject<T> assertObject, string message = null)
        {
            Assert.IsNotNull(assertObject.Object, message ?? "The object is null");
            return assertObject;
        }

        public static AssertObject<T> IsTrue<T>(this AssertObject<T> assertObject, string message = null)
        {
            Assert.IsNotNull(assertObject.Object, message ?? "The object is null");
            return assertObject;
        }
        #endregion

        #region Types
        /// <summary>
        /// Verify that the object has the desired type
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <typeparam name="O">Expected Object type to verify</typeparam>
        /// <param name="assertObject">Assert object</param>
        public static void IsOfType<T,O>(this AssertObject<O> assertObject, string message = null)
        {
            if (assertObject.Object is T)
            {
                return;
            }
            throw new AssertFailedException(message ?? $"Expected type {typeof(T)} but was {assertObject.GetType()}");
        }
        #endregion

        #region Object properties
        /// <summary>
        /// Verify that two objects have the same properties, this will ignore the comparison for objects and collections
        /// </summary>
        /// <param name="assertObject">Assert object</param>
        /// <param name="comparedObject">Object to be compared with</param>
        /// <param name="exclusions">String array with the list of properties to not compare</param>
        public static AssertObject<T> HasSameProperties<T>(this AssertObject<T> assertObject, object comparedObject, params string[] exclusions)
        {
            Assert.IsNotNull(assertObject.Object);
            Assert.IsNotNull(comparedObject);

            foreach (var propertyInfo in assertObject.Object.GetType().GetProperties())
            {
                // Exclude properties
                if (exclusions != null && ((IList)exclusions).Contains(propertyInfo.Name)) continue;

                // Ignore Objects and Collections
                if (propertyInfo.PropertyType.GetTypeInfo().IsValueType || propertyInfo.PropertyType == typeof(string))
                {
                    var objectValue = assertObject.Object.GetType().GetProperty(propertyInfo.Name).GetValue(assertObject.Object, null);
                    var comparedObjectValue = comparedObject.GetType().GetProperty(propertyInfo.Name).GetValue(comparedObject, null);

                    if (objectValue is DateTime)
                    {
                        TimeSpan difference = (DateTime)objectValue - (DateTime)comparedObjectValue;
                        Assert.IsTrue(difference < TimeSpan.FromSeconds(1),
                            $"Property '{propertyInfo.Name}' of type DateTime has value {objectValue} but was expected {comparedObjectValue}");
                        continue;
                    }

                    Assert.AreEqual(objectValue, comparedObjectValue, $"Property '{propertyInfo.Name}' of type {assertObject.Object.GetType()} has value {objectValue} but was expected {comparedObjectValue}");
                }
            }
            return assertObject;
        }

        public static SamePropertyObject<T> Except<T>(this SamePropertyObject<T> assertObject, Expression<Func<T, object>> excludedProperty)
        {
            assertObject.Exclusions.Add(excludedProperty);
            return assertObject;
        }

        public static SamePropertyObject<T> Except<T>(this AssertObject<T> assertObject, Expression<Func<T, object>> excludedProperty)
        {
            var assertSamePropertyObject = new SamePropertyObject<T>(assertObject.Object);
            assertSamePropertyObject.Exclusions.Add(excludedProperty);
            return assertSamePropertyObject;
        }
        public static AssertObject<T> HasSameProperties<T>(this SamePropertyObject<T> assertObject, T comparedObject)
        {
            assertObject.ExpectedObject = comparedObject;
            return new AssertObject<T>(assertObject.ComparedObject);
        }

        public static AssertObject<T> Has<T>(this AssertObject<T> assertObject, Func<T, bool> assertions)
        {
            Assert.IsTrue(assertions(assertObject.Object));
            return assertObject;
        }
        #endregion

        #region Collections
        public static AssertCollection<T> IsNotNull<T>(this AssertCollection<T> assertCollection)
        {
            Assert.IsNotNull(assertCollection.Collection);
            return assertCollection;
        }

        public static AssertCollection<T> IsNotNullOrEmpty<T>(this AssertCollection<T> assertCollection)
        {
            Assert.IsNotNull(assertCollection.Collection, "The collection is null");
            Assert.IsTrue(assertCollection.Collection.Any(), "The collection is empty");
            return assertCollection;
        }

        /// <summary>
        /// Make sure that each element of a collection matches the specified criteria in the function
        /// </summary>
        /// <typeparam name="T">Generic type for the collection</typeparam>
        /// <param name="assertCollection"></param>
        /// <param name="assertions">Function which must return true to succeed validation</param>
        /// /// <returns></returns>
        public static AssertCollection<T> Are<T>(this AssertCollection<T> assertCollection, Func<T, bool> assertions)
        {
            Assert.IsTrue(assertCollection.Collection.All(assertions));
            return assertCollection;
        }

        /// <summary>
        /// Make sure that at least one element of a collection matches the specified criteria in the function
        /// </summary>
        /// <typeparam name="T">Generic type for the collection</typeparam>
        /// <param name="assertCollection"></param>
        /// <param name="assertions">Function which must return true to succeed validation</param>
        /// <returns></returns>
        public static AssertCollection<T> Contains<T>(this AssertCollection<T> assertCollection, Func<T, bool> assertions)
        {
            Assert.IsTrue(assertCollection.Collection.Any(assertions));
            return assertCollection;
        }        

        public static AssertCollection<T> Have<T>(this AssertCollection<T> assertCollection, Func<T, bool> assertions)
        {
            return Are(assertCollection, assertions);
        }

        public static AssertCollection<T> HaveCount<T>(this AssertCollection<T> assertCollection, int expectedCount) {
            if (expectedCount != assertCollection.Collection.Count)
                throw new AssertFailedException($"Expected {expectedCount} elements, but the collection had {assertCollection.Collection.Count}");
            return assertCollection;
        }

        public static AssertCollection<T> All<T>(this Assert assert, ICollection<T> collection)
        {
            return new AssertCollection<T>(collection);
        }

        public static AssertCollection<T> And<T>(this AssertCollection<T> assertCollection)
        {
            return assertCollection;
        }

        public static AssertCollection<T> These<T>(this Assert assert, ICollection<T> collection)
        {
            return new AssertCollection<T>(collection);
        }
        #endregion
    }

    /// <summary>
    /// Represent a single object being subject of an assertion
    /// </summary>
    /// <typeparam name="T">Type of the object under test</typeparam>
    public class AssertObject<T>
    {
        public readonly T Object;

        public AssertObject(T subject)
        {
            Object = subject;
        }
    }

    /// <summary>
    /// Class used to create fluent assertions for collections.
    /// To access the inner collection use the property Collection.
    /// Note that the collection is immutable and the original collection will never be changed
    /// </summary>
    /// <typeparam name="T">Generic type for the collection</typeparam>
    public class AssertCollection<T>
    {
        public readonly ReadOnlyCollection<T> Collection;

        public AssertCollection(ICollection<T> collection)
        {
            Collection =  new ReadOnlyCollection<T>(collection.ToList());
        }
    }

    public class SamePropertyObject<T>
    {
        public readonly T ComparedObject;
        public T ExpectedObject;
        public List<Expression<Func<T, object>>> Exclusions;

        public SamePropertyObject(T subject)
        {
            ComparedObject = subject;
            Exclusions = new List<Expression<Func<T, object>>>();
        }
    }    
}

/*
 *  TO COMPLETE
 * Assert.That.This(object)
 * .Except(x.property)
 * .Except(x.property2)
 * .Including(x.property3)
 * .HasSameProperties) 
 */