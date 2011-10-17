using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities.Ids;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Query.RelationIn
{
	[Audited]
	public class EntityReferingEmbEntity 
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
		public virtual EmbIdTestEntity Reference { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as EntityReferingEmbEntity;
			if (casted == null)
				return false;
			return (casted.Id == Id && string.Equals(casted.Data, Data));
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}