using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Store
{
	public class EntityMeta : IEntityMeta
	{
		public EntityMeta()
		{
			ClassMetas = new List<Attribute>();
			MemberMetas = new Dictionary<MemberInfo, IEnumerable<Attribute>>();
		}

		public IEnumerable<Attribute> ClassMetas { get; }
		public IDictionary<MemberInfo, IEnumerable<Attribute>> MemberMetas { get; }

		public void AddMemberMeta(MemberInfo member, Attribute envAttr)
		{
			if (!MemberMetas.TryGetValue(member, out _))
			{
				MemberMetas[member] = new List<Attribute>();
			}
			((IList<Attribute>)MemberMetas[member]).Add(envAttr);
		}

		public void AddClassMeta(Attribute envAttr)
		{
			((IList<Attribute>)ClassMetas).Add(envAttr);
		}
	}
}