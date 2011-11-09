using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Merge
{
	[Audited]
	public class GivenIdStrEntity
	{
		public GivenIdStrEntity()
		{
			Data = string.Empty;
		}

		public virtual int Id { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as GivenIdStrEntity;
			if (casted == null)
				return false;
			return Id == casted.Id && string.Equals(Data, casted.Data);
		}
	}
}