using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace linqprovider
{
    public class DbQueryProvider : QueryProvider
    {
        private readonly DbConnection _connection;

        public DbQueryProvider(DbConnection connection)
        {
            _connection = connection;
        }

        public override string GetQueryText(Expression expression)
        {
            return Translate(expression);
        }

        public override object Execute(Expression expression)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = Translate(expression);
            var reader = cmd.ExecuteReader();
            var elementType = TypeSystem.GetElementType(expression.Type);

            return Activator.CreateInstance(
                typeof(ObjectReader<>).MakeGenericType(elementType),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] {reader},
                null);
        }

        private static string Translate(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            return new QueryTranslator().Translate(expression);
        }
    }
}