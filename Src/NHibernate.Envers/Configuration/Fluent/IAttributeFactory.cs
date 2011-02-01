using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public interface IAttributeFactory
	{
		IDictionary<MemberInfo, IEnumerable<Attribute>> Create();
	}
}