using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DummyOrm.Repository
{
    public class PocoReader<T> : IPocoReader<T>
    {
        private readonly IEnumerator<T> _enumerator;
        
        public PocoReader(IDataReader dataReader, IPocoMapper pocoMapper)
        {
            _enumerator = new PocoMapperEnumerator(dataReader, pocoMapper);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class PocoMapperEnumerator : IEnumerator<T>
        {
            private readonly IDataReader _dataReader;
            private readonly IPocoMapper _pocoMapper;

            public PocoMapperEnumerator(IDataReader dataReader, IPocoMapper pocoMapper)
            {
                _dataReader = dataReader;
                _pocoMapper = pocoMapper;
            }

            public T Current { get; private set; }

            public void Dispose()
            {
                _dataReader.Dispose();
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (!_dataReader.Read())
                {
                    return false;
                }

                Current = (T)_pocoMapper.Map(_dataReader);

                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
    }
}