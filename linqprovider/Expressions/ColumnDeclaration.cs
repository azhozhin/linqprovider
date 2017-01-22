﻿using System.Linq.Expressions;

namespace linqprovider
{
    public class ColumnDeclaration
    {
        public string Name { get; }
        public Expression Expression { get; }

        public ColumnDeclaration(string name, Expression expression)
        {
            Name = name;
            Expression = expression;
        }
    }
}