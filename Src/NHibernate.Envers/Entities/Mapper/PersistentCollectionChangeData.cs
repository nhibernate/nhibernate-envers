using System.Collections.Generic;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Entities.Mapper
{
	public class PersistentCollectionChangeData 
	{
		private readonly object changedElement;

		public PersistentCollectionChangeData(string entityName, IDictionary<string, object> data, object changedElement) 
		{
			EntityName = entityName;
			Data = data;
			this.changedElement = changedElement;
		}

		public IDictionary<string, object> Data { get; private set; }
		public string EntityName { get; private set; }

		public object GetChangedElement()
		{
			var elementAsPair = changedElement as IPair;
			return elementAsPair != null ? elementAsPair.Second : keyValueOrDefault(changedElement, "Value", changedElement);
		}

		public object GetChangedElementIndex()
		{
			var elementAsPair = changedElement as IPair;
			return elementAsPair != null ? elementAsPair.First : keyValueOrDefault(changedElement, "Key", null);
		}

		private static object keyValueOrDefault(object changedElement, string keyOrValue, object defaultValue)
		{
			//rk hack - fix later
			var type = changedElement.GetType();
			if (type.IsGenericType)
			{
				if (type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
				{
					return type.GetProperty(keyOrValue).GetValue(changedElement, null);
				}
			}
			return defaultValue;
		}
	}
}
