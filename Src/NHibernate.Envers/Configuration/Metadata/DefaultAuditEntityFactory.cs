using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Configuration.Metadata
{
	[Serializable]
	class DefaultAuditEntityFactory : IEntityFactory
	{
		public object Instantiate(System.Type entityType)
		{
			return ReflectionTools.CreateInstanceByDefaultConstructor(entityType);
		}
	}
}