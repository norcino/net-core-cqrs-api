using System;
using System.Linq.Expressions;

namespace Service.Common.QueryTraits
{
    public interface ICanFilter<T>
    {
        Expression<Func<T, bool>> Filter { get; set; }
    }
}