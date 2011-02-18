using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Persister.Entity;

namespace NHibernate.Envers.Synchronization.Work
{
	public class ModWorkUnit: AbstractAuditWorkUnit, IAuditWorkUnit 
	{
		private readonly bool changes;        

		public ModWorkUnit(ISessionImplementor sessionImplementor, string entityName, AuditConfiguration verCfg, 
						   object id, IEntityPersister entityPersister, object[] newState, object[] oldState)
			: base(sessionImplementor, entityName, verCfg, id)
		{
			Data = new Dictionary<string, object>();
			changes = verCfg.EntCfg[EntityName].PropertyMapper.Map(sessionImplementor, Data,
					entityPersister.PropertyNames, newState, oldState);
		}

		public IDictionary<string, object> Data { get; private set; }

		public override bool ContainsWork() 
		{
			return changes;
		}

		public override IDictionary<string, object> GenerateData(object revisionData)
		{
			FillDataWithId(Data, revisionData, RevisionType.Modified);

			return Data;
		}

		public override IAuditWorkUnit Merge(AddWorkUnit second)
		{
			return this;
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
			return FakeBidirectionalRelationWorkUnit.Merge(second, this, second.NestedWorkUnit);
		}

		public override IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first)
		{
			return first.Merge(this);
		}
	}
}
