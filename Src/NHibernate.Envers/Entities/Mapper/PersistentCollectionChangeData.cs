﻿using System;
using System.Collections.Generic;

namespace NHibernate.Envers.Entities.Mapper
{
	public class PersistentCollectionChangeData 
	{
		private readonly object _changedElement;

		public PersistentCollectionChangeData(string entityName, IDictionary<string, object> data, object changedElement) 
		{
			EntityName = entityName;
			Data = data;
			_changedElement = changedElement;
		}

		public IDictionary<string, object> Data { get; }
		public string EntityName { get; }

		public object GetChangedElement()
		{
			return _changedElement is Tuple<int, object> elementAsPair ? 
				elementAsPair.Item2 : 
				keyValueOrDefault(_changedElement, "Value", _changedElement);
		}

		public object GetChangedElementIndex()
		{
			var elementAsPair = _changedElement as Tuple<int, object>;
			return elementAsPair?.Item1 ?? keyValueOrDefault(_changedElement, "Key", null);
		}

		private static object keyValueOrDefault(object changedElement, string keyOrValue, object defaultValue)
		{
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
