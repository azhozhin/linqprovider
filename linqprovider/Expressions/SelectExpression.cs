using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace linqprovider
{
    public class SelectExpression : Expression
    {
        public string Alias { get; }
        public ReadOnlyCollection<ColumnDeclaration> Columns { get; }
        public Expression From { get; }
        public Expression Where { get; }

        public override ExpressionType NodeType { get; }
        public override Type Type { get; }

        public SelectExpression(Type type, string alias, IEnumerable<ColumnDeclaration> columns,
            Expression from, Expression where)
        {
            NodeType = (ExpressionType) DbExpressionType.Select;
            Type = type;
            Alias = alias;
            Columns = columns as ReadOnlyCollection<ColumnDeclaration> ??
                      new List<ColumnDeclaration>(columns).AsReadOnly();
            From = from;
            Where = where;
        }
    }
}