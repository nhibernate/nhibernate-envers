using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Persister.Entity;

namespace NHibernate.Envers.Synchronization.Work
{
	public class ModWorkUnit: AbstractAuditWorkUnit 
	{
		private readonly bool _changes;
		private readonly IEntityPersister _entityPersister;
		private readonly object[] _oldState;
		private readonly object[] _newState;

		public ModWorkUnit(ISessionImplementor sessionImplementor, string entityName, AuditConfiguration verCfg, 
						   object id, IEntityPersister entityPersister, object[] newState, object[] oldState)
			: base(sessionImplementor, entityName, verCfg, id, RevisionType.Modified)
		{
			_entityPersister = entityPersister;
			_oldState = oldState;
			_newState = newState;
			Data = new Dictionary<string, object>();
			_changes = verCfg.EntCfg[EntityName].PropertyMapper.Map(sessionImplementor, Data,
					entityPersister.PropertyNames, newState, oldState);
		}

		public IDictionary<string, object> Data { get; private set; }

		public override bool ContainsWork() 
		{
			return _changes;
		}

		public override IDictionary<string, object> GenerateData(object revisionData)
		{
			FillDataWithId(Data, revisionData);

			return Data;
		}

		public override IAuditWorkUnit Merge(AddWorkUnit second)
		{
			return this;
		}

		public override IAuditWorkUnit Merge(ModWorkUnit second)
		{
			// In case of multiple subsequent flushes within single transaction, modification flags need to be
			// recalculated against initial and final state of the given entity.
			return new ModWorkUnit(second.SessionImplementor, second.EntityName, second.VerCfg, second.EntityId,
			                       second._entityPersister, second._newState, _oldState);
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
