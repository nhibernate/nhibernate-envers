using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public abstract partial class AbstractModifiedFlagsEntityTest : TestBase
	{
		protected AbstractModifiedFlagsEntityTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected virtual bool ForceModifiedFlags
		{
			get { return true; }
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			if(ForceModifiedFlags)
			{
				configuration.SetEnversProperty(ConfigurationKey.GlobalWithModifiedFlag, true);
			}
		}

		protected IList QueryForPropertyHasChanged(System.Type type, object id, params string[] propertyNames)
		{
			var query = createForRevisionQuery(type, id, false);
			addHasChangedProperties(query, propertyNames);
			return query.GetResultList();
		}

		protected IList QueryForPropertyHasChangedWithDeleted(System.Type type, object id, params string[] propertyNames)
		{
			var query = createForRevisionQuery(type, id, true);
			addHasChangedProperties(query, propertyNames);
			return query.GetResultList();
		}

		protected IList QueryForPropertyHasNotChanged(System.Type type, object id, params string[] propertyNames)
		{
			var query = createForRevisionQuery(type, id, false);
			addHasNotChangedProperties(query, propertyNames);
			return query.GetResultList();
		}

		protected IList QueryForPropertyHasNotChangedWithDeleted(System.Type type, object id, params string[] propertyNames)
		{
			var query = createForRevisionQuery(type, id, true);
			addHasNotChangedProperties(query, propertyNames);
			return query.GetResultList();
		}


		private static void addHasChangedProperties(IAuditQuery query, IEnumerable<string> propertyNames)
		{
			foreach (var propertyName in propertyNames)
			{
				query.Add(AuditEntity.Property(propertyName).HasChanged());
			}
		}

		private static void addHasNotChangedProperties(IAuditQuery query, IEnumerable<string> propertyNames)
		{
			foreach (var propertyName in propertyNames)
			{
				query.Add(AuditEntity.Property(propertyName).HasNotChanged());
			}
		}

		private IAuditQuery createForRevisionQuery(System.Type type, object id, bool withDeleted)
		{
			return AuditReader().CreateQuery()
				.ForRevisionsOfEntity(type, false, withDeleted)
				.Add(AuditEntity.Id().Eq(id));
		}
	}
}