using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Collection.Embeddable
{
	[Audited]
	public class DarkCharacter
	{
		public DarkCharacter()
		{
			Names = new HashSet<Name>();
		}

		public virtual int Id { get; set; }
		public virtual ISet<Name> Names { get; set; }
		public virtual int Kills { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as DarkCharacter;
			if (that == null)
				return false;
			return Id == that.Id && Kills == that.Kills;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Kills.GetHashCode();
		}
	}
}