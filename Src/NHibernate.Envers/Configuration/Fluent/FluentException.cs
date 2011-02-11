using System;
using System.Runtime.Serialization;

namespace NHibernate.Envers.Configuration.Fluent
{
	[Serializable]
	public class FluentException : Exception
	{
		public FluentException()
		{
		}

		public FluentException(string message) : base(message)
		{
		}

		public FluentException(string message, Exception inner) : base(message, inner)
		{
		}

		protected FluentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}