using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities.Components.Relations;

namespace NHibernate.Envers.Tests.Entities.Collection
{
	[Audited]
	public class EmbeddableListEntity2
	{
		public EmbeddableListEntity2()
		{
			ComponentList = new List<ManyToOneEagerComponent>();
		}
		public virtual int Id { get; set; }
		public virtual IList<ManyToOneEagerComponent> ComponentList { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as EmbeddableListEntity2;
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