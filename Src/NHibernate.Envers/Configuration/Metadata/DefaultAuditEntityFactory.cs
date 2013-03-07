using System;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Configuration.Metadata
{
	[Serializable]
	public class DefaultAuditEntityFactory : IEntityFactory
	{
		public object Instantiate(System.Type entityType)
		{
			return ReflectionTools.CreateInstanceByDefaultConstructor(entityType);
		}
	}
}