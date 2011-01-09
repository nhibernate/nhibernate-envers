using System;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Query.Property;

namespace NHibernate.Envers.Query
{
	/**
	 * TODO: ilike
	 * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
	 * @see org.hibernate.criterion.Restrictions
	 */
	public class AuditEntity {
		private AuditEntity() { }

		public static AuditId Id() {
			return new AuditId();
		}

		/**
		 * Create restrictions, projections and specify order for a property of an audited entity.
		 * @param propertyName Name of the property.
		 */
		public static AuditProperty<Object> Property(String propertyName) {
			return new AuditProperty<Object>(new EntityPropertyName(propertyName));
		}

	   /**
		 * Create restrictions, projections and specify order for the revision number, corresponding to an
		 * audited entity.
		 */
		public static AuditProperty<long> RevisionNumber() {
			return new AuditProperty<long>(new RevisionNumberPropertyName());
		}

		/**
		 * Create restrictions, projections and specify order for a property of the revision entity,
		 * corresponding to an audited entity.
		 * @param propertyName Name of the property.
		 */
		public static AuditProperty<Object> RevisionProperty(String propertyName) {
			return new AuditProperty<Object>(new RevisionPropertyPropertyName(propertyName));
		}

		/**
		 * Create restrictions, projections and specify order for the revision type, corresponding to an
		 * audited entity.
		 */
		public static AuditProperty<RevisionType> RevisionType() {
			return new AuditProperty<RevisionType>(new RevisionTypePropertyName());
		}

		/**
		 * Create restrictions on an id of a related entity.
		 * @param propertyName Name of the property, which is the relation.
		 */
		public static AuditRelatedId RelatedId(String propertyName) {
			return new AuditRelatedId(new EntityPropertyName(propertyName));
		}

		/**
		 * Return the conjuction of two criterions.
		 */
		public static IAuditCriterion And(IAuditCriterion lhs, IAuditCriterion rhs) {
			return new LogicalAuditExpression(lhs, rhs, "and");
		}

		/**
		 * Return the disjuction of two criterions.
		 */
		public static IAuditCriterion Or(IAuditCriterion lhs, IAuditCriterion rhs) {
			return new LogicalAuditExpression(lhs, rhs, "or");
		}

		/**
		 * Return the negation of a criterion.
		 */
		public static IAuditCriterion Not(IAuditCriterion expression) {
			return new NotAuditExpression(expression);
		}

		/**
		 * Group criterions together in a single conjunction (A and B and C...).
		 */
		public static AuditConjunction Conjunction() {
			return new AuditConjunction();
		}

		/**
		 * Group criterions together in a single disjunction (A or B or C...).
		 */
		public static AuditDisjunction Disjunction() {
			return new AuditDisjunction();
		}
	}
}
