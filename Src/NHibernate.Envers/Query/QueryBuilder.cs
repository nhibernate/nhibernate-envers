using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query
{
    /**
     * A class for incrementaly building a HQL query.
     * @author Catalina Panait, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class QueryBuilder {
        //TODO in second implementation phase
        private readonly String entityName;
        private readonly String alias;

        /**
         * For use by alias generator (in case an alias is not provided by the user).
         */
        private MutableInteger aliasCounter;
        /**
         * For use by parameter generator, in {@link Parameters}. This counter must be
         * the same in all parameters and sub-queries of this query.
         */
        private MutableInteger paramCounter;
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
        public QueryBuilder(String entityName, String alias)
            :this(entityName, alias, new MutableInteger(), new MutableInteger())
        {
            
        }

        private QueryBuilder(String entityName, String alias, MutableInteger aliasCounter, MutableInteger paramCounter) {
            this.entityName = entityName;
            this.alias = alias;
            this.aliasCounter = aliasCounter;
            this.paramCounter = paramCounter;

            rootParameters = new Parameters(alias, "and", paramCounter);

            froms = new List<Pair<String, String>>();
            orders = new List<Pair<String, Boolean>>();
            projections = new List<String>();

            AddFrom(entityName, alias);
        }

        /**
         * Add an entity from which to select.
         * @param entName Name of the entity from which to select.
         * @param als Alias of the entity. Should be different than all other aliases.
         */
        public void AddFrom(String entName, String als) {
            froms.Add(Pair<String, String>.Make(entName, als));
        }

        private String GenerateAlias() {
            return "_e" + aliasCounter.getAndIncrease();
        }

        /**
         * @return A sub-query builder for the same entity (with an auto-generated alias). The sub-query can
         * be later used as a value of a parameter.
         */
        public QueryBuilder NewSubQueryBuilder() {
            return NewSubQueryBuilder(entityName, GenerateAlias());
        }

        /**
         * @param entityName Entity name, which will be the main entity for the sub-query.
         * @param alias Alias of the entity, which can later be used in parameters.
         * @return A sub-query builder for the given entity, with the given alias. The sub-query can
         * be later used as a value of a parameter.
         */
        public QueryBuilder NewSubQueryBuilder(String entityName, String alias) {
            return new QueryBuilder(entityName, alias, aliasCounter, paramCounter);
        }

        public void AddOrder(String propertyName, bool ascending) {
            orders.Add(Pair<String,bool>.Make(propertyName, ascending));
        }

        public void AddProjection(String function, String propertyName, bool distinct) {
            AddProjection(function, propertyName, distinct, true);
        }

        public void AddProjection(String function, String propertyName, bool distinct, bool addAlias) {
            if (function == null) {
                projections.Add((distinct ? "distinct " : "") + (addAlias ? alias+ "." : "") + propertyName);
            } else {
                projections.Add(function + "(" + (distinct ? "distinct " : "") + (addAlias ? alias + "." : "") + propertyName + ")");
            }
        }

        /**
         * Builds the given query, appending results to the given string buffer, and adding all query parameter values
         * that are used to the map provided.
         * @param sb String builder to which the query will be appended.
         * @param queryParamValues Map to which name and values of parameters used in the query should be added.
         */
         public void Build(StringBuilder sb, IDictionary<String, Object> queryParamValues)
        {
            sb.Append("select ");
            if (projections.Count > 0)
            {
                // all projections separated with commas
                StringTools.Append(sb, projections.GetEnumerator(), ", ");
            }
            else
            {
                // all aliases separated with commas
                StringTools.Append(sb, GetAliasList().GetEnumerator(), ", ");
            }
            sb.Append(" from ");
            // all from entities with aliases, separated with commas
            StringTools.Append(sb, GetFromList().GetEnumerator(), ", ");
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
                StringTools.Append(sb, GetOrderList().GetEnumerator(), ", ");
            }
        }

        private IList<String> GetAliasList() {
            IList<String> aliasList = new List<String>();
            foreach (Pair<String, String> from in froms) {
                aliasList.Add(from.Second);
            }

            return aliasList;
        }

        private IList<String> GetFromList() {
            IList<String> fromList = new List<String>();
            foreach (Pair<String, String> from in froms) {
                fromList.Add(from.First + " " + from.Second);
            }

            return fromList;
        }

        private IList<String> GetOrderList() {
            IList<String> orderList = new List<String>();
            foreach (Pair<String, Boolean> order in orders) {
                orderList.Add(alias + "." + order.First + " " + (order.Second ? "asc" : "desc"));
            }

            return orderList;
        }
    }
}
