using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Reader
{
	public partial class CrossTypeRevisionChangesReader : ICrossTypeRevisionChangesReader
	{
		private readonly IAuditReaderImplementor _auditReaderImplementor;
		private readonly AuditConfiguration _verCfg;

		public CrossTypeRevisionChangesReader(IAuditReaderImplementor auditReaderImplementor, AuditConfiguration verCfg)
		{
			_auditReaderImplementor = auditReaderImplementor;
			_verCfg = verCfg;
		}

		public IEnumerable<object> FindEntities(long revision)
		{
			var entityTypes = FindEntityTypes(revision);
			var result = new List<object>();
			foreach (var type in entityTypes)
			{
				result.AddRange(_auditReaderImplementor.CreateQuery().ForEntitiesModifiedAtRevision(type.Item1, revision).GetResultList<object>());
			}
			return result;
		}

		public IEnumerable<object> FindEntities(long revision, RevisionType revisionType)
		{
			var entityTypes = FindEntityTypes(revision);
			var result = new List<object>();
			foreach (var type in entityTypes)
			{
				result.AddRange(_auditReaderImplementor.CreateQuery().ForEntitiesModifiedAtRevision(type.Item1, revision)
												.Add(new RevisionTypeAuditExpression(revisionType, "=")).GetResultList<object>());
			}
			return result;
		}

		public IDictionary<RevisionType, IEnumerable<object>> FindEntitiesGroupByRevisionType(long revision)
		{
			var entityTypes = FindEntityTypes(revision);
			var result = new Dictionary<RevisionType, IEnumerable<object>>();
			foreach (var revType in Enum.GetValues(typeof(RevisionType)))
			{
				var revisionType = (RevisionType)revType;
				var tempList = new List<object>();
				foreach (var type in entityTypes)
				{
					var list = _auditReaderImplementor.CreateQuery().ForEntitiesModifiedAtRevision(type.Item1, revision)
										.Add(new RevisionTypeAuditExpression(revisionType, "=")).GetResultList<object>();
					tempList.AddRange(list);
				}
				result[revisionType] = tempList;
			}
			return result;
		}

		public ISet<Tuple<string, System.Type>> FindEntityTypes(long revision)
		{
			ArgumentsTools.CheckPositive(revision, "revision");
			if (!_verCfg.GlobalCfg.IsTrackEntitiesChangedInRevisionEnabled)
			{
				throw new AuditException(@"This query is designed for Envers default mechanism of tracking entities modified in a given revision." +
											 " Extend DefaultTrackingModifiedEntitiesRevisionEntity, utilize ModifiedEntityNamesAttribute or set " +
											 "'nhibernate.envers.track_entities_changed_in_revision' parameter to true.");
			}
			var session = _auditReaderImplementor.Session;
			var sessionImplementor = _auditReaderImplementor.SessionImplementor;
			var revisions = new HashSet<long> { revision };
			var query = _verCfg.RevisionInfoQueryCreator.RevisionsQuery(session, revisions);
			var revisionInfo = query.UniqueResult();
			if (revisionInfo != null)
			{
				// If revision exists
				var entityNames = _verCfg.ModifiedEntityNamesReader.ModifiedEntityTypes(revisionInfo);
				if (entityNames != null)
				{
					// Generate result that contains entity names and corresponding CLR classes.
					var result = new HashSet<Tuple<string, System.Type>>();
					foreach (var entityName in entityNames)
					{
						result.Add(new Tuple<string, System.Type>(entityName, Toolz.ResolveEntityClass(sessionImplementor, entityName)));
					}
					return result;
				}
			}
			return new HashSet<Tuple<string, System.Type>>();
		}
	}
}