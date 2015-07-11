using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DummyOrm.Execution;

namespace DummyOrm.Repository
{
    /// <summary>
    /// Adapter: DataReader -> IEnumerable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PocoReader<T> : IPocoReader<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public PocoReader(IDataReader dataReader, IPocoDeserializer pocoDeserializer)
        {
            _enumerator = new PocoMapperEnumerator(dataReader, pocoDeserializer);
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
            private readonly IPocoDeserializer _pocoDeserializer;

            public PocoMapperEnumerator(IDataReader dataReader, IPocoDeserializer pocoDeserializer)
            {
                _dataReader = dataReader;
                _pocoDeserializer = pocoDeserializer;
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

                Current = (T)_pocoDeserializer.Deserialize(_dataReader);

                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
    }
}