using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Envers.Tools
{
    public class ListWrapper<T> : IList<T>
    {

        IList list;

        public ListWrapper(IList list)
        {

            this.list = list;

        }

        #region IList<T> Members

        public int IndexOf(T item)
        {

            return list.IndexOf(item);

        }

        public void Insert(int index, T item)
        {

            list.Insert(index, item);

        }

        public void RemoveAt(int index)
        {

            list.RemoveAt(index);

        }

        public T this[int index]
        {

            get
            {

                return (T)list[index];

            }

            set
            {

                list[index] = value;

            }

        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {

            list.Add(item);

        }

        public void Clear()
        {

            list.Clear();

        }

        public bool Contains(T item)
        {

            return list.Contains(item);

        }

        public void CopyTo(T[] array, int arrayIndex)
        {

            list.CopyTo(array, arrayIndex);

        }

        public int Count
        {

            get { return list.Count; }

        }

        public bool IsReadOnly
        {

            get { return list.IsReadOnly; }

        }

        public bool Remove(T item)
        {

            bool contains = list.Contains(item);

            if (contains)

                list.Remove(item);

            return contains;

        }

        #endregion

        #region IEnumerable<T> Members



        public IEnumerator<T> GetEnumerator()
        {

            return new EnumeratorWrapper<T>(list.GetEnumerator());

        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {

            return list.GetEnumerator();

        }

        #endregion

    }
}