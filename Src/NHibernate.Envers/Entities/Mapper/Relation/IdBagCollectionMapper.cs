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
																 MiddleComponentData elementComponentData,
																bool revisionTypeInId)
			: base(enversProxyFactory, commonCollectionMapperData, proxyType, elementComponentData, revisionTypeInId)
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
				//hack - need to know if element if of nh core's hidden type SnapshotElement
				var itemTypeName = item.GetType().FullName;
				if (itemTypeName.Contains("NHibernate.Collection.Generic.PersistentIdentifierBag") && itemTypeName.Contains("SnapshotElement"))
				{
					//hack again - can't get the snapshot value without reflection. Needs to be changed inside NH Core
					var itemValue = item.GetType().GetProperty("Value").GetValue(item, null);
					ret.Add(itemValue);					
				}
				else
				{
					ret.Add(item);
				}
			}
			return ret;
		}
	}
}