using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static linqprovider.TypeSystem;

namespace linqprovider
{
    public abstract class QueryProvider : IQueryProvider
    {
        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new Query<S>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = GetElementType(expression.Type);
            try
            {
                var genericType = typeof(Query<>).MakeGenericType(elementType);
                return (IQueryable) Activator.CreateInstance(genericType, this, expression);
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult) Execute(expression);
        }

        public abstract string GetQueryText(Expression expression);

        public abstract object Execute(Expression expression);
    }
}