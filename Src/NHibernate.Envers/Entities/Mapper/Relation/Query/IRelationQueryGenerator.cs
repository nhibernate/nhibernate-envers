using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	/// <summary>
	/// Implementations of this interface provide a method to generate queries on a relation table (a table used
	/// for mapping relations). The query can select, apart from selecting the content of the relation table, also data of
	/// other "related" entities.
	/// </summary>
	public interface IRelationQueryGenerator
	{
		IQuery GetQuery(IAuditReaderImplementor versionsReader, object primaryKey, long revision, bool removed);
	}
}
