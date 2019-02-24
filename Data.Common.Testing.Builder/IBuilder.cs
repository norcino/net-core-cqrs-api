using System;
using System.Collections.Generic;

namespace Data.Common.Testing.Builder
{
    /// <summary>
    /// Generic builder used to create custom testing model set.
    /// Each entity is generated creating random values for reference types, strings and dates.
    /// </summary>
    /// <typeparam name="TE">Type of the entity to be created</typeparam>
    public interface IBuilder<TE> where TE : class, new()
    {
        /// <summary>
        /// Generates a default valid entity, by default does not create child entities, but this can be changed specifying the depth level.
        /// Depth one will generate child entities, if any. Depth two will also generate grandchild entities, and so on.
        /// </summary>
        /// <param name="hierarchyDepth">Depth lever to be reached when generating child entity objects</param>
        /// <returns>Entity generated</returns>
        TE Build(int hierarchyDepth);

        /// <summary>
        /// Generates the entity with default values and customise it applying the provided action.
        /// </summary>
        /// <param name="entitySetupAction">Action used to customise the created entity</param>
        /// <returns>The created entity</returns>
        TE Build(Action<TE> entitySetupAction);

        /// <summary>
        /// Generates a specific amount of entities as per the number specified.
        /// Uses the standard build method to generate each instance and uses the optional action
        /// to customize the entity based on the creation index
        /// </summary>
        /// <param name="numberOfEntities">Number of entities to be created</param>
        /// <param name="entitySetupAction">Override action based on the index of the created element</param>
        /// <returns>List of the entities created</returns>
        List<TE> BuildMany(int numberOfEntities, Action<TE, int> entitySetupAction = null);
    }
}
