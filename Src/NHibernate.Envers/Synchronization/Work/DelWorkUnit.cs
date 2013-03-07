using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools;
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
			: base(sessionImplementor, entityName, verCfg, id, RevisionType.Deleted)
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
			FillDataWithId(data, revisionData);

			VerCfg.EntCfg[EntityName].PropertyMapper
				.Map(SessionImplementor, data, propertyNames, VerCfg.GlobalCfg.StoreDataAtDelete ? state : null, state);

			return data;
		}

		public override IAuditWorkUnit Merge(AddWorkUnit second)
		{
			return Toolz.ArraysEqual(second.State, state)
			       	? null
			       	: new ModWorkUnit(SessionImplementor, EntityName, VerCfg, EntityId, entityPersister, second.State, state);
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

