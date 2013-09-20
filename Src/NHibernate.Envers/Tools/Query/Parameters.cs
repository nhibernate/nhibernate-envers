using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tools.Query
{
	/// <summary>
	/// Generates metadata for to-one relations (reference-valued properties).
	/// </summary>
	public class Parameters : ICloneable
	{
		public const string AND = "and";
		public const string OR = "or";
	
		/// <summary>
		/// Main alias of the entity.
		/// </summary>
		private readonly string alias;

		/// <summary>
		/// Connective between these parameters - "and" or "or".
		/// </summary>
		private readonly string connective;

		/// <summary>
		/// For use by the parameter generator. Must be the same in all "child" (and parent) parameters.
		/// </summary>
		private readonly Incrementor queryParamCounter;

		/// <summary>
		/// A list of sub-parameters (parameters with a different connective).
		/// </summary>
		private readonly IList<Parameters> subParameters;

		/// <summary>
		/// A list of negated parameters.
		/// </summary>
		private readonly IList<Parameters> negatedParameters;

		/// <summary>
		/// A list of complete where-expressions.
		/// </summary>
		private readonly IList<string> expressions;

		/// <summary>
		/// Values of parameters used in expressions.
		/// </summary>
		private readonly IDictionary<string, object> localQueryParamValues;

		public Parameters(string alias, string connective, Incrementor queryParamCounter) 
		{
			this.alias = alias;
			this.connective = connective;
			this.queryParamCounter = queryParamCounter;

			subParameters = new List<Parameters>();   
			negatedParameters = new List<Parameters>();
			expressions = new List<string>();
			localQueryParamValues = new Dictionary<string, object>(); 
		}

		//only for clone purpose
		private Parameters(Parameters other)
		{
			alias = other.alias;
			connective = other.connective;
			queryParamCounter = (Incrementor) other.queryParamCounter.Clone();
			subParameters = new List<Parameters>(other.subParameters.Count);
			foreach (var p in other.subParameters)
			{
				subParameters.Add((Parameters) p.Clone());
			}
			negatedParameters = new List<Parameters>(other.negatedParameters.Count);
			foreach (var p in other.negatedParameters)
			{
				negatedParameters.Add((Parameters) p.Clone());
			}
			expressions = new List<string>(other.expressions);
			localQueryParamValues = new Dictionary<string, object>(other.localQueryParamValues);
		}

		private string generateQueryParam() 
		{
			return "_p" + queryParamCounter.Get();
		}

		/// <summary>
		///  Adds sub-parameters with a new connective. That is, the parameters will be grouped in parentheses in the
		///  generated query, e.g.: ... and (exp1 or exp2) and ..., assuming the old connective is "and", and the
		///  new connective is "or".
		/// </summary>
		/// <param name="newConnective">New connective of the parameters.</param>
		/// <returns>Sub-parameters with the given connective.</returns>
		public Parameters AddSubParameters(string newConnective) 
		{
			if (connective.Equals(newConnective)) 
			{
				return this;
			}
			var newParams = new Parameters(alias, newConnective, queryParamCounter);
			subParameters.Add(newParams);
			return newParams;
		}

		/// <summary>
		///  Adds negated parameters, by default with the "and" connective. These paremeters will be grouped in parentheses
		///  in the generated query and negated, e.g. ... not (exp1 and exp2) ...
		/// </summary>
		/// <returns>Negated sub paremters.</returns>
		public Parameters AddNegatedParameters() 
		{
			var newParams = new Parameters(alias, AND, queryParamCounter);
			negatedParameters.Add(newParams);
			return newParams;
		}

		/// <summary>
		/// Adds <code>IS NULL</code> restriction.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <param name="addAlias">Positive if alias to property name shall be added.</param>
		public void AddNullRestriction(string propertyName, bool addAlias)
		{
			AddWhere(propertyName, addAlias, "is", "null", false);
		}

		/// <summary>
		/// Adds <code>IS NOT NULL</code> restriction.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <param name="addAlias">Positive if alias to property name shall be added.</param>
		public void AddNotNullRestriction(string propertyName, bool addAlias)
		{
			AddWhere(propertyName, addAlias, "is not", "null", false);
		}

		public void AddWhere(string left, string op, string right) 
		{
			AddWhere(left, true, op, right, true);
		}

		public void AddWhere(string left, bool addAliasLeft, string op, string right, bool addAliasRight) 
		{
			var expression = new StringBuilder();

			if (addAliasLeft) { expression.Append(alias).Append("."); }
			expression.Append(left);

			expression.Append(" ").Append(op).Append(" ");

			if (addAliasRight) { expression.Append(alias).Append("."); }
			expression.Append(right);

			expressions.Add(expression.ToString());
		}

		public void AddWhereWithParam(string left, string op, object paramValue) 
		{
			AddWhereWithParam(left, true, op, paramValue);
		}

		public void AddWhereWithParam(string left, bool addAlias, string op, object paramValue) 
		{
			var paramName = generateQueryParam();
			localQueryParamValues.Add(paramName, paramValue);

			AddWhereWithNamedParam(left, addAlias, op, paramName);
		}

		public void AddWhereWithNamedParam(string left, string op, string paramName) 
		{
			AddWhereWithNamedParam(left, true, op, paramName);
		}

		public void AddWhereWithNamedParam(string left, bool addAlias, string op, string paramName) 
		{
			var expression = new StringBuilder(1024);

			if (addAlias) { expression.Append(alias).Append("."); }
			expression.Append(left);
			expression.Append(" ").Append(op).Append(" ");
			expression.Append(":").Append(paramName);

			expressions.Add(expression.ToString());
		}

		public void AddWhereWithParams(string left, string opStart, object[] paramValues, string opEnd)
		{
			var expression = new StringBuilder(1024);
			var paramValuesLength = paramValues.Length;
			expression.Append(alias).Append(".").Append(left).Append(" ").Append(opStart);
			for (var i = 0; i < paramValuesLength; i++)
			{
				var paramValue = paramValues[i];
				var paramName = generateQueryParam();
				localQueryParamValues.Add(paramName, paramValue);
				expression.Append(":").Append(paramName);
				if (i != paramValues.Length - 1)
				{
					expression.Append(", ");
				}
			}
			expression.Append(opEnd);	
			expressions.Add(expression.ToString());
		}

		public void AddWhere(string left, string op, QueryBuilder right) 
		{
			AddWhere(left, true, op, right);
		}

		public void AddWhere(string left, bool addAlias, string op, QueryBuilder right) 
		{
			var expression = new StringBuilder(1024);

			if (addAlias) {
				expression.Append(alias).Append(".");
			}

			expression.Append(left);

			expression.Append(" ").Append(op).Append(" ");

			expression.Append("(");
			right.Build(expression, localQueryParamValues);
			expression.Append(")");        

			expressions.Add(expression.ToString());
		}

		public bool IsEmpty() 
		{
			return expressions.Count == 0 && subParameters.Count == 0 && negatedParameters.Count == 0;
		}

		public void Build(StringBuilder sb, IDictionary<string, object> queryParamValues)
		{
			var pp = new ClauseAppender(sb, connective);
			foreach (var expression in expressions)
			{
				pp.Append(expression);
			}
			foreach (var sub in subParameters.Where(sub => subParameters.Count > 0))
			{
				pp.Append("(");
				sub.Build(sb, queryParamValues);
				sb.Append(")");
			}

			foreach (var negated in negatedParameters.Where(negated => negatedParameters.Count > 0))
			{
				pp.Append("not (");
				negated.Build(sb, queryParamValues);
				sb.Append(")");
			}

			foreach (var pair in localQueryParamValues)
			{
				queryParamValues.Add(pair);
			}
		}

		private class ClauseAppender
		{
			private readonly StringBuilder stringBuilder;
			private readonly string connective;
			private bool isFirstAppend = true;
			public ClauseAppender(StringBuilder stringBuilder, string connective)
			{
				this.stringBuilder = stringBuilder;
				this.connective = connective;
			}

			public void Append(string toAppend)
			{
				if (!isFirstAppend)
				{
					stringBuilder.Append(" ").Append(connective).Append(" ");
				}

				stringBuilder.Append(toAppend);
				isFirstAppend = false;
			}
		}

		public object Clone()
		{
			return new Parameters(this);
		}

		public void AddWhereWithFunction(string left, string leftFunction, string op, object paramValue)
		{
			var paramName = generateQueryParam();
			localQueryParamValues[paramName] = paramValue;
			var expression = new StringBuilder();
			expression.Append(leftFunction).Append("(")
			          .Append(alias).Append(".")
			          .Append(left).Append(")")
			          .Append(" ").Append(op).Append(" ")
			          .Append(":").Append(paramName);
			expressions.Add(expression.ToString());
		}
	}
}