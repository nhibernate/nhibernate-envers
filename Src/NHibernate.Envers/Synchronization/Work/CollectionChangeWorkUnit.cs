using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Synchronization.Work
{
	public class CollectionChangeWorkUnit : AbstractAuditWorkUnit 
	{
		private readonly string _collectionPropertyName;
		private readonly object _entity;
		private readonly IDictionary<string, object> _data;

		public CollectionChangeWorkUnit(ISessionImplementor session, string entityName, string collectionPropertyName, AuditConfiguration verCfg,
											object id, object entity)
			: base(session, entityName, verCfg, id, RevisionType.Modified)
		{
			_collectionPropertyName = collectionPropertyName;
			_entity = entity;
			_data = new Dictionary<string, object>();
		}

		public override bool ContainsWork()
		{
			return true;
		}

		public override IDictionary<string, object> GenerateData(object revisionData)
		{
			FillDataWithId(_data, revisionData);

			var preGenerateData = new Dictionary<string, object>(_data);
			VerCfg.EntCfg[EntityName].PropertyMapper
				.MapToMapFromEntity(SessionImplementor, _data, _entity, null);
			VerCfg.EntCfg[EntityName].PropertyMapper
				.MapModifiedFlagsToMapFromEntity(SessionImplementor, _data, _entity, _entity);
			VerCfg.EntCfg[EntityName].PropertyMapper
				.MapModifiedFlagsToMapForCollectionChange(_collectionPropertyName, _data);
			foreach (var item in preGenerateData)
			{
				_data[item.Key] = item.Value;
			}
			return _data;
		}

		public override IAuditWorkUnit Merge(AddWorkUnit second) 
		{
			return second;
		}

		public override IAuditWorkUnit Merge(ModWorkUnit second)
		{
			MergeCollectionModifiedData(second.Data);
			return second;
		}

		public override IAuditWorkUnit Merge(DelWorkUnit second)
		{
			return second;
		}

		public override IAuditWorkUnit Merge(CollectionChangeWorkUnit second)
		{
			second.MergeCollectionModifiedData(_data);
			return this;
		}

		public void MergeCollectionModifiedData(IDictionary<string, object> data)
		{
			VerCfg.EntCfg[EntityName].PropertyMapper.MapModifiedFlagsToMapForCollectionChange(_collectionPropertyName, data);
		}

		public override IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second)
		{
			return second;
		}

		public override IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first)
		{
			return first.Merge(this);
		}
	}
}
