using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NHibernate.AdoNet;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Loader.Custom;
using NHibernate.Persister.Entity;
using NHibernate.Transaction;
using NHibernate.Type;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public abstract class AbstractDelegateSessionImplementor : ISessionImplementor 
	{
		protected AbstractDelegateSessionImplementor(ISessionImplementor delegat) 
		{
			SessionDelegate = delegat;
		}

		protected ISessionImplementor SessionDelegate { get; private set; }

		protected abstract object DoImmediateLoad(string entityName);

		public void Initialize()
		{
			SessionDelegate.Initialize();
		}

		public void InitializeCollection(IPersistentCollection collection, bool writing)
		{
			SessionDelegate.InitializeCollection(collection, writing);
		}

		public object InternalLoad(string entityName, object id, bool eager, bool isNullable)
		{
			return SessionDelegate.InternalLoad(entityName, id, eager, isNullable);
		}

		public object ImmediateLoad(string entityName, object id)
		{
			return DoImmediateLoad(entityName);
		}

		public long Timestamp
		{
			get { return SessionDelegate.Timestamp; }
		}

		public ISessionFactoryImplementor Factory
		{
			get { return SessionDelegate.Factory; }
		}

		public IBatcher Batcher
		{
			get { return SessionDelegate.Batcher; }
		}

		public IList List(string query, QueryParameters parameters)
		{
			return SessionDelegate.List(query, parameters);
		}

		public IList List(IQueryExpression queryExpression, QueryParameters parameters)
		{
			return SessionDelegate.List(queryExpression, parameters);
		}

		public IQuery CreateQuery(IQueryExpression queryExpression)
		{
			return SessionDelegate.CreateQuery(queryExpression);
		}

		public void List(string query, QueryParameters parameters, IList results)
		{
			SessionDelegate.List(query, parameters, results);
		}

		public IList<T> List<T>(string query, QueryParameters parameters)
		{
			return SessionDelegate.List<T>(query, parameters);
		}

		public IList<T> List<T>(CriteriaImpl criteria)
		{
			return SessionDelegate.List<T>(criteria);
		}

		public void List(CriteriaImpl criteria, IList results)
		{
			SessionDelegate.List(criteria, results);
		}

		public IList List(CriteriaImpl criteria)
		{
			return SessionDelegate.List(criteria);
		}

		public IEnumerable Enumerable(string query, QueryParameters parameters)
		{
			return SessionDelegate.Enumerable(query, parameters);
		}

		public IEnumerable<T> Enumerable<T>(string query, QueryParameters parameters)
		{
			return SessionDelegate.Enumerable<T>(query, parameters);
		}

		public IList ListFilter(object collection, string filter, QueryParameters parameters)
		{
			return SessionDelegate.ListFilter(collection, filter, parameters);
		}

		public IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			return SessionDelegate.ListFilter<T>(collection, filter, parameters);
		}

		public IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters)
		{
			return SessionDelegate.EnumerableFilter(collection, filter, parameters);
		}

		public IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			return SessionDelegate.EnumerableFilter<T>(collection, filter, parameters);
		}

		public IEntityPersister GetEntityPersister(string entityName, object obj)
		{
			return SessionDelegate.GetEntityPersister(entityName, obj);
		}

		public void AfterTransactionBegin(ITransaction tx)
		{
			SessionDelegate.AfterTransactionBegin(tx);
		}

		public void BeforeTransactionCompletion(ITransaction tx)
		{
			SessionDelegate.BeforeTransactionCompletion(tx);
		}

		public void AfterTransactionCompletion(bool successful, ITransaction tx)
		{
			SessionDelegate.AfterTransactionCompletion(successful, tx);
		}

		public object GetContextEntityIdentifier(object obj)
		{
			return SessionDelegate.GetContextEntityIdentifier(obj);
		}

		public object Instantiate(string entityName, object id)
		{
			return SessionDelegate.Instantiate(entityName, id);
		}

		public IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			return SessionDelegate.List(spec, queryParameters);
		}

		public void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
		{
			SessionDelegate.List(spec, queryParameters, results);
		}

		public IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			return SessionDelegate.List<T>(spec, queryParameters);
		}

		public void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
		{
			SessionDelegate.ListCustomQuery(customQuery,queryParameters, results);
		}

		public IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters)
		{
			return SessionDelegate.ListCustomQuery<T>(customQuery, queryParameters);
		}

		public object GetFilterParameterValue(string filterParameterName)
		{
			return SessionDelegate.GetFilterParameterValue(filterParameterName);
		}

		public IType GetFilterParameterType(string filterParameterName)
		{
			return SessionDelegate.GetFilterParameterType(filterParameterName);
		}

		public IDictionary<string, IFilter> EnabledFilters
		{
			get { return SessionDelegate.EnabledFilters; }
		}

		public IQuery GetNamedSQLQuery(string name)
		{
			return SessionDelegate.GetNamedSQLQuery(name);
		}

		public IQueryTranslator[] GetQueries(string query, bool scalar)
		{
			return SessionDelegate.GetQueries(query, scalar);
		}

		public IInterceptor Interceptor
		{
			get { return SessionDelegate.Interceptor; }
		}

		public EventListeners Listeners
		{
			get { return SessionDelegate.Listeners; }
		}

		public int DontFlushFromFind
		{
			get { return SessionDelegate.DontFlushFromFind; }
		}

		public ConnectionManager ConnectionManager
		{
			get { return SessionDelegate.ConnectionManager; }
		}

		public bool IsEventSource
		{
			get { return SessionDelegate.IsEventSource; }
		}

		public object GetEntityUsingInterceptor(EntityKey key)
		{
			return SessionDelegate.GetEntityUsingInterceptor(key);
		}

		public IPersistenceContext PersistenceContext
		{
			get { return SessionDelegate.PersistenceContext; }
		}

		public CacheMode CacheMode
		{
			get
			{
				return SessionDelegate.CacheMode;
			}
			set
			{
				SessionDelegate.CacheMode = value;
			}
		}

		public bool IsOpen
		{
			get { return SessionDelegate.IsOpen; }
		}

		public bool IsConnected
		{
			get { return SessionDelegate.IsConnected; }
		}

		public FlushMode FlushMode
		{
			get
			{
				return SessionDelegate.FlushMode;
			}
			set
			{
				SessionDelegate.FlushMode = value;
			}
		}

		public string FetchProfile
		{
			get
			{
				return SessionDelegate.FetchProfile;
			}
			set
			{
				SessionDelegate.FetchProfile = value;
			}
		}

		public string BestGuessEntityName(object entity)
		{
			return SessionDelegate.BestGuessEntityName(entity);
		}

		public string GuessEntityName(object entity)
		{
			return SessionDelegate.GuessEntityName(entity);
		}

		public IDbConnection Connection
		{
			get { return SessionDelegate.Connection; }
		}

		public IQuery GetNamedQuery(string queryName)
		{
			return SessionDelegate.GetNamedQuery(queryName);
		}

		public bool IsClosed
		{
			get { return SessionDelegate.IsClosed; }
		}

		public void Flush()
		{
			SessionDelegate.Flush();
		}

		public bool TransactionInProgress
		{
			get { return SessionDelegate.TransactionInProgress; }
		}

		public EntityMode EntityMode
		{
			get { return SessionDelegate.EntityMode; }
		}

		public int ExecuteNativeUpdate(NativeSQLQuerySpecification specification, QueryParameters queryParameters)
		{
			return SessionDelegate.ExecuteNativeUpdate(specification, queryParameters);
		}

		public int ExecuteUpdate(string query, QueryParameters queryParameters)
		{
			return SessionDelegate.ExecuteUpdate(query, queryParameters);
		}

		public FutureCriteriaBatch FutureCriteriaBatch
		{
			get { return SessionDelegate.FutureCriteriaBatch; }
		}

		public FutureQueryBatch FutureQueryBatch
		{
			get { return SessionDelegate.FutureQueryBatch; }
		}

		public Guid SessionId
		{
			get { return SessionDelegate.SessionId; }
		}

		public ITransactionContext TransactionContext
		{
			get
			{
				return SessionDelegate.TransactionContext;
			}
			set
			{
				SessionDelegate.TransactionContext = value;
			}
		}

		public void CloseSessionFromDistributedTransaction()
		{
			SessionDelegate.CloseSessionFromDistributedTransaction();
		}
	}
}
