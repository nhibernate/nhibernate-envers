using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.TablePerClass.Discriminate
{
	[Audited]
	public class OtherChildEntity : ParentEntity
	{
		public virtual string OtherSpecificData { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as OtherChildEntity;
			if (casted == null || !base.Equals(casted))
				return false;
			return OtherSpecificData.Equals(casted.OtherSpecificData);
		}

		public override int GetHashCode()
		{
			var res = base.GetHashCode(); return base.GetHashCode() ^ OtherSpecificData.GetHashCode();
		}


	}
}
