using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.CustomLists
{
	class CustomListTests
	{
		[Test]
		public void TestCustomLists()
		{
			var cfg = new Cfg.Configuration();
			cfg.Configure();
			cfg.AddResource("NHibernate.Envers.Tests.NetSpecific.UnitTests.CustomLists.Mapping.hbm.xml", GetType().Assembly);

			var ecfg = new FluentConfiguration();
			ecfg.Audit<AuditParent>().SetCollectionMapper<CustomCollectionMapperFactory<AuditChild>>(a => a.Children);
			ecfg.Audit<AuditChild>();

			// Throws exceptions without custon list hooks
			cfg.IntegrateWithEnvers(ecfg);
		}
	}

	internal class CustomCollectionMapperFactory<TItem> : ICustomCollectionMapperFactory
	{
		public IPropertyMapper Create(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType)
		{
			return new CustomListCollectionMapper<CustomList<TItem>, TItem>(enversProxyFactory, commonCollectionMapperData, typeof(CustomList<TItem>), elementComponentData, indexComponentData, embeddableElementType);
		}
	}

	[Serializable]
	public class CustomListCollectionMapper<TCustomList, TItem> : AbstractCollectionMapper
		where TCustomList : IList<TItem>, new()
	{
		private readonly MiddleComponentData _elementComponentData;
		private readonly MiddleComponentData _indexComponentData;

		public CustomListCollectionMapper(IEnversProxyFactory enversProxyFactory,
									CommonCollectionMapperData commonCollectionMapperData,
									System.Type proxyType,
									MiddleComponentData elementComponentData,
									MiddleComponentData indexComponentData,
									bool revisionTypeInId)
			: base(enversProxyFactory, commonCollectionMapperData, proxyType, false, revisionTypeInId)
		{
			_elementComponentData = elementComponentData;
			_indexComponentData = indexComponentData;
		}

		protected override IEnumerable GetNewCollectionContent(IPersistentCollection newCollection)
		{
			return newCollection == null ? null : Toolz.ListToIndexElementPairList((IList)newCollection);
		}

		protected override IEnumerable GetOldCollectionContent(object oldCollection)
		{
			return oldCollection == null ? null : Toolz.ListToIndexElementPairList((IList)oldCollection);
		}

		protected override void MapToMapFromObject(ISessionImplementor session, IDictionary<string, object> idData, IDictionary<string, object> data, object changed)
		{
			var indexValuePair = (Tuple<int, object>)changed;
			_elementComponentData.ComponentMapper.MapToMapFromObject(session, idData, data, indexValuePair.Item2);
			_indexComponentData.ComponentMapper.MapToMapFromObject(session, idData, data, indexValuePair.Item1);
		}

		protected override IInitializor GetInitializor(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, object primaryKey, long revision, bool removed)
		{
			return new CustomListCollectionInitializer<TCustomList, TItem>(verCfg,
												versionsReader,
												CommonCollectionMapperData.QueryGenerator,
												primaryKey,
												revision,
												removed,
												_elementComponentData,
												_indexComponentData);
		}
	}

	public class CustomListCollectionInitializer<TCustomList, TItem> : AbstractCollectionInitializor<TCustomList>
		where TCustomList : IList<TItem>, new()
	{
		private readonly MiddleComponentData _elementComponentData;
		private readonly MiddleComponentData _indexComponentData;

		public CustomListCollectionInitializer(AuditConfiguration verCfg,
		                                       IAuditReaderImplementor versionsReader,
		                                       IRelationQueryGenerator queryGenerator,
		                                       object primaryKey,
		                                       long revision,
																					bool removed,
		                                       MiddleComponentData elementComponentData,
		                                       MiddleComponentData indexComponentData)
			: base(verCfg, versionsReader, queryGenerator, primaryKey, revision, removed)
		{
			_elementComponentData = elementComponentData;
			_indexComponentData = indexComponentData;
		}

		protected override TCustomList InitializeCollection(int size)
		{
			var ret = new TCustomList();
			for (var i = 0; i < size; i++)
			{
				ret.Add(default(TItem));
			}
			return ret;
		}

		protected override void AddToCollection(TCustomList collection, object collectionRow)
		{
			var listRow = (IList) collectionRow;
			var elementData = listRow[_elementComponentData.ComponentIndex];
			var indexData = listRow[_indexComponentData.ComponentIndex];

			var element = (TItem) _elementComponentData.ComponentMapper.MapToObjectFromFullMap(EntityInstantiator,
			                                                                                   (IDictionary) elementData,
			                                                                                   null,
			                                                                                   Revision);
			var index = (int) _indexComponentData.ComponentMapper.MapToObjectFromFullMap(EntityInstantiator,
			                                                                             (IDictionary) indexData,
			                                                                             element,
			                                                                             Revision);
			collection[index] = element;
		}
	}
}
