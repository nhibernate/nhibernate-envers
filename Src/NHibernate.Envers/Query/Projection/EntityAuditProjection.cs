using System;
using System.Collections;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;

namespace NHibernate.Envers.Query.Projection
{
	public class EntityAuditProjection : IAuditProjection
	{
		private readonly bool _distinct;

		public EntityAuditProjection(bool distinct)
		{
			_distinct = distinct;
		}

		public Tuple<string, string, bool> GetData(AuditConfiguration auditCfg)
		{
			// no property is selected, instead the whole entity (alias) is selected
			return new Tuple<string, string, bool>(null, null, _distinct);
		}

		public object ConvertQueryResult(AuditConfiguration auditCfg, EntityInstantiator entityInstantiator, string entityName,
			long revision, object value)
		{
			return auditCfg.EntCfg.IsVersioned(entityName) ? 
				entityInstantiator.CreateInstanceFromVersionsEntity(entityName, (IDictionary) value, revision) : 
				value;
		}
	}
}