using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace linqprovider
{
    public class QueryTranslator : ExpressionVisitor
    {
        private StringBuilder _sb;
        private ParameterExpression _row;
        private ColumnProjection _projection;

        internal QueryTranslator()
        {

        }

        internal TranslateResult Translate(Expression expression)
        {
            _sb = new StringBuilder();
            _row = Expression.Parameter(typeof(ProjectionRow), "row");
            Visit(expression);
            return new TranslateResult
            {
                CommandText = _sb.ToString(),
                Projector = _projection != null ? Expression.Lambda(_projection.Selector, _row) : null,
            };
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                var lambda = (LambdaExpression) StripQuotes(m.Arguments[1]);
                _sb.Append("SELECT ");
                _sb.Append("*");
                _sb.Append(" FROM (");
                Visit(m.Arguments[0]);
                _sb.Append(") AS T");
                _sb.Append(" WHERE ");
                Visit(lambda.Body);
                return m;
            }
            if (m.Method.Name == "Select")
            {
                var lambda = (LambdaExpression) StripQuotes(m.Arguments[1]);
                var localProjection = new ColumnProjector().ProjectColumns(lambda.Body, _row);
                _sb.Append("SELECT ");
                _sb.Append(localProjection.Columns);
                _sb.Append(" FROM (");
                Visit(m.Arguments[0]);
                _sb.Append(") AS T");
                _projection = localProjection;
                return m;
            }

            throw new NotSupportedException($"The method ‘{m.Method.Name}’ is not supported");
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    _sb.Append(" NOT ");
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException($"The unary operator ‘{u.NodeType}’ is not supported");
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            _sb.Append("(");

            Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                    _sb.Append(" AND ");
                    break;
                case ExpressionType.Or:
                    _sb.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    _sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    _sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    _sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    _sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _sb.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException($"The binary operator ‘{b.NodeType}’ is not supported");
            }

            Visit(b.Right);

            _sb.Append(")");

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;
            if (q != null)
            {
                // assume constant nodes w/ IQueryables are table references
                _sb.Append("SELECT * FROM ");
                _sb.Append(q.ElementType.Name);
            }
            else if (c.Value == null)
            {
                _sb.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        _sb.Append((bool) c.Value ? 1 : 0);
                        break;
                    case TypeCode.String:
                        _sb.Append("'");
                        _sb.Append(c.Value);
                        _sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException($"The constant for ‘{c.Value}’ is not supported");
                    default:
                        _sb.Append(c.Value);
                        break;
                }
            }
            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                _sb.Append(m.Member.Name);
                return m;
            }
            throw new NotSupportedException($"The member ‘{m.Member.Name}’ is not supported");
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression) e).Operand;
            }
            return e;
        }
    }

    internal class TranslateResult {

        internal string CommandText;

        internal LambdaExpression Projector;

    }
}