using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Mixed
{
	[Audited]
	public class NormalActivity : AbstractActivity
	{
		public override bool Equals(object obj)
		{
			var casted = obj as NormalActivity;
			if (casted == null)
				return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}