using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Order;
using NHibernate.Envers.Query.Projection;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Criteria
{
    /**
     * Create restrictions, projections and specify order for a property of an audited entity.
     * @author Adam Warski (adam at warski dot org)
     */
    public class AuditProperty<T> : IAuditProjection {
        private readonly IPropertyNameGetter propertyNameGetter;

        public AuditProperty(IPropertyNameGetter propertyNameGetter) {
            this.propertyNameGetter = propertyNameGetter;
        }

        /**
	     * Apply an "equal" constraint
	     */
	    public IAuditCriterion Eq(T value) {
		    return new SimpleAuditExpression(propertyNameGetter, value, "=");
	    }

        /**
	     * Apply a "not equal" constraint
	     */
	    public IAuditCriterion ne(T value) {
		    return new SimpleAuditExpression(propertyNameGetter, value, "<>");
	    }

        /**
	     * Apply a "like" constraint
	     */
	    public IAuditCriterion like(T value) {
		    return new SimpleAuditExpression(propertyNameGetter, value, " like ");
	    }

        /**
	     * Apply a "like" constraint
	     */
	    public IAuditCriterion like(String value, MatchMode matchMode) {
		    return new SimpleAuditExpression(propertyNameGetter, matchMode.ToMatchString(value), " like " );
	    }

        /**
	     * Apply a "greater than" constraint
	     */
	    public IAuditCriterion gt(T value) {
		    return new SimpleAuditExpression(propertyNameGetter, value, ">");
	    }

        /**
	     * Apply a "less than" constraint
	     */
	    public IAuditCriterion lt(T value) {
		    return new SimpleAuditExpression(propertyNameGetter, value, "<");
	    }

        /**
	     * Apply a "less than or equal" constraint
	     */
	    public IAuditCriterion le(T value) {
		    return new SimpleAuditExpression(propertyNameGetter, value, "<=");
	    }

        /**
	     * Apply a "greater than or equal" constraint
	     */
	    public IAuditCriterion ge(T value) {
		    return new SimpleAuditExpression(propertyNameGetter, value, ">=");
	    }

        /**
	     * Apply a "between" constraint
	     */
	    public IAuditCriterion between(T lo, T hi) {
		    return new BetweenAuditExpression(propertyNameGetter, lo, hi);
	    }

        /**
	     * Apply an "in" constraint
	     */
	    public IAuditCriterion In(T[] values) {
		    return new InAuditExpression(propertyNameGetter, values.Cast<object>().ToArray());
	    }

        /**
	     * Apply an "in" constraint
	     */
	    public IAuditCriterion In(ICollection values) {
            object[] valuesArray = new object[values.Count];
            values.CopyTo(valuesArray, 0);
		    return new InAuditExpression(propertyNameGetter, valuesArray);
	    }

        /**
	     * Apply an "is null" constraint
	     */
	    public IAuditCriterion isNull() {
		    return new NullAuditExpression(propertyNameGetter);
	    }

        /**
	     * Apply an "equal" constraint to another property
	     */
	    public IAuditCriterion eqProperty(String otherPropertyName) {
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, "=");
	    }

        /**
	     * Apply a "not equal" constraint to another property
	     */
	    public IAuditCriterion neProperty(String otherPropertyName) {
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, "<>");
	    }

        /**
	     * Apply a "less than" constraint to another property
	     */
	    public IAuditCriterion ltProperty(String otherPropertyName) {
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, "<");
	    }

        /**
	     * Apply a "less than or equal" constraint to another property
	     */
	    public IAuditCriterion leProperty(String otherPropertyName) {
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, "<=");
	    }

        /**
	     * Apply a "greater than" constraint to another property
	     */
	    public IAuditCriterion gtProperty(String otherPropertyName) {
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, ">");
	    }

        /**
	     * Apply a "greater than or equal" constraint to another property
	     */
	    public IAuditCriterion geProperty(String otherPropertyName) {
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, ">=");
	    }

        /**
	     * Apply an "is not null" constraint to the another property
	     */
	    public IAuditCriterion isNotNull() {
		    return new NotNullAuditExpression(propertyNameGetter);
	    }

        /**
         * Apply a "maximalize" constraint, with the ability to specify further constraints on the maximized
         * property
         */
        public AggregatedAuditExpression maximize() {
            return new AggregatedAuditExpression(propertyNameGetter,
                    AggregatedAuditExpression.AggregatedMode.MAX);
        }

        /**
         * Apply a "minimize" constraint, with the ability to specify further constraints on the minimized
         * property
         */
        public AggregatedAuditExpression minimize() {
            return new AggregatedAuditExpression(propertyNameGetter,
                    AggregatedAuditExpression.AggregatedMode.MIN);
        }

        // Projections

        /**
         * Projection on the maximum value
         */
        public IAuditProjection max() {
            return new PropertyAuditProjection(propertyNameGetter, "max", false);
        }

        /**
         * Projection on the minimum value
         */
        public IAuditProjection min() {
            return new PropertyAuditProjection(propertyNameGetter, "min", false);
        }

        /**
         * Projection counting the values
         */
        public IAuditProjection count() {
            return new PropertyAuditProjection(propertyNameGetter, "count", false);
        }

        /**
         * Projection counting distinct values
         */
        public IAuditProjection countDistinct() {
            return new PropertyAuditProjection(propertyNameGetter, "count", true);
        }

        /**
         * Projection on distinct values
         */
        public IAuditProjection distinct() {
            return new PropertyAuditProjection(propertyNameGetter, null, true);
        }

        /**
         * Projection using a custom function
         */
        public IAuditProjection function(String functionName) {
            return new PropertyAuditProjection(propertyNameGetter, functionName, false);
        }

        // Projection on this property

        public Triple<String, String, Boolean> GetData(AuditConfiguration auditCfg) {
            return Triple<String, String, Boolean>.Make(null, propertyNameGetter.Get(auditCfg), false);
        }

        // Order

        /**
         * Sort the results by the property in ascending order
         */
        public IAuditOrder asc() {
            return new PropertyAuditOrder(propertyNameGetter, true);
        }

        /**
         * Sort the results by the property in descending order
         */
        public IAuditOrder desc() {
            return new PropertyAuditOrder(propertyNameGetter, false);
        }
    }
}
