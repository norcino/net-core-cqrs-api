using System;
using System.Collections.Generic;
using System.Reflection;
using Common.Tests;

namespace Data.Common.Testing.Builder
{
    /// <inheritdoc cref="IBuilder{TE}"/>
    public class Builder<TE> : IBuilder<TE> where TE : class, new()
    {
        /// <inheritdoc cref="IBuilder{TE}.Build()"/>
        public virtual TE Build()
        {
            var e = (TE)Activator.CreateInstance(typeof(TE));

            var properties = e.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            
            foreach (var propertyInfo in properties)
            {
                var propertyType = propertyInfo.PropertyType;
                propertyInfo.SetValue(e, GenerateAnonymousData(propertyType, propertyInfo.Name));
            }
            
            return e;
        }

        private object GenerateAnonymousData(Type propertyType, string propertyName)
        {
            if (propertyType == typeof(string))
                return AnonymousData.String(propertyName);

            if (propertyType == typeof(sbyte) || propertyType == typeof(byte))
                return AnonymousData.Byte();

            if (propertyType == typeof(short) || propertyType == typeof(ushort))
                return AnonymousData.Short();

            if (propertyType == typeof(int) || propertyType == typeof(uint))
                return AnonymousData.Int();

            if (propertyType == typeof(long) || propertyType == typeof(ulong))
                return AnonymousData.Long();

            if (propertyType == typeof(double))
                return AnonymousData.Double();

            if (propertyType == typeof(float))
                return AnonymousData.Float();

            if (propertyType == typeof(char))
                return AnonymousData.Char();

            if (propertyType == typeof(DateTime))
                return AnonymousData.DateTime();

            if (propertyType == typeof(TimeSpan))
                return AnonymousData.TimeSpan();

            if (propertyType.IsValueType)
            {
                return Activator.CreateInstance(propertyType);
            }
            return null;
        }

        /// <inheritdoc cref="IBuilder{TE}.Build(int, Func{TE,int,TE})"/>
        public List<TE> Build(int numberOfEntities, Func<TE, int, TE> creationFunction = null)
        {
            if (numberOfEntities < 1)
                throw new ArgumentOutOfRangeException($"{nameof(numberOfEntities)} must be greater than zero");

            var result = new List<TE>();
            for (var i = 1; i <= numberOfEntities; i++)
            {
                result.Add(creationFunction == null ? Build() : creationFunction(Build(), i));
            }
            return result;
        }

        /// <inheritdoc cref="IBuilder{TE}.Build(Action{TE})"/>
        public TE Build(Action<TE> entitySetupAction)
        {
            if (entitySetupAction == null)
                throw new ArgumentNullException($"{nameof(entitySetupAction)}");

            var entity = Build();
            entitySetupAction(entity);
            return entity;
        }
    }
}
