using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace linqprovider
{
    public class Query<T> : IOrderedQueryable<T>
    {
        private readonly QueryProvider _provider;
        private readonly Expression _expression;

        public Query([NotNull] QueryProvider provider)
        {
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public Query(
            [NotNull] QueryProvider provider,
            [NotNull] Expression expression)
        {
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException($"expression should implement IQueryable<{typeof(T)}>");
            }
            _provider = provider;
            _expression = expression;
        }

        Expression IQueryable.Expression => _expression;
        Type IQueryable.ElementType => typeof(T);
        IQueryProvider IQueryable.Provider => _provider;

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _provider.Execute(_expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _provider.Execute(_expression)).GetEnumerator();
        }

        public override string ToString()
        {
            return _provider.GetQueryText(_expression);
        }
    }
}