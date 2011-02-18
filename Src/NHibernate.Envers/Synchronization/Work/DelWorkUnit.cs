using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Persister.Entity;

namespace NHibernate.Envers.Synchronization.Work
{
	public class DelWorkUnit : AbstractAuditWorkUnit, IAuditWorkUnit 
	{
		private readonly object[] state;
		private readonly string[] propertyNames;

		public DelWorkUnit(ISessionImplementor sessionImplementor, string entityName, AuditConfiguration verCfg,
						   object id, IEntityPersister entityPersister, object[] state)
			: base(sessionImplementor, entityName, verCfg, id)
		{
			this.state = state;
			propertyNames = entityPersister.PropertyNames;
		}

		public override bool ContainsWork()
		{
			return true;
		}

		public override IDictionary<string, object> GenerateData(object revisionData)
		{
			IDictionary<string, object> data = new Dictionary<string, object>();
			FillDataWithId(data, revisionData, RevisionType.Deleted);

			if (verCfg.GlobalCfg.StoreDataAtDelete) {
				verCfg.EntCfg[EntityName].PropertyMapper.Map(sessionImplementor, data,
						propertyNames, state, state);
			}

			return data;
		}

		public override IAuditWorkUnit Merge(AddWorkUnit second)
		{
			return null;
		}

		public override IAuditWorkUnit Merge(ModWorkUnit second)
		{
			return null;
		}

		public override IAuditWorkUnit Merge(DelWorkUnit second)
		{
			return this;
		}

		public override IAuditWorkUnit Merge(CollectionChangeWorkUnit second)
		{
			return this;
		}

		public override IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second)
		{
			return this;
		}

		public override IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first)
		{
			return first.Merge(this);
		}
	}
}

