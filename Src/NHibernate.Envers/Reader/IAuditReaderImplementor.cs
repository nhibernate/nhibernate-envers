using NHibernate.Engine;

namespace NHibernate.Envers.Reader
{
	/// <summary>
	/// An interface exposed by a VersionsReader to library-facing classes.
	/// </summary>
	public interface IAuditReaderImplementor : IAuditReader
	{
		ISessionImplementor SessionImplementor { get; }
		ISession Session { get; }
		FirstLevelCache FirstLevelCache { get; }
	}
}
