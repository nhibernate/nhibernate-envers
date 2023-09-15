using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Synchronization.Work
{
	public abstract partial class AbstractAuditWorkUnit : IAuditWorkUnit 
	{
		private object _performedData;

		protected AbstractAuditWorkUnit(ISessionImplementor sessionImplementor,			
										string entityName, 
										AuditConfiguration verCfg,
										object id,
										RevisionType revisionType) 
		{
			SessionImplementor = sessionImplementor;
			VerCfg = verCfg;
			EntityId = id;
			RevisionType = revisionType;
			EntityName = entityName;
		}

		public object EntityId { get; }
		public string EntityName { get; }
		public RevisionType RevisionType { get; }
		protected ISessionImplementor SessionImplementor { get; }
		protected AuditConfiguration VerCfg { get; }

		protected void FillDataWithId(IDictionary<string, object> data, object revision) 
		{
			var entitiesCfg = VerCfg.AuditEntCfg;

			var originalId = new Dictionary<string, object> {{entitiesCfg.RevisionFieldName, revision}};

			VerCfg.EntCfg[EntityName].IdMapper.MapToMapFromId(originalId, EntityId);
			data.Add(entitiesCfg.RevisionTypePropName, RevisionType);
			data.Add(entitiesCfg.OriginalIdPropName, originalId);
		}

		public virtual void Perform(ISession session, object revisionData) 
		{
			var data = GenerateData(revisionData);
			VerCfg.GlobalCfg.AuditStrategy.Perform(session, EntityName, EntityId, data, revisionData);
			_performedData = data;
		}

		public bool IsPerformed() 
		{
			return _performedData != null;
		}

		public void Undo(ISession session) 
		{
			if (IsPerformed()) 
			{
				session.Delete(VerCfg.AuditEntCfg.GetAuditEntityName(EntityName), _performedData);
				session.Flush();
			}
		}

		public abstract IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first);
		public abstract IAuditWorkUnit Merge(AddWorkUnit second);
		public abstract IAuditWorkUnit Merge(ModWorkUnit second);
		public abstract IAuditWorkUnit Merge(DelWorkUnit second);
		public abstract IAuditWorkUnit Merge(CollectionChangeWorkUnit second);
		public abstract IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second);
		public abstract IDictionary<string, object> GenerateData(object revisionData);
		public abstract bool ContainsWork();
	}
}
