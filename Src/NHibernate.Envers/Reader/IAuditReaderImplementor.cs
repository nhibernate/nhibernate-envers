using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Envers.Reader
{
    /**
     * An interface exposed by a VersionsReader to library-facing classes.
     * @author Adam Warski (adam at warski dot org)
     */
    public interface IAuditReaderImplementor : IAuditReader {
        ISessionImplementor SessionImplementor { get; }
        ISession Session { get; }
        IFirstLevelCache FirstLevelCache { get; }
    }
}
