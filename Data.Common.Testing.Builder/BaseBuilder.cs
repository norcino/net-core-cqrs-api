using System;
using System.Collections.Generic;

namespace Data.Common.Testing.Builder
{
    /// <summary>
    /// Base builder to be used to create specific builders able to create entities for testing
    /// </summary>
    /// <typeparam name="TB">Type of the Builder</typeparam>
    /// <typeparam name="TE">Type of the Entity</typeparam>
    public abstract class BaseBuilder<TB, TE>
        where TB : BaseBuilder<TB, TE>
        where TE : class, new()
    {
        /// <summary>
        /// Starts the creation of a new Entity
        /// </summary>
        /// <returns>Builder to use fluent configuration</returns>
        public static TB New()
        {
            return (TB)Activator.CreateInstance(typeof(TB));
        }

        /// <summary>
        /// Generates the entity according with the customization and returns it
        /// </summary>
        /// <returns>Entity generated</returns>
        public virtual TE Build()
        {
            return (TE)Activator.CreateInstance(typeof(TE));
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
