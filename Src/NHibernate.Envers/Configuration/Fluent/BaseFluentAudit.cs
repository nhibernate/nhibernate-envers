using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Fluent
{
	public abstract class BaseFluentAudit : IAttributeProvider
	{
		protected readonly ICollection<MemberInfoAndAttribute> AttributeCollection;
		private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		protected BaseFluentAudit()
		{
			AttributeCollection = new List<MemberInfoAndAttribute>();
		}

		protected void Exclude(System.Type entityType, string property)
		{
			var member = getMemberOrThrow(entityType, property);
			AttributeCollection.Add(new MemberInfoAndAttribute(entityType, member, new NotAuditedAttribute()));
		}

		protected void ExcludeRelationData(System.Type entityType, string property)
		{
			var member = getMemberOrThrow(entityType, property);
			var attr = new AuditedAttribute { TargetAuditMode = RelationTargetAuditMode.NotAudited };
			AttributeCollection.Add(new MemberInfoAndAttribute(entityType, member, attr));
		}

		protected void SetTableInfo(System.Type entityType, Action<AuditTableAttribute> tableInfo)
		{
			var attr = new AuditTableAttribute(string.Empty);
			tableInfo(attr);
			AttributeCollection.Add(new MemberInfoAndAttribute(entityType, attr));
		}

		protected void SetTableInfo(System.Type entityType, string property, Action<AuditJoinTableAttribute> tableInfo)
		{
			var member = getMemberOrThrow(entityType, property);
			var attr = new AuditJoinTableAttribute();
			tableInfo(attr);
			AttributeCollection.Add(new MemberInfoAndAttribute(entityType, member, attr));
		}

		private static MemberInfo getMemberOrThrow(System.Type entityType, string propertyName)
		{
			var member = entityType.GetField(propertyName, bindingFlags) ?? entityType.GetProperty(propertyName, bindingFlags) as MemberInfo;
			if (member == null)
			{
				var baseType = entityType.BaseType;
				if (baseType != null && baseType != typeof(object))
					return getMemberOrThrow(baseType, propertyName);
				throw new FluentException("Cannot find member " + propertyName + " on type " + entityType);
			}
			return member;
		}

		public IEnumerable<MemberInfoAndAttribute> Attributes(Cfg.Configuration nhConfiguration)
		{
			return AttributeCollection;
		}
	}
}
