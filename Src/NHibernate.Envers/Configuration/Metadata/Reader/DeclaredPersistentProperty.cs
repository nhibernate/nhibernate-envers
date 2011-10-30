using System.Reflection;
using NHibernate.Envers.Tools;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	public class DeclaredPersistentProperty
	{
		public static readonly MemberInfo NotAvailableMemberInfo = typeof (object).GetMethod("ToString");

		public DeclaredPersistentProperty(Property property, MemberInfo memberInfo)
		{
			ArgumentsTools.CheckNotNull(property, "property"); 
			ArgumentsTools.CheckNotNull(memberInfo, "memberInfo");
			Member = memberInfo;
			Property = property;
		}

		public MemberInfo Member { get; private set; }
		public Property Property { get; private set; }
	}
}