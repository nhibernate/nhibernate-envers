using System.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.Collection
{
	public class StringListEntity
	{
		public StringListEntity()
		{
			Strings = new List<string>();
		}

		public virtual int Id { get; set; }
		[Audited]
		public virtual IList<string> Strings { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as StringSetEntity;
			return casted != null && casted.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
