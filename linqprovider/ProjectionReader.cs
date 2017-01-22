using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace linqprovider
{
    public class ProjectionReader<T> : IEnumerable<T>
    {
        private Enumerator<T> _enumerator;

        internal ProjectionReader(DbDataReader reader, Func<ProjectionRow, T> projector)
        {
            _enumerator = new Enumerator<T>(reader, projector);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var e = _enumerator;
            if (e == null)
            {
                throw new InvalidOperationException("Cannot enumerate more than once");
            }

            _enumerator = null;
            return e;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class Enumerator<T> : ProjectionRow, IEnumerator<T>
        {
            readonly DbDataReader _reader;

            T _current;

            readonly Func<ProjectionRow, T> _projector;


            internal Enumerator(DbDataReader reader, Func<ProjectionRow, T> projector)
            {
                _reader = reader;
                _projector = projector;
            }

            public override object GetValue(int index)
            {
                if (index >= 0)
                {
                    var value = _reader.IsDBNull(index) ? null : _reader.GetValue(index);
                    return value;
                }
                throw new IndexOutOfRangeException();
            }

            public T Current => _current;

            object IEnumerator.Current => _current;

            public bool MoveNext()
            {
                if (_reader.Read())
                {
                    _current = _projector(this);
                    return true;
                }
                return false;
            }

            public void Reset()
            {
            }

            public void Dispose()
            {
                _reader.Dispose();
            }
        }
    }
}