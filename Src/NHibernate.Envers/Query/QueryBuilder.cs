using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query
{
	/// <summary>
	/// A class for incrementaly building a HQL query.
	/// </summary>
    public class QueryBuilder 
	{
        //TODO in second implementation phase
        private readonly string entityName;
        private readonly string alias;

        /**
         * For use by alias generator (in case an alias is not provided by the user).
         */
        private readonly Incrementor aliasCounter;
        /**
         * For use by parameter generator, in {@link Parameters}. This counter must be
         * the same in all parameters and sub-queries of this query.
         */
        private readonly Incrementor paramCounter;
        /**
         * Main "where" parameters for this query.
         */
        private readonly Parameters rootParameters;
        public Parameters RootParameters {get { return rootParameters;} }

        /**
         * A list of pairs (from entity name, alias name).
         */
        private readonly List<Pair<String, String>> froms;
        /**
         * A list of pairs (property name, order ascending?).
         */
        private readonly List<Pair<String, Boolean>> orders;
        /**
         * A list of complete projection definitions: either a sole property name, or a function(property name).
         */
        private readonly IList<String> projections;

        /**
         *
         * @param entityName Main entity which should be selected.
         * @param alias Alias of the entity
         */
        public QueryBuilder(string entityName, string alias)
            :this(entityName, alias, new Incrementor(), new Incrementor())
        {
            
        }

		private QueryBuilder(string entityName, string alias, Incrementor aliasCounter, Incrementor paramCounter)
		{
            this.entityName = entityName;
            this.alias = alias;
            this.aliasCounter = aliasCounter;
            this.paramCounter = paramCounter;

            rootParameters = new Parameters(alias, "and", paramCounter);

			froms = new List<Pair<string, String>>();
			orders = new List<Pair<string, Boolean>>();
			projections = new List<string>();

            AddFrom(entityName, alias);
        }

        /**
         * Add an entity from which to select.
         * @param entName Name of the entity from which to select.
         * @param als Alias of the entity. Should be different than all other aliases.
         */
		public void AddFrom(string entName, string als)
		{
			froms.Add(Pair<string, string>.Make(entName, als));
        }

		private string GenerateAlias()
		{
            return "_e" + aliasCounter.Get();
        }

        /**
         * @return A sub-query builder for the same entity (with an auto-generated alias). The sub-query can
         * be later used as a value of a parameter.
         */
        public QueryBuilder NewSubQueryBuilder() 
		{
            return NewSubQueryBuilder(entityName, GenerateAlias());
        }

        /**
         * @param entityName Entity name, which will be the main entity for the sub-query.
         * @param alias Alias of the entity, which can later be used in parameters.
         * @return A sub-query builder for the given entity, with the given alias. The sub-query can
         * be later used as a value of a parameter.
         */
		public QueryBuilder NewSubQueryBuilder(string entityName, string alias)
		{
            return new QueryBuilder(entityName, alias, aliasCounter, paramCounter);
        }

		public void AddOrder(string propertyName, bool ascending)
		{
			orders.Add(Pair<string, bool>.Make(propertyName, ascending));
        }

        public void AddProjection(string function, string propertyName, bool distinct)
        {
            AddProjection(function, propertyName, distinct, true);
        }

        public void AddProjection(string function, string propertyName, bool distinct, bool addAlias)
        {
            if (function == null) 
            {
                projections.Add((distinct ? "distinct " : string.Empty) + (addAlias ? alias+ "." : string.Empty) + propertyName);
            } 
            else 
            {
                projections.Add(function + "(" + (distinct ? "distinct " : string.Empty) + (addAlias ? alias + "." : string.Empty) + propertyName + ")");
            }
        }

        /**
         * Builds the given query, appending results to the given string buffer, and adding all query parameter values
         * that are used to the map provided.
         * @param sb String builder to which the query will be appended.
         * @param queryParamValues Map to which name and values of parameters used in the query should be added.
         */
		public void Build(StringBuilder sb, IDictionary<string, object> queryParamValues)
        {
            sb.Append("select ");
            if (projections.Count > 0)
            {
                // all projections separated with commas
							sb.Append(string.Join(", ",projections.ToArray()));
            }
            else
            {
                // all aliases separated with commas
							sb.Append(string.Join(", ",GetAliasList().ToArray()));
            }
            sb.Append(" from ");
            // all from entities with aliases, separated with commas
						sb.Append(string.Join(", ", GetFromList().ToArray()));
            // where part - rootParameters
            if (!rootParameters.IsEmpty())
            {
                sb.Append(" where ");
                rootParameters.Build(sb, queryParamValues);
            }
            // orders
            if (orders.Count > 0)
            {
                sb.Append(" order by ");
								sb.Append(string.Join(", ", GetOrderList().ToArray()));
            }
        }

		private IList<string> GetAliasList()
		{
			IList<string> aliasList = new List<string>();
			foreach (var from in froms)
			{
                aliasList.Add(from.Second);
            }

            return aliasList;
        }

		private IList<string> GetFromList() 
		{
			var fromList = new List<string>();
            foreach (var from in froms) 
			{
                fromList.Add(from.First + " " + from.Second);
            }

            return fromList;
        }

		private IList<string> GetOrderList()
		{
			var orderList = new List<string>();
            foreach (var order in orders) 
			{
                orderList.Add(alias + "." + order.First + " " + (order.Second ? "asc" : "desc"));
            }

            return orderList;
        }
    }
}
