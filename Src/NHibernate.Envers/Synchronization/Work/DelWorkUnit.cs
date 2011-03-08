using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Persister.Entity;

namespace NHibernate.Envers.Synchronization.Work
{
	public class DelWorkUnit : AbstractAuditWorkUnit 
	{
		private readonly object[] state;
		private readonly string[] propertyNames;
		private readonly IEntityPersister entityPersister;

		public DelWorkUnit(ISessionImplementor sessionImplementor, string entityName, AuditConfiguration verCfg,
						   object id, IEntityPersister entityPersister, object[] state)
			: base(sessionImplementor, entityName, verCfg, id)
		{
			this.state = state;
			propertyNames = entityPersister.PropertyNames;
			this.entityPersister = entityPersister;
		}

		public override bool ContainsWork()
		{
			return true;
		}

		public override IDictionary<string, object> GenerateData(object revisionData)
		{
			IDictionary<string, object> data = new Dictionary<string, object>();
			FillDataWithId(data, revisionData, RevisionType.Deleted);

			if (VerCfg.GlobalCfg.StoreDataAtDelete) {
				VerCfg.EntCfg[EntityName].PropertyMapper.Map(SessionImplementor, data,
						propertyNames, state, state);
			}

			return data;
		}

		public override IAuditWorkUnit Merge(AddWorkUnit second)
		{
			//needed to get onetoone pk work. What should happen with "normal" entities?
			return new ModWorkUnit(SessionImplementor, EntityName, VerCfg, EntityId, entityPersister, second.State, state);
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

