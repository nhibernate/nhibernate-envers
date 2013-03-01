using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities.Components;

namespace NHibernate.Envers.Tests.Entities.Collection
{
	[Audited]
	public class EmbeddableListEntity1
	{
		public EmbeddableListEntity1()
		{
			ComponentList = new List<Component3>();
		}

		public virtual int Id { get; set; }
		public virtual string OtherData { get; set; }
		public virtual IList<Component3> ComponentList { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as EmbeddableListEntity1;
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