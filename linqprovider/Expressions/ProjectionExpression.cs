using System;
using System.Linq.Expressions;

namespace linqprovider
{
    public class ProjectionExpression : Expression
    {
        public SelectExpression Source { get; }
        public Expression Projector { get; }

        public override ExpressionType NodeType { get; }
        public override Type Type { get; }

        public ProjectionExpression(SelectExpression source, Expression projector)
        {
            NodeType = (ExpressionType) DbExpressionType.Projection;
            Type = projector.Type;
            Source = source;
            Projector = projector;
        }
    }
}