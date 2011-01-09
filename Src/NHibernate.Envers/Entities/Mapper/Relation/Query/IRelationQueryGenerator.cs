using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
    /**
     * TODO: cleanup implementations and extract common code
     *
     * Implementations of this interface provide a method to generate queries on a relation table (a table used
     * for mapping relations). The query can select, apart from selecting the content of the relation table, also data of
     * other "related" entities.
     * @author Simon Duduica, port of omonyme class by Adam Warski (adam at warski dot org)
     */
    public interface IRelationQueryGenerator
    {
        IQuery GetQuery(IAuditReaderImplementor versionsReader, Object primaryKey, long revision);
    }
}
