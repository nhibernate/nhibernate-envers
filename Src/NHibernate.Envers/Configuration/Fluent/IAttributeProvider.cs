using System;
using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Fluent
{
	public interface IAttributeProvider
	{
		System.Type Type { get; }
		IEnumerable<Attribute> CreateClassAttributes();
		IEnumerable<MemberInfoAndAttribute> CreateMemberAttributes();
	}
}