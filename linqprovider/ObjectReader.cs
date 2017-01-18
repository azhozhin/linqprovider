using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

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
            Enumerator<T> e = _enumerator;

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
    }
}