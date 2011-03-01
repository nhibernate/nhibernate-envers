using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Synchronization.Work
{
	public abstract class AbstractAuditWorkUnit : IAuditWorkUnit 
	{
		private object performedData;

		protected AbstractAuditWorkUnit(ISessionImplementor sessionImplementor,			
										string entityName, 
										AuditConfiguration verCfg,
										object id) 
		{
			SessionImplementor = sessionImplementor;
			VerCfg = verCfg;
			EntityId = id;
			EntityName = entityName;
		}

		public object EntityId { get; private set; }
		public string EntityName { get; private set; }
		protected ISessionImplementor SessionImplementor { get; private set; }
		protected AuditConfiguration VerCfg { get; private set; }

		protected void FillDataWithId(IDictionary<string, object> data, object revision, RevisionType revisionType) 
		{
			var entitiesCfg = VerCfg.AuditEntCfg;

			var originalId = new Dictionary<string, object> {{entitiesCfg.RevisionFieldName, revision}};

			VerCfg.EntCfg[EntityName].IdMapper.MapToMapFromId(originalId, EntityId);
			data.Add(entitiesCfg.RevisionTypePropName, revisionType);
			data.Add(entitiesCfg.OriginalIdPropName, originalId);
		}

		public virtual void Perform(ISession session, object revisionData) 
		{
			var data = GenerateData(revisionData);
			session.Save(VerCfg.AuditEntCfg.GetAuditEntityName(EntityName), data);
			SetPerformed(data);
		}

		public bool IsPerformed() 
		{
			return performedData != null;
		}

		private void SetPerformed(object performedData) 
		{
			this.performedData = performedData;
		}

		public void Undo(ISession session) 
		{
			if (IsPerformed()) 
			{
				session.Delete(VerCfg.AuditEntCfg.GetAuditEntityName(EntityName), performedData);
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
