using System;
using System.Collections.Generic;
using Data.Context;

namespace Data.Common.Testing.Builder
{
    public interface IPersister<TE> where TE : class, new()
    {
//        TE Build();
//        List<TE> Build(int numberOfEntities, Func<TE, int, TE> creationFunction = null);
//        IBuilder<TE> Prepare(Action<TE> entitySetupAction);
//        TE Build(Action<TE> entitySetupAction);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TE">Type of the Entity</typeparam>
    public class Persister<TE> : IPersister<TE> where TE : class, new()
    {
        private Action<TE> _entitySetupAction;
        private IHouseKeeperContext _context;

        public Persister(IHouseKeeperContext context)
        {
            _context = context;
        }

        public virtual TE Persist(TE entity)
        {
            var dbSet = _context.Set<TE>();
            dbSet.Attach(entity);
            _context.SaveChanges();
            return entity;
        }

        public List<TE> Persist(int numberOfEntities, Action<TE, int> entitySetupAction = null)
        {
            if (numberOfEntities < 1)
                throw new ArgumentOutOfRangeException($"{nameof(numberOfEntities)} must be greater than zero");

            var result = new List<TE>();
            for (var i = 1; i <= numberOfEntities; i++)
            {
                var entity = new Builder<TE>().Build();

                entitySetupAction?.Invoke(entity, i);

                result.Add(Persist(entity));
            }
            return result;
        }

        public TE Persist(Action<TE> entitySetupAction)
        {
            if (entitySetupAction == null)
                throw new ArgumentNullException($"{nameof(entitySetupAction)}");

            var entity = new Builder<TE>().Build();
            entitySetupAction(entity);
            return Persist(entity);
        }


        //    public static class Persister
        //    {
        //        public static TE Persist<TE>(this IBuilder<TE> builder, IHouseKeeperContext context) where TE : class, new()
        //        {

        //        }
        //    }
    }
}