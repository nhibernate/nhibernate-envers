using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.TablePerClass.Discriminate
{
	[Audited]
	public class ClassTypeEntity
	{
		public const string BaseName = "base";
		public const string SubtypeName = "subtype";

		public virtual int Id { get; set; }
		public virtual string Type { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ClassTypeEntity;
			if(casted==null)
				return false;
			return Id == casted.Id && Type == casted.Type;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}