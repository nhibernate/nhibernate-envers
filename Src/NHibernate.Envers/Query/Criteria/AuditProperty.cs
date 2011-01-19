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
	/// <summary>
	/// Create restrictions, projections and specify order for a property of an audited entity.
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class AuditProperty<T> : IAuditProjection 
    {
        private readonly IPropertyNameGetter _propertyNameGetter;

        public AuditProperty(IPropertyNameGetter propertyNameGetter) 
        {
            _propertyNameGetter = propertyNameGetter;
        }

        public IAuditCriterion Eq(T value) 
        {
		    return new SimpleAuditExpression(_propertyNameGetter, value, "=");
	    }

	    public IAuditCriterion Ne(T value) 
        {
		    return new SimpleAuditExpression(_propertyNameGetter, value, "<>");
	    }

	    public IAuditCriterion Like(T value) 
        {
		    return new SimpleAuditExpression(_propertyNameGetter, value, " like ");
	    }

	    public IAuditCriterion Like(String value, MatchMode matchMode) 
        {
		    return new SimpleAuditExpression(_propertyNameGetter, matchMode.ToMatchString(value), " like " );
	    }

	    public IAuditCriterion Gt(T value) 
        {
		    return new SimpleAuditExpression(_propertyNameGetter, value, ">");
	    }

	    public IAuditCriterion Lt(T value) 
        {
		    return new SimpleAuditExpression(_propertyNameGetter, value, "<");
	    }

	    public IAuditCriterion Le(T value) 
        {
		    return new SimpleAuditExpression(_propertyNameGetter, value, "<=");
	    }

	    public IAuditCriterion Ge(T value) 
        {
		    return new SimpleAuditExpression(_propertyNameGetter, value, ">=");
	    }

	    public IAuditCriterion Between(T lo, T hi) 
        {
		    return new BetweenAuditExpression(_propertyNameGetter, lo, hi);
	    }

	    public IAuditCriterion In(IEnumerable<T> values) 
        {
			//rk - makes this nicer when upgrading NH, http://216.121.112.228/browse/NH-2461
			return new InAuditExpression(_propertyNameGetter, values.Cast<object>().ToArray());
	    }

	    public IAuditCriterion IsNull() 
		{
		    return new NullAuditExpression(_propertyNameGetter);
	    }

	    public IAuditCriterion EqProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, "=");
	    }

	    public IAuditCriterion NeProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, "<>");
	    }

	    public IAuditCriterion LtProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, "<");
	    }

	    public IAuditCriterion LeProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, "<=");
	    }

	    public IAuditCriterion GtProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, ">");
	    }

	    public IAuditCriterion GeProperty(String otherPropertyName) 
		{
		    return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, ">=");
	    }

	    public IAuditCriterion IsNotNull() 
		{
		    return new NotNullAuditExpression(_propertyNameGetter);
	    }

		/// <summary>
		/// Apply a "maximalize" constraint, with the ability to specify further constraints on the maximized property
		/// </summary>
		/// <returns></returns>
		public AggregatedAuditExpression Maximize() 
		{
            return new AggregatedAuditExpression(_propertyNameGetter,
                    AggregatedAuditExpression.AggregatedMode.MAX);
        }

		/// <summary>
		/// Apply a "minimize" constraint, with the ability to specify further constraints on the minimized property
		/// </summary>
		/// <returns></returns>
        public AggregatedAuditExpression Minimize() 
		{
            return new AggregatedAuditExpression(_propertyNameGetter,
                    AggregatedAuditExpression.AggregatedMode.MIN);
        }

		/// <summary>
		/// Projection on the maximum value
		/// </summary>
		/// <returns></returns>
        public IAuditProjection Max() 
		{
            return new PropertyAuditProjection(_propertyNameGetter, "max", false);
        }

		/// <summary>
		/// Projection on the minimum value
		/// </summary>
		/// <returns></returns>
        public IAuditProjection Min() 
		{
            return new PropertyAuditProjection(_propertyNameGetter, "min", false);
        }

        public IAuditProjection Count() 
		{
            return new PropertyAuditProjection(_propertyNameGetter, "count", false);
        }


        public IAuditProjection CountDistinct() 
		{
            return new PropertyAuditProjection(_propertyNameGetter, "count", true);
        }

        public IAuditProjection Distinct() 
		{
            return new PropertyAuditProjection(_propertyNameGetter, null, true);
        }

		/// <summary>
		/// Projection using a custom function
		/// </summary>
		/// <param name="functionName"></param>
		/// <returns></returns>
        public IAuditProjection Function(string functionName) 
		{
            return new PropertyAuditProjection(_propertyNameGetter, functionName, false);
        }


		public Triple<string, string, bool> GetData(AuditConfiguration auditCfg) 
		{
			return Triple<string, string, bool>.Make(null, _propertyNameGetter.Get(auditCfg), false);
        }

        public IAuditOrder Asc() 
		{
            return new PropertyAuditOrder(_propertyNameGetter, true);
        }

        public IAuditOrder Desc() 
		{
            return new PropertyAuditOrder(_propertyNameGetter, false);
        }
    }
}
