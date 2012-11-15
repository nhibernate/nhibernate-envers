using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.IdBag.ManyToMany.UniDirectional
{
	[Audited]
	public class UniOwned
	{
		public virtual Guid Id { get; set; }
		public virtual int Number { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as UniOwned;
			if (that == null)
				return false;
			return Id == that.Id && Number == that.Number;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Number;
		}
	}
}