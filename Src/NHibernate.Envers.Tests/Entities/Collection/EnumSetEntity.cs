using Iesi.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.Collection
{
	public enum E1
	{
		X,
		Y
	}

	public enum E2
	{
		A,
		B
	}

	public class EnumSetEntity
	{
		public EnumSetEntity()
		{
			Enums1 = new HashedSet<E1>();
			Enums2 = new HashedSet<E2>();
		}

		public virtual int Id { get; set; }
		[Audited]
		public virtual ISet<E1> Enums1 { get; set;}
		[Audited]
		public virtual ISet<E2> Enums2 { get; set; }
	}
}