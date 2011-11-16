﻿using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Ids
{
	public class EmbIdWithCustomTypeTestEntity
	{
		public EmbIdWithCustomTypeTestEntity()
		{
			Str1 = string.Empty;
		}

		public virtual EmbIdWithCustomType Id { get; set; }
		[Audited]
		public virtual string Str1 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as EmbIdWithCustomTypeTestEntity;
			if (casted == null)
				return false;
			return Id.Equals(casted.Id) && Str1.Equals(casted.Str1);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Str1.GetHashCode();
		}
	}
}