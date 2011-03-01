using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Persister.Entity;

namespace NHibernate.Envers.Synchronization.Work
{
	public class AddWorkUnit : AbstractAuditWorkUnit, IAuditWorkUnit 
	{
		private readonly IDictionary<string, object> data;

		public AddWorkUnit(ISessionImplementor sessionImplementor, string entityName, AuditConfiguration verCfg,
						   object id, IEntityPersister entityPersister, object[] state) 
			: base(sessionImplementor, entityName, verCfg, id)
		{
			State = state;
			data = new Dictionary<string, object>();
			verCfg.EntCfg[EntityName].PropertyMapper.Map(sessionImplementor, data,
					entityPersister.PropertyNames, state, null);
		}

		public AddWorkUnit(ISessionImplementor sessionImplementor, string entityName, AuditConfiguration verCfg,
						   object id, IDictionary<string, object> data)
			: base(sessionImplementor, entityName, verCfg, id)
		{
			this.data = data;
		}

		internal object[] State { get; private set; }

		public override bool ContainsWork()
		{
			return true;
		}

		public override IDictionary<string, object> GenerateData(object revisionData)
		{
			FillDataWithId(data, revisionData, RevisionType.Added);
			return data;
		}

		public override IAuditWorkUnit Merge(AddWorkUnit second)
		{
			return second;
		}

		public override IAuditWorkUnit Merge(ModWorkUnit second)
		{
			return new AddWorkUnit(SessionImplementor, EntityName, VerCfg, EntityId, second.Data);
		}

		public override IAuditWorkUnit Merge(DelWorkUnit second)
		{
			return null;
		}

		public override IAuditWorkUnit Merge(CollectionChangeWorkUnit second)
		{
			return this;
		}

		public override IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second)
		{
			return FakeBidirectionalRelationWorkUnit.Merge(second, this, second.NestedWorkUnit);
		}

		public override IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first)
		{
			return first.Merge(this);
		}
	}
}
