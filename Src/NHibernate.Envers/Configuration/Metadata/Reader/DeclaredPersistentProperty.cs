using System.Reflection;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	public class DeclaredPersistentProperty
	{
		public MemberInfo Member { get; set; }
		public Property Property { get; set; }
	}
}