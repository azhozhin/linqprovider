using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace linqprovider
{
    public class ObjectReader<T> : IEnumerable<T> where T : class, new()
    {
        private Enumerator<T> _enumerator;

        internal ObjectReader(DbDataReader reader)
        {
            _enumerator = new Enumerator<T>(reader);
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

        public class Enumerator<T> : ProjectionRow, IEnumerator<T> where T : new()
        {
            readonly DbDataReader _reader;
            readonly FieldInfo[] _fields;
            int[] _fieldLookup;
            T _current;
            private Func<ProjectionRow, T> _projector;

            internal Enumerator(DbDataReader reader) : this(reader, null)
            {
            }

            internal Enumerator(DbDataReader reader, Func<ProjectionRow, T> projector)
            {
                _reader = reader;
                _fields = typeof(T).GetFields();
                _projector = projector;
            }

            public T Current => _current;

            object IEnumerator.Current => _current;

            public bool MoveNext()
            {
                if (_reader.Read())
                {
                    if (_fieldLookup == null)
                    {
                        InitFieldLookup();
                    }

                    T instance = new T();

                    for (int i = 0, n = _fields.Length; i < n; i++)
                    {
                        int index = _fieldLookup[i];

                        if (index >= 0)
                        {
                            FieldInfo fi = _fields[i];

                            var value = _reader.IsDBNull(index)
                                ? null
                                : _reader.GetValue(index);

                            fi.SetValue(instance, value);
                        }
                    }

                    _current = instance;

                    return true;
                }

                return false;
            }

            public override Object GetValue(int index)
            {
                if (index >= 0)
                {
                    if (_reader.IsDBNull(index))
                    {
                        return null;
                    }
                    return _reader.GetValue(index);
                }
                throw new IndexOutOfRangeException();
            }

            public void Reset()
            {
            }

            public void Dispose()
            {
                _reader.Dispose();
            }

            private void InitFieldLookup()
            {
                var map = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

                for (int i = 0, n = _reader.FieldCount; i < n; i++)
                {
                    map.Add(_reader.GetName(i), i);
                }

                _fieldLookup = new int[_fields.Length];

                for (int i = 0, n = _fields.Length; i < n; i++)
                {
                    int index;

                    if (map.TryGetValue(_fields[i].Name, out index))
                    {
                        _fieldLookup[i] = index;
                    }
                    else
                    {
                        _fieldLookup[i] = -1;
                    }
                }
            }
        }
    }
}