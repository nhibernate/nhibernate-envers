using System;
using System.Collections.Generic;
using System.Collections;

namespace NHibernate.Envers.Tools
{
    class EnumeratorWrapper<T> : IEnumerator<T>
    {
        IEnumerator e;
        public EnumeratorWrapper(IEnumerator e)
        {
            this.e = e;
        }

        #region IEnumerator<T> Members
        public T Current
        {
            get { return (T)e.Current; }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (e is IDisposable)
                ((IDisposable)e).Dispose();
        }
        #endregion

        #region IEnumerator Members
        object IEnumerator.Current
        {
            get { return e.Current; }
        }

        public bool MoveNext()
        {
            return e.MoveNext();
        }

        public void Reset()
        {
            e.Reset();
        }
        #endregion

    }

}
