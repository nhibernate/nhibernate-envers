using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities.Components;

namespace NHibernate.Envers.Tests.Entities.Collection
{
	[Audited]
	public class EmbeddableSetEntity
	{
		public EmbeddableSetEntity()
		{
			ComponentSet = new HashSet<Component3>();
		}

		public virtual int Id { get; set; }
		public virtual ISet<Component3> ComponentSet { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as EmbeddableSetEntity;
			if (that == null)
				return false;
			return Id == that.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}