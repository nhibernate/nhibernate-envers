using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Ids
{
	public class DateIdTestEntity
	{
		public virtual DateTime Id { get; set; }
		[Audited]
		public virtual string Str1 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as CompositeDateIdTestEntity;
			if (casted == null)
				return false;
			if (!Id.Equals(casted.Id))
				return false;
			if (Str1 != null ? !Str1.Equals(casted.Str1) : casted.Str1 != null)
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			var result = Id.GetHashCode();
			result = 31 * result + (Str1 != null ? Str1.GetHashCode() : 0);
			return result;
		} 
	}
}