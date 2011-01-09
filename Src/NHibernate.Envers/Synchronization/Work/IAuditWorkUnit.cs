using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Synchronization.Work
{
    /**
     * TODO: refactor constructors into factory methods
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public interface IAuditWorkUnit: IWorkUnitMergeVisitor, IWorkUnitMergeDispatcher {
    Object EntityId { get;}
    String EntityName { get;}
    
    bool ContainsWork();

    bool IsPerformed();

    /**
     * Perform this work unit in the given session.
     * @param session Session, in which the work unit should be performed.
     * @param revisionData The current revision data, which will be used to populate the work unit with the correct
     * revision relation.
     */
    void Perform(ISession session, Object revisionData);
    void Undo(ISession session);

    /**
     * @param revisionData The current revision data, which will be used to populate the work unit with the correct
     * revision relation.
     * @return Generates data that should be saved when performing this work unit.
     */
    IDictionary<String, Object> GenerateData(Object revisionData);
}
}
