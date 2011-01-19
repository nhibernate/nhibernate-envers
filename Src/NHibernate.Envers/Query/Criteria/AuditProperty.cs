using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AuditProperty<T> : IAuditProjection 
    {
        private readonly IPropertyNameGetter propertyNameGetter;

        public AuditProperty(IPropertyNameGetter propertyNameGetter) 
        {
            this.propertyNameGetter = propertyNameGetter;
        }

        public IAuditCriterion Eq(T value) 
        {
		    return new SimpleAuditExpression(propertyNameGetter, value, "=");
	    }

	    public IAuditCriterion Ne(T value) 
        {
		    return new SimpleAuditExpression(propertyNameGetter, value, "<>");
	    }

	    public IAuditCriterion Like(T value) 
        {
		    return new SimpleAuditExpression(propertyNameGetter, value, " like ");
	    }

	    public IAuditCriterion Like(String value, MatchMode matchMode) 
        {
		    return new SimpleAuditExpression(propertyNameGetter, matchMode.ToMatchString(value), " like " );
	    }

	    public IAuditCriterion Gt(T value) 
        {
		    return new SimpleAuditExpression(propertyNameGetter, value, ">");
	    }

	    public IAuditCriterion Lt(T value) 
        {
		    return new SimpleAuditExpression(propertyNameGetter, value, "<");
	    }

	    public IAuditCriterion Le(T value) 
        {
		    return new SimpleAuditExpression(propertyNameGetter, value, "<=");
	    }

	    public IAuditCriterion Ge(T value) 
        {
		    return new SimpleAuditExpression(propertyNameGetter, value, ">=");
	    }

	    public IAuditCriterion Between(T lo, T hi) 
        {
		    return new BetweenAuditExpression(propertyNameGetter, lo, hi);
	    }

	    public IAuditCriterion In(IEnumerable<T> values) 
        {
			//rk - makes this nicer when upgrading NH, http://216.121.112.228/browse/NH-2461
			return new InAuditExpression(propertyNameGetter, values.Cast<object>().ToArray());
	    }

	    public IAuditCriterion IsNull() 
		{
		    return new NullAuditExpression(propertyNameGetter);
	    }

	    public IAuditCriterion EqProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, "=");
	    }

	    public IAuditCriterion NeProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, "<>");
	    }

	    public IAuditCriterion LtProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, "<");
	    }

	    public IAuditCriterion LeProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, "<=");
	    }

	    public IAuditCriterion GtProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, ">");
	    }

	    public IAuditCriterion GeProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(propertyNameGetter, otherPropertyName, ">=");
	    }

	    public IAuditCriterion IsNotNull() 
		{
		    return new NotNullAuditExpression(propertyNameGetter);
	    }

		/// <summary>
		/// Apply a "maximalize" constraint, with the ability to specify further constraints on the maximized property
		/// </summary>
		/// <returns></returns>
		public AggregatedAuditExpression Maximize() 
		{
            return new AggregatedAuditExpression(propertyNameGetter,
                    AggregatedAuditExpression.AggregatedMode.MAX);
        }

		/// <summary>
		/// Apply a "minimize" constraint, with the ability to specify further constraints on the minimized property
		/// </summary>
		/// <returns></returns>
        public AggregatedAuditExpression Minimize() 
		{
            return new AggregatedAuditExpression(propertyNameGetter,
                    AggregatedAuditExpression.AggregatedMode.MIN);
        }

		/// <summary>
		/// Projection on the maximum value
		/// </summary>
		/// <returns></returns>
        public IAuditProjection Max() 
		{
            return new PropertyAuditProjection(propertyNameGetter, "max", false);
        }

		/// <summary>
		/// Projection on the minimum value
		/// </summary>
		/// <returns></returns>
        public IAuditProjection Min() 
		{
            return new PropertyAuditProjection(propertyNameGetter, "min", false);
        }

        public IAuditProjection Count() 
		{
            return new PropertyAuditProjection(propertyNameGetter, "count", false);
        }


        public IAuditProjection CountDistinct() 
		{
            return new PropertyAuditProjection(propertyNameGetter, "count", true);
        }

        public IAuditProjection Distinct() 
		{
            return new PropertyAuditProjection(propertyNameGetter, null, true);
        }

		/// <summary>
		/// Projection using a custom function
		/// </summary>
		/// <param name="functionName"></param>
		/// <returns></returns>
        public IAuditProjection Function(string functionName) 
		{
            return new PropertyAuditProjection(propertyNameGetter, functionName, false);
        }


		public Triple<string, string, bool> GetData(AuditConfiguration auditCfg) 
		{
			return Triple<string, string, bool>.Make(null, propertyNameGetter.Get(auditCfg), false);
        }

        public IAuditOrder Asc() 
		{
            return new PropertyAuditOrder(propertyNameGetter, true);
        }

        public IAuditOrder Desc() 
		{
            return new PropertyAuditOrder(propertyNameGetter, false);
        }
    }
}
