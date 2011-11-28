using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Fluent
{
	/// <summary>
	/// An <see cref="IAttributeProvider"/> to audit the full class.
	/// </summary>
	/// <remarks>
	/// The class will be audited using default values of <see cref="AuditedAttribute"/> without exclusions of properties.
	/// </remarks>
	/// <seealso cref="IFluentAudit{T}"/>
	/// <seealso cref="FluentAudit{T}"/>
	public class LooselyTypedFluentAudit : IAttributeProvider
	{
		private readonly System.Type _entityType;

		public LooselyTypedFluentAudit(System.Type entityType)
		{
			if (entityType == null)
			{
				throw new ArgumentNullException("entityType");
			}
			if (entityType.IsValueType)
			{
				throw new ArgumentOutOfRangeException("entityType", "Expected class type found:" + entityType);
			}
			_entityType = entityType;
		}

		public IEnumerable<MemberInfoAndAttribute> Attributes(Cfg.Configuration nhConfiguration)
		{
			yield return new MemberInfoAndAttribute(_entityType, new AuditedAttribute());
		}
	}
}