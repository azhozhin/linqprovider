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
            return Translate(expression).CommandText;
        }

        public override object Execute(Expression expression)
        {
            var result = Translate(expression);

            var cmd = _connection.CreateCommand();
            cmd.CommandText =result.CommandText;
            var reader = cmd.ExecuteReader();
            var elementType = TypeSystem.GetElementType(expression.Type);

            if (result.Projector != null)
            {
                var projector = result.Projector.Compile();
                return Activator.CreateInstance(
                    typeof(ProjectionReader<>).MakeGenericType(elementType),
                    BindingFlags.Instance | BindingFlags.NonPublic, null,
                    new object[] {reader, projector},
                    null);

            }
            return Activator.CreateInstance(
                typeof(ObjectReader<>).MakeGenericType(elementType),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] {reader},
                null);
        }

        private static TranslateResult Translate(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            return new QueryTranslator().Translate(expression);
        }
    }
}