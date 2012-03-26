namespace NHibernate.Envers.Tests.Entities.CustomType
{
	public class PrimitiveImmutableType
	{
		protected PrimitiveImmutableType() { }

		public char CharValue { get; protected set; }

		public int IntValue { get; protected set; }

		public static PrimitiveImmutableType Get(int intValue)
		{
			return new PrimitiveImmutableType
						{
							CharValue = testString.ToCharArray()[intValue],
							IntValue = intValue
						};
		}

		private const string testString = "abcdefghijklmnopqrstuvwxyz";
	}
}
