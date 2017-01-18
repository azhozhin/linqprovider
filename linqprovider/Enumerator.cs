using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace linqprovider
{
    public class Enumerator<T> : IEnumerator<T> where T : new()
    {
        readonly DbDataReader _reader;
        readonly FieldInfo[] _fields;
        int[] _fieldLookup;
        T _current;

        internal Enumerator(DbDataReader reader)
        {
            _reader = reader;
            _fields = typeof(T).GetFields();
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

                        fi.SetValue(instance, _reader.IsDBNull(index)
                            ? null
                            : _reader.GetValue(index));
                    }
                }

                _current = instance;

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