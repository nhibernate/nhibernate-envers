using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Synchronization.Work
{
	public class CollectionChangeWorkUnit : AbstractAuditWorkUnit 
	{
		private readonly object entity;

		public CollectionChangeWorkUnit(ISessionImplementor session, string entityName, AuditConfiguration verCfg,
										object id, object entity)
			: base(session, entityName, verCfg, id, RevisionType.Modified)
		{
			this.entity = entity;
		}

		public override bool ContainsWork()
		{
			return true;
		}

		public override IDictionary<string, object> GenerateData(object revisionData)
		{
			IDictionary<string, object> data = new Dictionary<string, object>();
			FillDataWithId(data, revisionData);

			VerCfg.EntCfg[EntityName].PropertyMapper.MapToMapFromEntity(SessionImplementor,
					data, entity, null);

			return data;
		}

		public override IAuditWorkUnit Merge(AddWorkUnit second) 
		{
			return second;
		}

		public override IAuditWorkUnit Merge(ModWorkUnit second)
		{
			return second;
		}

		public override IAuditWorkUnit Merge(DelWorkUnit second)
		{
			return second;
		}

		public override IAuditWorkUnit Merge(CollectionChangeWorkUnit second)
		{
			return this;
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
