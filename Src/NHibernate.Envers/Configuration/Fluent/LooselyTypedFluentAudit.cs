using System;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Fluent
{
	/// <summary>
	/// An <see cref="IAttributeProvider"/> to audit the full class.
	/// </summary>
	/// <seealso cref="IFluentAudit"/>
	/// <seealso cref="IFluentAudit{T}"/>
	/// <seealso cref="FluentAudit{T}"/>
	public class LooselyTypedFluentAudit : BaseFluentAudit, IFluentAudit
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
			AttributeCollection.Add(new MemberInfoAndAttribute(entityType, new AuditedAttribute()));
		}

		public IFluentAudit Exclude(string property)
		{
			Exclude(_entityType, property);
			return this;
		}

		public IFluentAudit ExcludeRelationData(string property)
		{
			ExcludeRelationData(_entityType, property);
			return this;
		}

		public IFluentAudit SetTableInfo(Action<AuditTableAttribute> tableInfo)
		{
			SetTableInfo(_entityType, tableInfo);
			return this;
		}

		public IFluentAudit SetTableInfo(string property, Action<AuditJoinTableAttribute> tableInfo)
		{
			SetTableInfo(_entityType, property, tableInfo);
			return this;
		}

		public IFluentAudit UseFactory(IEntityFactory factory)
		{
			AttributeCollection.Add(new MemberInfoAndAttribute(_entityType, new AuditFactoryAttribute(factory)));
			return this;
		}

		public IFluentAudit UseFactory<TFactory>() where TFactory: IEntityFactory
		{
			AttributeCollection.Add(new MemberInfoAndAttribute(_entityType, new AuditFactoryAttribute(typeof(TFactory))));
			return this;
		}
	}
}