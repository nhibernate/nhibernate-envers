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

		public IEnumerable<Attribute> ClassMetas { get; set; }
		public IDictionary<MemberInfo, IEnumerable<Attribute>> MemberMetas { get; set; }

		public void AddMemberMeta(MemberInfo member, Attribute envAttr)
		{
			IEnumerable<Attribute> attributes;
			if (!MemberMetas.TryGetValue(member, out attributes))
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