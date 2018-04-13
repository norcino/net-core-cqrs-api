using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests
{
    /// <summary>
    /// Add assertion extensions
    /// </summary>
    public static class AssertExtension
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

        #region Types
        /// <summary>
        /// Check that the object has a specific type
        /// </summary>
        /// <typeparam name="T">Expected Type</typeparam>
        /// <param name="assert">Assert object</param>
        /// <param name="obj">Object under test</param>
        public static void IsOfType<T>(this Assert assert, object obj)
        {
            if (obj is T)
            {
                return;
            }
            throw new AssertFailedException($"Expected type {typeof(T)} but was {obj.GetType()}");
        }
        #endregion

        #region Object properties
        /// <summary>
        /// Veriry that two objects have the same properties, this will ignore the comparison for objects and collections
        /// </summary>
        /// <param name="assert">Assert object</param>
        /// <param name="obj">Object under test</param>
        /// <param name="comparedObject">Object to be compared with</param>
        /// <param name="exclusions">String array with the list of properties to not compare</param>
        public static void HaveSameProperties(this Assert assert, object obj, object comparedObject, params string[] exclusions)
        {
            Assert.IsNotNull(obj);
            Assert.IsNotNull(comparedObject);

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                // Exclude properties
                if (exclusions != null && ((IList) exclusions).Contains(propertyInfo.Name)) continue;

                // Ignore Objects and Collections
                if (propertyInfo.PropertyType.GetTypeInfo().IsValueType || propertyInfo.PropertyType == typeof(string))
                {
                    var objectValue = obj.GetType().GetProperty(propertyInfo.Name).GetValue(obj, null);
                    var comparedObjectValue = comparedObject.GetType().GetProperty(propertyInfo.Name).GetValue(comparedObject, null);

                    if (objectValue is DateTime)
                    {
                        TimeSpan difference = (DateTime) objectValue - (DateTime) comparedObjectValue;
                        Assert.IsTrue(difference < TimeSpan.FromSeconds(1),
                            $"Property '{propertyInfo.Name}' of type DateTime has value {objectValue} but was expected {comparedObjectValue}");
                        continue;
                    }

                    Assert.AreEqual(objectValue, comparedObjectValue, $"Property '{propertyInfo.Name}' of type {obj.GetType()} has value {objectValue} but was expected {comparedObjectValue}");
                }
            }
        }
        #endregion

        #region Collections
        public static void HasCountOf(this Assert assert, int expectedCount, ICollection collection)
        {
            if(collection == null)
                throw new AssertFailedException($"Expected {expectedCount} elements, but the collection was null");

            if (expectedCount != collection.Count)
                throw new AssertFailedException($"Expected {expectedCount} elements, but the collection had {collection.Count}");
        }
        #endregion
    }
}