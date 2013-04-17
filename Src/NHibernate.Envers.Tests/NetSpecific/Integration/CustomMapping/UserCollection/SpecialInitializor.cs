using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class SpecialInitializor : BagCollectionInitializor<Number>
	{
		public SpecialInitializor(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, IRelationQueryGenerator queryGenerator, object primaryKey, long revision, bool removed, MiddleComponentData elementComponentData) : base(verCfg, versionsReader, queryGenerator, primaryKey, revision, removed, elementComponentData)
		{
		}

		protected override System.Collections.Generic.IList<Number> InitializeCollection(int size)
		{
			return new SpecialCollection(10);
		}
	}
}