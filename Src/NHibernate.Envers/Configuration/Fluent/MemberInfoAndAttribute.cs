using System;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class MemberInfoAndAttribute
	{
		public MemberInfoAndAttribute(MemberInfo memberInfo, Attribute attribute)
		{
			MemberInfo = memberInfo;
			Attribute = attribute;
		}

		public MemberInfo MemberInfo { get; private set; }
		public Attribute Attribute { get; private set; }
	}
}