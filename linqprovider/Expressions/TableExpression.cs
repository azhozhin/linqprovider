using System;
using System.Linq.Expressions;

namespace linqprovider
{
    public class TableExpression : Expression
    {
        public string Alias { get; }
        public string Name { get; }

        public override ExpressionType NodeType { get; }
        public override Type Type { get; }

        internal TableExpression(Type type, string alias, string name)
        {
            NodeType = (ExpressionType) DbExpressionType.Table;
            Type = type;
            Alias = alias;
            Name = name;
        }
    }
}