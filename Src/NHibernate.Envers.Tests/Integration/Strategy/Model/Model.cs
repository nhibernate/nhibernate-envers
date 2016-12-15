using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Strategy.Model
{
	[Audited]
	public class Product
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Item> Items { get; set; }
	}

	[Audited]
	public class Item
	{
		public virtual string Name { get; set; }
		public virtual int? Value { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as Item;
			if (that == null)
				return false;
			return Name.Equals(that.Name) && Value == that.Value;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Value.GetHashCode();
		}
	}
}