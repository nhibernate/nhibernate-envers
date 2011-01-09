using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tools
{
    class EnumeratorOfDictionariesWrapper<T, TKey, TValue>: IEnumerator<T> where T : IDictionary<TKey, TValue>
    {
        IEnumerator enu { get; set; }

        public EnumeratorOfDictionariesWrapper(IEnumerator enumerator)
        {
            enu = enumerator;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            return enu.MoveNext();
        }

        public void Reset()
        {
            enu.Reset();
        }

        public T Current
        {
            get { return (T)enu.Current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
