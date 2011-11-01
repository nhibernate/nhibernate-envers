using System;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class MemberInfoAndAttribute
	{
		public MemberInfoAndAttribute(System.Type type, MemberInfo memberInfo, Attribute attribute)
		{
			MemberInfo = memberInfo;
			Attribute = attribute;
			Type = type;
		}

		public MemberInfoAndAttribute(System.Type type, Attribute attribute)
		{
			MemberInfo = type;
			Attribute = attribute;
			Type = type;
		}

		public MemberInfo MemberInfo { get; private set; }
		public Attribute Attribute { get; private set; }
		public System.Type Type { get; private set; }
	}
}