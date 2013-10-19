using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Properties;
using NHibernate.UserTypes;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.CustomLists
{
	public interface ICustomEnumerable<T> : IEnumerable<T>, INotifyCollectionChanged
	{
	}

	public interface ICustomList<T> : ICustomEnumerable<T>, IList<T>, INotifyPropertyChanged
	{
	}

	public class CustomList<T> : ObservableCollection<T>, ICustomList<T>
	{
		public CustomList()
		{
		}

		public CustomList(IEnumerable<T> collection)
			: base(collection)
		{
		}
	}

	public class CustomBagType<T> : IUserCollectionType
	{
		public bool Contains(object collection, object entity)
		{
			return ((IList<T>)collection).Contains((T)entity);
		}

		public IEnumerable GetElements(object collection)
		{
			return (IEnumerable)collection;
		}

		public object IndexOf(object collection, object entity)
		{
			return ((IList<T>)collection).IndexOf((T)entity);
		}

		public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			IList<T> result = (IList<T>)target;
			result.Clear();
			foreach (object item in ((IEnumerable)original))
				result.Add((T)item);
			return result;
		}

		// return an instance of the inner collection type
		public object Instantiate(int anticipatedSize)
		{
			return new CustomList<T>();
		}

		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentCustomBag<T>(session);
		}

		public IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentCustomBag<T>(session, (ICollection<T>)collection);
		}
	}

	[Serializable, DebuggerTypeProxy(typeof(NHibernate.DebugHelpers.CollectionProxy<>))]
	public class PersistentCustomBag<T> : PersistentGenericBag<T>, ICustomList<T>
	{
		public PersistentCustomBag(ISessionImplementor session)
			: base(session)
		{
		}

		public PersistentCustomBag(ISessionImplementor session, ICollection<T> coll)
			: base(session, coll)
		{
			if (coll != null)
			{
				((INotifyCollectionChanged)coll).CollectionChanged += OnCollectionChanged;
				((INotifyPropertyChanged)coll).PropertyChanged += OnPropertyChanged;
			}
		}

		// We used to do this in BeforeInitialize, but CollectionChanged events were getting fired while NHibernate was loading the collection.
		public override bool AfterInitialize(ICollectionPersister persister)
		{
			bool result = base.AfterInitialize(persister);
			throw new NotSupportedException("not yet ported to 2.0");
			//((INotifyCollectionChanged)InternalBag).CollectionChanged += OnCollectionChanged;
			//((INotifyPropertyChanged)InternalBag).PropertyChanged += OnPropertyChanged;
			return result;
		}

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (CollectionChanged != null) CollectionChanged(this, args);
		}

		#endregion

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Count" && PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs("Count"));
		}

		#endregion
	}

	public class AutomaticCustomCollection<T> : ICustomList<T>, IList
	{
		public event Action<T> ItemAdded;
		public event Action<T> ItemRemoved;

		/// <summary>
		/// If we're performing our own update to the list, we want to fire the ItemAdded and ItemRemoved
		/// events before we fire the NotifyCollectionChanged events.  As such, we use this flag to tell
		/// our handler to ignore 
		/// </summary>
		protected bool IgnoreNextCollectionChangedEvent { get; set; }

		public ICustomList<T> List
		{
			get { return _List; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (!(value is IList))
					throw new ArgumentException("AutomaticCustomCollection can only be used to wrap objects that implement IList.");

				if (_List != null)
				{
					_List.CollectionChanged -= WatchedList_CollectionChanged;
					_List.PropertyChanged -= _List_PropertyChanged;
				}

				_List = value;

				_List.CollectionChanged += WatchedList_CollectionChanged;
				_List.PropertyChanged += _List_PropertyChanged;

				NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}
		protected ICustomList<T> _List;

		public AutomaticCustomCollection(ICustomList<T> list)
		{
			List = list;
		}

		protected void FireAdd(T item)
		{
			if (ItemAdded != null)
				ItemAdded(item);
		}

		protected void FireRemove(T item)
		{
			if (ItemRemoved != null)
				ItemRemoved(item);
		}

		void _List_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Count")
				NotifyPropertyChanged("Count");
		}

		void WatchedList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!IgnoreNextCollectionChangedEvent)
				if (CollectionChanged != null)
					CollectionChanged(this, e);

			IgnoreNextCollectionChangedEvent = false;
		}

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return List.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			IgnoreNextCollectionChangedEvent = true;
			List.Insert(index, item);
			FireAdd(item);
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		public void RemoveAt(int index)
		{
			IgnoreNextCollectionChangedEvent = true;
			T oldItem = List[index];
			List.RemoveAt(index);
			FireRemove(oldItem);
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
		}

		public T this[int index]
		{
			get
			{
				return List[index];
			}
			set
			{
				IgnoreNextCollectionChangedEvent = true;
				T oldItem = List[index];
				List[index] = value;
				FireRemove(oldItem);
				FireAdd(value);
				NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			IgnoreNextCollectionChangedEvent = true;
			List.Add(item);
			FireAdd(item);
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, List.Count - 1));
		}

		public void Clear()
		{
			IgnoreNextCollectionChangedEvent = true;
			T[] oldItems = List.ToArray();
			List.Clear();
			foreach (T oldItem in oldItems)
				FireRemove(oldItem);
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public bool Contains(T item)
		{
			return List.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return List.Count; }
		}

		public bool IsReadOnly
		{
			get { return List.IsReadOnly; }
		}

		public bool Remove(T item)
		{
			int index = List.IndexOf(item);
			if (index == -1)
				return false;

			IgnoreNextCollectionChangedEvent = true;
			List.RemoveAt(index);
			FireRemove(item);
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
			return true;
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return List.GetEnumerator();
		}

		#endregion

		#region IList Members

		public int Add(object value)
		{
			IgnoreNextCollectionChangedEvent = true;
			int result = ((IList)_List).Add(value);
			FireAdd((T)value);
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, result));
			return result;
		}

		public bool Contains(object value)
		{
			return Contains((T)value);
		}

		public int IndexOf(object value)
		{
			return IndexOf((T)value);
		}

		public void Insert(int index, object value)
		{
			Insert(index, (T)value);
		}

		public bool IsFixedSize
		{
			get { return false; } //_IList.IsFixedSize; }
		}

		public void Remove(object value)
		{
			Remove((T)value);
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T)value;
			}
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			((IList)_List).CopyTo(array, index);
		}

		public bool IsSynchronized
		{
			get { return ((IList)_List).IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return ((IList)_List).SyncRoot; }
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return List.GetEnumerator();
		}

		#endregion

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, args);
		}

		#endregion

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}

	public class InternalPropertyAccessor : IPropertyAccessor
	{
		private BasicPropertyAccessor accessor = new BasicPropertyAccessor();

		public Boolean CanAccessThroughReflectionOptimizer
		{
			get
			{
				return false;
			}
		}

		public IGetter GetGetter(System.Type theClass, String propertyName)
		{
			return accessor.GetGetter(theClass, (propertyName + "Internal"));
		}

		public ISetter GetSetter(System.Type theClass, String propertyName)
		{
			return accessor.GetSetter(theClass, (propertyName + "Internal"));
		}
	}
}
