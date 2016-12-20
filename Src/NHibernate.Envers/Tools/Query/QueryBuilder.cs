using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tools.Query
{
	/// <summary>
	/// A class for incrementaly building a HQL query.
	/// </summary>
	public class QueryBuilder : ICloneable
	{
		private readonly string _entityName;

		// For use by alias generator (in case an alias is not provided by the user).
		private readonly Incrementor _aliasCounter;

		// For use by parameter generator, in {@link Parameters}. This counter must be
		// the same in all parameters and sub-queries of this query.
		private readonly Incrementor _paramCounter;

		// A list of pairs (from entity name, alias name, whether to select the entity).
		private readonly ICollection<Tuple<string, string, bool>> _froms;

		// A list of triples (property alias, name, order ascending?).
		private readonly ICollection<Tuple<string, string, bool>> _orders;

		// A list of complete projection definitions: either a sole property name, or a function(property name).
		private readonly ICollection<string> _projections;

		// "Where" parameters for this query.Each parameter element of the list for one alias from the "from" part.
		private readonly IList<Parameters> _parameters = new List<Parameters>();


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

			_parameters.Add(new Parameters(alias, "and", paramCounter));

			_froms = new List<Tuple<string, string, bool>>();
			_orders = new List<Tuple<string, string, bool>>();
			_projections = new List<string>();

			AddFrom(entityName, alias, true);
		}

		//only for clone purpose
		private QueryBuilder(QueryBuilder other)
		{
			_entityName = other._entityName;
			RootAlias = other.RootAlias;
			_aliasCounter = (Incrementor) other._aliasCounter.Clone();
			_paramCounter = (Incrementor) other._paramCounter.Clone();
			foreach (var parameter in other._parameters)
			{
				_parameters.Add((Parameters) parameter.Clone());
			}
			_froms = new List<Tuple<string, string, bool>>(other._froms);
			_orders = new List<Tuple<string, string, bool>>(other._orders);
			_projections = new List<string>(other._projections);
		}


		public Parameters RootParameters
		{
			get { return _parameters[0]; }
		}

		public Parameters AddParameters(string alias)
		{
			var result = new Parameters(alias, Parameters.AND, _paramCounter);
			_parameters.Add(result);
			return result;
		}

		/// <summary>
		/// Add an entity from which to select.
		/// </summary>
		/// <param name="entName">Name of the entity from which to select.</param>
		/// <param name="als">Alias of the entity. Should be different than all other aliases.</param>
		/// <param name="select">Whether the entity should be selected.</param>
		public void AddFrom(string entName, string als, bool select)
		{
			_froms.Add(new Tuple<string, string, bool>(entName, als, select));
		}

		public string GenerateAlias()
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

		public void AddOrder(string alias, string propertyName, bool ascending)
		{
			_orders.Add(new Tuple<string, string, bool>(alias, propertyName, ascending));
		}

		public void AddProjection(string function, string alias, string propertyName, bool distinct)
		{
			var effectivePropertyName = propertyName == null ? string.Empty : "." + propertyName;
			if (function == null)
			{
				_projections.Add((distinct ? "distinct " : string.Empty) + alias + effectivePropertyName);
			}
			else
			{
				_projections.Add(function + "(" + (distinct ? "distinct " : string.Empty) + alias + effectivePropertyName + ")");
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
							  : string.Join(", ", selectAliasList().ToArray()));
			sb.Append(" from ");
			// all from entities with aliases, separated with commas
			sb.Append(string.Join(", ", fromList().ToArray()));
			// where part - rootParameters
			var first = true;
			foreach (var prms in _parameters.Where(prms => !prms.IsEmpty()))
			{
				if (first)
				{
					sb.Append(" where ");
					first = false;
				}
				else
				{
					sb.Append(" and ");
				}
				prms.Build(sb, queryParamValues);
			}
			// orders
			if (_orders.Count > 0)
			{
				sb.Append(" order by ");
				sb.Append(string.Join(", ", orderList().ToArray()));
			}
		}

		private IEnumerable<string> selectAliasList()
		{
			return (from @from in _froms where @from.Item3 select @from.Item2);
		}

		public string RootAlias { get; }

		private IEnumerable<string> fromList()
		{
			return _froms.Select(from => from.Item1 + " " + from.Item2);
		}

		private IEnumerable<string> orderList()
		{
			return _orders.Select(order => order.Item1 + "." + order.Item2 + " " + (order.Item3 ? "asc" : "desc"));
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

		public object Clone()
		{
			return new QueryBuilder(this);
		}
	}
}
