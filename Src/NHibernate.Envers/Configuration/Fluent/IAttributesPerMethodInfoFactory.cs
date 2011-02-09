using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public interface IAttributesPerMethodInfoFactory
	{
		IDictionary<MemberInfo, IEnumerable<Attribute>> Create();
	}
}