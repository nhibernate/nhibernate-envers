using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities.Components;

namespace NHibernate.Envers.Tests.Entities.Collection
{
	public class EmbeddableMapEntity
	{
		public EmbeddableMapEntity()
		{
			ComponentMap = new Dictionary<string, Component3>();
		}

		public virtual int Id { get; set; }
		[Audited]
		public virtual IDictionary<string, Component3> ComponentMap { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as EmbeddableMapEntity;
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