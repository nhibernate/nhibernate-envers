using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Query.Impl;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class AuditQueryCreator {
        private readonly AuditConfiguration auditCfg;
        private readonly IAuditReaderImplementor auditReaderImplementor;

        public AuditQueryCreator(AuditConfiguration auditCfg, IAuditReaderImplementor auditReaderImplementor) {
            this.auditCfg = auditCfg;
            this.auditReaderImplementor = auditReaderImplementor;
        }

        /**
         * Creates a query, which will return entities satisfying some conditions (specified later),
         * at a given revision.
         * @param c Class of the entities for which to query.
         * @param revision Revision number at which to execute the query.
         * @return A query for entities at a given revision, to which conditions can be added and which
         * can then be executed. The result of the query will be a list of entities (beans), unless a
         * projection is added.
         */
        public IAuditQuery ForEntitiesAtRevision(System.Type c, long revision) {
            //throw new NotImplementedException("Query not implemented yet");
            ArgumentsTools.CheckNotNull(revision, "Entity revision");
            ArgumentsTools.CheckPositive(revision, "Entity revision");
            return new EntitiesAtRevisionQuery(auditCfg, auditReaderImplementor, c, revision);
        }

        /**
         * Creates a query, which selects the revisions, at which the given entity was modified.
         * Unless an explicit projection is set, the result will be a list of three-element arrays, containing:
         * <ol>
         * <li>the entity instance</li>
         * <li>revision entity, corresponding to the revision at which the entity was modified. If no custom
         * revision entity is used, this will be an instance of {@link org.hibernate.envers.DefaultRevisionEntity}</li>
         * <li>type of the revision (an enum instance of class {@link org.hibernate.envers.RevisionType})</li>.
         * </ol>
         * Additional conditions that the results must satisfy may be specified.
         * @param c Class of the entities for which to query.
         * @param selectEntitiesOnly If true, instead of a list of three-element arrays, a list of entites will be
         * returned as a result of executing this query.
         * @param selectDeletedEntities If true, also revisions where entities were deleted will be returned. The additional
         * entities will have revision type "delete", and contain no data (all fields null), except for the id field.
         * @return A query for revisions at which instances of the given entity were modified, to which
         * conditions can be added (for example - a specific id of an entity of class <code>c</code>), and which
         * can then be executed. The results of the query will be sorted in ascending order by the revision number,
         * unless an order or projection is added.
         */
        public IAuditQuery ForRevisionsOfEntity(System.Type c, bool selectEntitiesOnly, bool selectDeletedEntities) {
            //throw new NotImplementedException("Query not implemented yet");
            return new RevisionsOfEntityQuery(auditCfg, auditReaderImplementor, c, selectEntitiesOnly,selectDeletedEntities);
        }
    }
}
