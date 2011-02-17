using System;
using System.Collections.Generic;
using System.Linq;

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
		private readonly AuditedAttribute createClassAttributes = new AuditedAttribute();

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
			Type = entityType;
		}

		#region IAttributeProvider Members

		public System.Type Type { get; private set; }

		public IEnumerable<Attribute> CreateClassAttributes()
		{
			yield return createClassAttributes;
		}

		public IEnumerable<MemberInfoAndAttribute> CreateMemberAttributes()
		{
			return Enumerable.Empty<MemberInfoAndAttribute>();
		}

		#endregion
	}
}