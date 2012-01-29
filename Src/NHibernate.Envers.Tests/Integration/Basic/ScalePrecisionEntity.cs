using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	[Audited]
	public class ScalePrecisionEntity
	{
		public virtual long Id { get; set; }
		public virtual double Number { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ScalePrecisionEntity;
			if (casted == null)
				return false;

			return Id == casted.Id && Number == casted.Number;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Number.GetHashCode();
		}
	}
}