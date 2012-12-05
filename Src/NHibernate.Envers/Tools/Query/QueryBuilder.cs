﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tools.Query
{
	/// <summary>
	/// A class for incrementaly building a HQL query.
	/// </summary>
	public class QueryBuilder
	{
		private readonly string _entityName;

		// For use by alias generator (in case an alias is not provided by the user).
		private readonly Incrementor _aliasCounter;

		// For use by parameter generator, in {@link Parameters}. This counter must be
		// the same in all parameters and sub-queries of this query.
		private readonly Incrementor _paramCounter;

		// A list of pairs (from entity name, alias name).
		private readonly ICollection<Pair<string, string>> _froms;

		// A list of pairs (property name, order ascending?).
		private readonly ICollection<Pair<string, bool>> _orders;

		// A list of complete projection definitions: either a sole property name, or a function(property name).
		private readonly ICollection<string> _projections;

		/// <param name="entityName">Main entity which should be selected.</param>
		/// <param name="alias">Alias of the entity</param>
		public QueryBuilder(string entityName, string alias)
			: this(entityName, alias, new Incrementor(), new Incrementor())
		{
		}

		private QueryBuilder(string entityName, string alias, Incrementor aliasCounter, Incrementor paramCounter)
		{
			_entityName = entityName;
			RootAlias = alias;
			_aliasCounter = aliasCounter;
			_paramCounter = paramCounter;

			RootParameters = new Parameters(alias, "and", paramCounter);

			_froms = new List<Pair<string, string>>();
			_orders = new List<Pair<string, bool>>();
			_projections = new List<string>();

			AddFrom(entityName, alias);
		}

		/// <summary>
		/// Main "where" parameters for this query.
		/// </summary>
		public Parameters RootParameters { get; private set; }

		/// <summary>
		/// Add an entity from which to select.
		/// </summary>
		/// <param name="entName">Name of the entity from which to select.</param>
		/// <param name="als">Alias of the entity. Should be different than all other aliases.</param>
		public void AddFrom(string entName, string als)
		{
			_froms.Add(new Pair<string, string>(entName, als));
		}

		private string GenerateAlias()
		{
			return "_e" + _aliasCounter.Get();
		}
		
		/// <returns>
		/// A sub-query builder for the same entity (with an auto-generated alias). The sub-query can
		/// be later used as a value of a parameter.
		/// </returns>
		public QueryBuilder NewSubQueryBuilder()
		{
			return NewSubQueryBuilder(_entityName, GenerateAlias());
		}

		/// <param name="entityName">Entity name, which will be the main entity for the sub-query.</param>
		/// <param name="alias">Alias of the entity, which can later be used in parameters.</param>
		/// <returns>
		/// A sub-query builder for the given entity, with the given alias. The sub-query can
		/// be later used as a value of a parameter.
		/// </returns>
		public QueryBuilder NewSubQueryBuilder(string entityName, string alias)
		{
			return new QueryBuilder(entityName, alias, _aliasCounter, _paramCounter);
		}

		public void AddOrder(string propertyName, bool ascending)
		{
			_orders.Add(new Pair<string, bool>(propertyName, ascending));
		}

		public void AddProjection(string function, string propertyName, bool distinct)
		{
			AddProjection(function, propertyName, distinct, true);
		}

		public void AddProjection(string function, string propertyName, bool distinct, bool addAlias)
		{
			if (function == null)
			{
				_projections.Add((distinct ? "distinct " : string.Empty) + (addAlias ? RootAlias + "." : string.Empty) + propertyName);
			}
			else
			{
				_projections.Add(function + "(" + (distinct ? "distinct " : string.Empty) + (addAlias ? RootAlias + "." : string.Empty) + propertyName + ")");
			}
		}

		/// <summary>
		/// Builds the given query, appending results to the given string buffer, and adding all query parameter values
		/// that are used to the map provided.
		/// </summary>
		/// <param name="sb">String builder to which the query will be appended.</param>
		/// <param name="queryParamValues">
		/// Map to which name and values of parameters used in the query should be added.
		/// <code>null</code> is allowed if no additional parameters are to be added.
		/// </param>
		public void Build(StringBuilder sb, IDictionary<string, object> queryParamValues)
		{
			sb.Append("select ");
			sb.Append(_projections.Count > 0
							  ? string.Join(", ", _projections.ToArray())
							  : string.Join(", ", GetAliasList().ToArray()));
			sb.Append(" from ");
			// all from entities with aliases, separated with commas
			sb.Append(string.Join(", ", GetFromList().ToArray()));
			// where part - rootParameters
			if (!RootParameters.IsEmpty())
			{
				sb.Append(" where ");
				RootParameters.Build(sb, queryParamValues);
			}
			// orders
			if (_orders.Count > 0)
			{
				sb.Append(" order by ");
				sb.Append(string.Join(", ", GetOrderList().ToArray()));
			}
		}

		private IEnumerable<string> GetAliasList()
		{
			return _froms.Select(theFrom => theFrom.Second).ToList();
		}

		public string RootAlias { get; private set; }

		private IEnumerable<string> GetFromList()
		{
			return _froms.Select(theFrom => theFrom.First + " " + theFrom.Second).ToList();
		}

		private IEnumerable<string> GetOrderList()
		{
			return _orders.Select(theOrder => RootAlias + "." + theOrder.First + " " + (theOrder.Second ? "asc" : "desc")).ToList();
		}

		public IQuery ToQuery(ISession session)
		{
			var querySb = new StringBuilder();
			var queryParamValues = new Dictionary<string, object>();

			Build(querySb, queryParamValues);

			var query = session.CreateQuery(querySb.ToString());
			foreach (var queryParamValue in queryParamValues)
			{
				query.SetParameter(queryParamValue.Key, queryParamValue.Value);
			}
			return query;
		}
	}
}
