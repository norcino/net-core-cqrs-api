using System;
using System.Linq.Expressions;

namespace Service.Common.QueryTreats
{
    public interface ICanFilter<T>
    {
        Expression<Func<T, bool>> Filter { get; set; }
    }
}