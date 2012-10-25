using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Ids.EmbeddedId
{
	[Audited]
	public class Producer
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public override bool Equals(object obj)
		{
			var producer = obj as Producer;
			if (producer == null)
				return false;
			if (Id != producer.Id)
				return false;
			if (Name != null ? !Name.Equals(producer.Name) : producer.Name != null) 
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			var result = Id.GetHashCode();
			result = 31*result + (Name != null ? Name.GetHashCode() : 0);
			return result;
		}
	}
}