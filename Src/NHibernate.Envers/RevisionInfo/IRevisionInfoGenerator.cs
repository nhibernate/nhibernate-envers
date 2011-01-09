using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.RevisionInfo
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public interface IRevisionInfoGenerator
    {
        void saveRevisionData(ISession session, Object revisionData);
        Object generate();
    }
}
