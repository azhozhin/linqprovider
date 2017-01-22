using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace linqprovider
{
    public class ProjectedColumns
    {
        public Expression Projector { get; }

        public ReadOnlyCollection<ColumnDeclaration> Columns { get; }

        internal ProjectedColumns(Expression projector, ReadOnlyCollection<ColumnDeclaration> columns)
        {
            Projector = projector;
            Columns = columns;
        }


    }
}