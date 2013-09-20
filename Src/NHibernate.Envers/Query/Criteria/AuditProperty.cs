using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query.Order;
using NHibernate.Envers.Query.Projection;
using NHibernate.Envers.Query.Property;

namespace NHibernate.Envers.Query.Criteria
{
	/// <summary>
	/// Create restrictions, projections and specify order for a property of an audited entity.
	/// </summary>
	public class AuditProperty : IAuditProjection
	{
		private readonly IPropertyNameGetter _propertyNameGetter;

		public AuditProperty(IPropertyNameGetter propertyNameGetter)
		{
			_propertyNameGetter = propertyNameGetter;
		}

		public virtual IAuditCriterion Eq(object value)
		{
			return new SimpleAuditExpression(_propertyNameGetter, value, "=");
		}

		public virtual IAuditCriterion Ne(object value)
		{
			return new SimpleAuditExpression(_propertyNameGetter, value, "<>");
		}

		public IAuditCriterion Like(object value)
		{
			return new SimpleAuditExpression(_propertyNameGetter, value, " like ");
		}

		public IAuditCriterion Like(string value, MatchMode matchMode)
		{
			return new SimpleAuditExpression(_propertyNameGetter, matchMode.ToMatchString(value), " like ");
		}

		public IAuditCriterion Gt(object value)
		{
			return new SimpleAuditExpression(_propertyNameGetter, value, ">");
		}

		public IAuditCriterion Lt(object value)
		{
			return new SimpleAuditExpression(_propertyNameGetter, value, "<");
		}

		public IAuditCriterion Le(object value)
		{
			return new SimpleAuditExpression(_propertyNameGetter, value, "<=");
		}

		public IAuditCriterion Ge(object value)
		{
			return new SimpleAuditExpression(_propertyNameGetter, value, ">=");
		}

		public IAuditCriterion Between(object lo, object hi)
		{
			return new BetweenAuditExpression(_propertyNameGetter, lo, hi);
		}

		public IAuditCriterion In(object[] values)
		{
			return new InAuditExpression(_propertyNameGetter, values);
		}

		public IAuditCriterion In<T>(IEnumerable<T> values)
		{
			var array = new object[values.Count()];
			var i = 0;
			foreach (var item in values)
			{
				array[i] = item;
				i++;
			}
			return new InAuditExpression(_propertyNameGetter, array);
		}

		public IAuditCriterion IsNull()
		{
			return new NullAuditExpression(_propertyNameGetter);
		}

		public IAuditCriterion EqProperty(string otherPropertyName)
		{
			return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, "=");
		}

		public IAuditCriterion NeProperty(string otherPropertyName)
		{
			return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, "<>");
		}

		public IAuditCriterion LtProperty(string otherPropertyName)
		{
			return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, "<");
		}

		public IAuditCriterion LeProperty(string otherPropertyName)
		{
			return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, "<=");
		}

		public IAuditCriterion GtProperty(string otherPropertyName)
		{
			return new PropertyAuditExpression(_propertyNameGetter, otherPropertyName, ">");
		}

		public IAuditCriterion GeProperty(string otherPropertyName)
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
					  AggregatedAuditExpression.AggregatedMode.Max);
		}

		/// <summary>
		/// Apply a "minimize" constraint, with the ability to specify further constraints on the minimized property
		/// </summary>
		/// <returns></returns>
		public AggregatedAuditExpression Minimize()
		{
			return new AggregatedAuditExpression(_propertyNameGetter,
					  AggregatedAuditExpression.AggregatedMode.Min);
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


		public Tuple<string, string, bool> GetData(AuditConfiguration auditCfg)
		{
			return new Tuple<string, string, bool>(null, _propertyNameGetter.Get(auditCfg), false);
		}

		public IAuditOrder Asc()
		{
			return new PropertyAuditOrder(_propertyNameGetter, true);
		}

		public IAuditOrder Desc()
		{
			return new PropertyAuditOrder(_propertyNameGetter, false);
		}

		public virtual IAuditCriterion HasChanged()
		{
			return new SimpleAuditExpression(new ModifiedFlagPropertyName(_propertyNameGetter), true, "=");
		}

		public virtual IAuditCriterion HasNotChanged()
		{
			return new SimpleAuditExpression(new ModifiedFlagPropertyName(_propertyNameGetter), false, "=");
		}

		public IAuditCriterion InsensitiveLike(object value)
		{
			return new IlikeAuditExpression(_propertyNameGetter, value.ToString());
		}

		public IAuditCriterion InsensitiveLike(string value, MatchMode matchMode)
		{
			return new IlikeAuditExpression(_propertyNameGetter, matchMode.ToMatchString(value));
		}
	}
}
