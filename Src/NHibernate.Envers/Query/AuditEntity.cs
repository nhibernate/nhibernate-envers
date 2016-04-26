﻿using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Query.Projection;
using NHibernate.Envers.Query.Property;

namespace NHibernate.Envers.Query
{
	public static class AuditEntity 
	{
		public static AuditId Id() 
		{
			return new AuditId();
		}

		/// <summary>
		/// Create restrictions, projections and specify order for a property of an audited entity.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static AuditProperty Property(string propertyName) 
		{
			return new AuditProperty(new EntityPropertyName(propertyName));
		}


		/// <summary>
		/// Create restrictions, projections and specify order for the revision number, corresponding to an audited entity.
		/// </summary>
		/// <returns></returns>
		public static AuditProperty RevisionNumber() 
		{
			return new AuditProperty(new RevisionNumberPropertyName());
		}

		/// <summary>
		/// Create restrictions, projections and specify order for a property of the revision entity, corresponding to an audited entity.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static AuditProperty RevisionProperty(string propertyName) 
		{
			return new AuditProperty(new RevisionPropertyPropertyName(propertyName));
		}

		/// <summary>
		/// Create restrictions, projections and specify order for the revision type, corresponding to an audited entity.
		/// </summary>
		/// <returns></returns>
		public static AuditProperty RevisionType() 
		{
			return new AuditProperty(new RevisionTypePropertyName());
		}

		/// <summary>
		/// Create restrictions on an id of a related entity.
		/// </summary>
		/// <param name="propertyName">Name of the property, which is the relation.</param>
		/// <returns></returns>
		public static AuditRelatedId RelatedId(string propertyName) 
		{
			return new AuditRelatedId(new EntityPropertyName(propertyName));
		}

		/// <summary>
		/// Return the conjuction of two criterions.
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static IAuditCriterion And(IAuditCriterion lhs, IAuditCriterion rhs) 
		{
			return new LogicalAuditExpression(lhs, rhs, "and");
		}

		/// <summary>
		/// Return the disjuction of two criterions.
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static IAuditCriterion Or(IAuditCriterion lhs, IAuditCriterion rhs) 
		{
			return new LogicalAuditExpression(lhs, rhs, "or");
		}

		/// <summary>
		/// Return the negation of a criterion.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static IAuditCriterion Not(IAuditCriterion expression) 
		{
			return new NotAuditExpression(expression);
		}

		/// <summary>
		/// Group criterions together in a single conjunction (A and B and C...).
		/// </summary>
		/// <returns></returns>
		public static AuditConjunction Conjunction() 
		{
			return new AuditConjunction();
		}

		/// <summary>
		/// Group criterions together in a single disjunction (A or B or C...).
		/// </summary>
		/// <returns></returns>
		public static AuditDisjunction Disjunction() 
		{
			return new AuditDisjunction();
		}

		/// <summary>
		/// Adds a projection to the current entity itself. 
		/// Useful for selecting entities which are reached through associations within the query.
		/// </summary>
		public static IAuditProjection SelectEntity(bool distinct)
		{
			return new EntityAuditProjection(distinct);
		}
	}
}
