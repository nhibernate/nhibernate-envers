using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	/// <summary>
	///  A class holding information about ids, which form a virtual "relation" from a middle-table. Middle-tables are used
	///  when mapping collections.
	/// </summary>
	[Serializable]
	public sealed class MiddleIdData
	{
		public MiddleIdData(AuditEntitiesConfiguration verEntCfg, IdMappingData mappingData, string prefix,
							string entityName, bool audited)
		{
			OriginalMapper = mappingData.IdMapper;
			PrefixedMapper = mappingData.IdMapper.PrefixMappedProperties(prefix);
			EntityName = entityName;
			AuditEntityName = audited ? verEntCfg.GetAuditEntityName(entityName) : null;
		}

		public IIdMapper OriginalMapper { get; private set; }
		public IIdMapper PrefixedMapper { get; private set; }
		public string EntityName { get; private set; }
		public string AuditEntityName { get; private set; }

		/// <summary>
		/// Is the entity, to which this middle id data correspond, audited.
		/// </summary>
		/// <returns></returns>
		public bool IsAudited()
		{
			return AuditEntityName != null;
		}
	}
}
