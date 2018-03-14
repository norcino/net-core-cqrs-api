using System;
using System.Collections.Generic;

namespace Data.Common.Testing.Builder
{
    public interface IBuilder<TE> where TE : class, new()
    {
        TE Build();
        List<TE> Build(int numberOfEntities, Func<TE, int, TE> creationFunction = null);
        //IBuilder<TE> Prepare(Action<TE> entitySetupAction);
        TE Build(Action<TE> entitySetupAction);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TE">Type of the Entity</typeparam>
    public class Builder<TE> : IBuilder<TE> where TE : class, new()
    {
        /// <summary>
        /// Generates the entity according with the customization and returs it
        /// </summary>
        /// <returns>Entity generated</returns>
        public virtual TE Build()
        {
            var e = (TE)Activator.CreateInstance(typeof(TE));

            // For each property
            // If property value type, generate random value and assign it
            // If property value is complex type use builder to genrate it
            // If List of value type, generate a list of random value and assign it
            // If property value is list of complex type use builder to genrate them

            return e;
        }

        /// <summary>
        /// Generates a specific amount of entities as per the number specified.
        /// Uses the standard build method to generate each instance and uses the optional function
        /// to customize the entity based on the creation index
        /// </summary>
        /// <param name="numberOfEntities">Number of entities to be created</param>
        /// <param name="creationFunction">Override function based on the index of the created element</param>
        /// <returns>List of the entities created</returns>
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
