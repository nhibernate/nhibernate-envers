using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public class IdBagCollectionMapper<T> : BagCollectionMapper<T>
	{
		public IdBagCollectionMapper(IEnversProxyFactory enversProxyFactory,
		                             CommonCollectionMapperData commonCollectionMapperData,
		                             System.Type proxyType,
		                             MiddleComponentData elementComponentData)
			: base(enversProxyFactory, commonCollectionMapperData, proxyType, elementComponentData)
		{
		}

		protected override IEnumerable GetOldCollectionContent(object oldCollection)
		{
			if (oldCollection == null)
			{
				return null;
			}
			var ret = new List<object>();
			foreach (var item in (IEnumerable) oldCollection)
			{
				//hack - can't get the snapshot value without reflection. Needs to be changed inside NH Core
				var itemValue = item.GetType().GetProperty("Value").GetValue(item, null);
				ret.Add(itemValue);
			}
			return ret;
		}
	}
}