using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ComponentAsId
{
	[Serializable]
	public class ComponentAsId
	{
		public Entity2 Key2 { get; set; }
		public Entity1 Key1 { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (!(obj is ComponentAsId))
				return false;

			if (!ReferenceEquals(this, obj))
				return false;

			var other = obj as ComponentAsId;

			return Key1.Id == other.Key1.Id && Key2.Id == other.Key2.Id;
		}

		public override int GetHashCode()
		{
			return 1;
		}
	}

	[Audited]
	public class Entity1
	{
		public virtual int Id { get; set; }
	}

	[Audited]
	public class Entity2
	{
		public virtual int Id { get; set; }
	}

	[Audited]
	public class SomeEntUDF
	{
		public virtual ComponentAsId Id { get; set; }
	}

}
