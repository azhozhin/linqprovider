using System;
using System.Linq.Expressions;

namespace linqprovider
{
    public class ColumnExpression : Expression
    {
        public string Alias { get; }
        public string Name { get; }
        public int Ordinal { get; }

        public override ExpressionType NodeType { get; }
        public override Type Type { get; }

        internal ColumnExpression(Type type, string alias, string name, int ordinal)
        {
            NodeType = (ExpressionType) DbExpressionType.Column;
            Type = type;
            Alias = alias;
            Name = name;
            Ordinal = ordinal;
        }
    }
}