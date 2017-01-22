using System.Linq.Expressions;
using System.Reflection;

namespace linqprovider
{
    internal class ProjectionBuilder : DbExpressionVisitor
    {
        ParameterExpression _row;
        private static readonly MethodInfo _miGetValue;

        static ProjectionBuilder()
        {
            _miGetValue = typeof(ProjectionRow).GetMethod("GetValue");
        }

        internal LambdaExpression Build(Expression expression)
        {
            _row = Expression.Parameter(typeof(ProjectionRow), "row");
            var body = Visit(expression);
            return Expression.Lambda(body, _row);
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            var methodCallExpression = Expression.Call(_row, _miGetValue, Expression.Constant(column.Ordinal));
            return Expression.Convert(methodCallExpression, column.Type);
        }
    }
}