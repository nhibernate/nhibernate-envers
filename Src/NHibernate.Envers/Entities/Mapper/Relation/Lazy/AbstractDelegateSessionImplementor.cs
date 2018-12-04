using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.AdoNet;
using NHibernate.Cache;
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
#pragma warning disable 618

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy
{
	public class AbstractDelegateSessionImplementor : ISessionImplementor
	{
		private readonly ISessionImplementor _session;

		protected AbstractDelegateSessionImplementor(ISessionImplementor session)
		{
			_session = session;
		}

		public Task InitializeCollectionAsync(IPersistentCollection collection, bool writing, CancellationToken cancellationToken)
		{
			return _session.InitializeCollectionAsync(collection, writing, cancellationToken);
		}

		public Task<object> InternalLoadAsync(string entityName, object id, bool eager, bool isNullable, CancellationToken cancellationToken)
		{
			return _session.InternalLoadAsync(entityName, id, eager, isNullable, cancellationToken);
		}

		public Task<object> ImmediateLoadAsync(string entityName, object id, CancellationToken cancellationToken)
		{
			return _session.ImmediateLoadAsync(entityName, id, cancellationToken);
		}

		public Task<IList> ListAsync(IQueryExpression queryExpression, QueryParameters parameters, CancellationToken cancellationToken)
		{
			return _session.ListAsync(queryExpression, parameters, cancellationToken);
		}

		public Task ListAsync(IQueryExpression queryExpression, QueryParameters queryParameters, IList results,
			CancellationToken cancellationToken)
		{
			return _session.ListAsync(queryExpression, queryParameters, results, cancellationToken);
		}

		public Task<IList<T>> ListAsync<T>(IQueryExpression queryExpression, QueryParameters queryParameters,
			CancellationToken cancellationToken)
		{
			return _session.ListAsync<T>(queryExpression, queryParameters, cancellationToken);
		}

		public Task<IList<T>> ListAsync<T>(CriteriaImpl criteria, CancellationToken cancellationToken)
		{
			return _session.ListAsync<T>(criteria, cancellationToken);
		}

		public Task ListAsync(CriteriaImpl criteria, IList results, CancellationToken cancellationToken)
		{
			return _session.ListAsync(criteria, results, cancellationToken);
		}

		public Task<IList> ListAsync(CriteriaImpl criteria, CancellationToken cancellationToken)
		{
			return _session.ListAsync(criteria, cancellationToken);
		}

		public Task<IEnumerable> EnumerableAsync(IQueryExpression query, QueryParameters parameters, CancellationToken cancellationToken)
		{
			return _session.EnumerableAsync(query, parameters, cancellationToken);
		}

		public Task<IEnumerable<T>> EnumerableAsync<T>(IQueryExpression query, QueryParameters queryParameters, CancellationToken cancellationToken)
		{
			return _session.EnumerableAsync<T>(query, queryParameters, cancellationToken);
		}

		public Task<IList> ListFilterAsync(object collection, string filter, QueryParameters parameters, CancellationToken cancellationToken)
		{
			return _session.ListFilterAsync(collection, filter, parameters, cancellationToken);
		}

		public Task<IList> ListFilterAsync(object collection, IQueryExpression queryExpression, QueryParameters parameters,
			CancellationToken cancellationToken)
		{
			return _session.ListFilterAsync(collection, queryExpression, parameters, cancellationToken);
		}

		public Task<IList<T>> ListFilterAsync<T>(object collection, string filter, QueryParameters parameters,
			CancellationToken cancellationToken)
		{
			return _session.ListFilterAsync<T>(collection, filter, parameters, cancellationToken);
		}

		public Task<IEnumerable> EnumerableFilterAsync(object collection, string filter, QueryParameters parameters,
			CancellationToken cancellationToken)
		{
			return _session.EnumerableFilterAsync(collection, filter, parameters, cancellationToken);
		}

		public Task<IEnumerable<T>> EnumerableFilterAsync<T>(object collection, string filter, QueryParameters parameters,
			CancellationToken cancellationToken)
		{
			return _session.EnumerableFilterAsync<T>(collection, filter, parameters, cancellationToken);
		}

		public Task BeforeTransactionCompletionAsync(ITransaction tx, CancellationToken cancellationToken)
		{
			return _session.BeforeTransactionCompletionAsync(tx, cancellationToken);
		}

		public Task FlushBeforeTransactionCompletionAsync(CancellationToken cancellationToken)
		{
			return _session.FlushBeforeTransactionCompletionAsync(cancellationToken);
		}

		public Task AfterTransactionCompletionAsync(bool successful, ITransaction tx, CancellationToken cancellationToken)
		{
			return _session.AfterTransactionCompletionAsync(successful, tx, cancellationToken);
		}

		public Task<IList> ListAsync(NativeSQLQuerySpecification spec, QueryParameters queryParameters, CancellationToken cancellationToken)
		{
			return _session.ListAsync(spec, queryParameters, cancellationToken);
		}

		public Task ListAsync(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results,
			CancellationToken cancellationToken)
		{
			return _session.ListAsync(spec, queryParameters, results, cancellationToken);
		}

		public Task<IList<T>> ListAsync<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters,
			CancellationToken cancellationToken)
		{
			return _session.ListAsync<T>(spec, queryParameters, cancellationToken);
		}

		public Task ListCustomQueryAsync(ICustomQuery customQuery, QueryParameters queryParameters, IList results,
			CancellationToken cancellationToken)
		{
			return _session.ListCustomQueryAsync(customQuery, queryParameters, results, cancellationToken);
		}

		public Task<IList<T>> ListCustomQueryAsync<T>(ICustomQuery customQuery, QueryParameters queryParameters,
			CancellationToken cancellationToken)
		{
			return _session.ListCustomQueryAsync<T>(customQuery, queryParameters, cancellationToken);
		}

		public Task<IQueryTranslator[]> GetQueriesAsync(IQueryExpression query, bool scalar, CancellationToken cancellationToken)
		{
			return _session.GetQueriesAsync(query, scalar, cancellationToken);
		}

		public Task<object> GetEntityUsingInterceptorAsync(EntityKey key, CancellationToken cancellationToken)
		{
			return _session.GetEntityUsingInterceptorAsync(key, cancellationToken);
		}

		public Task FlushAsync(CancellationToken cancellationToken)
		{
			return _session.FlushAsync(cancellationToken);
		}

		public Task<int> ExecuteNativeUpdateAsync(NativeSQLQuerySpecification specification, QueryParameters queryParameters,
			CancellationToken cancellationToken)
		{
			return _session.ExecuteNativeUpdateAsync(specification, queryParameters, cancellationToken);
		}

		public Task<int> ExecuteUpdateAsync(IQueryExpression query, QueryParameters queryParameters, CancellationToken cancellationToken)
		{
			return _session.ExecuteUpdateAsync(query, queryParameters, cancellationToken);
		}

		public Task<IQuery> CreateFilterAsync(object collection, IQueryExpression queryExpression, CancellationToken cancellationToken)
		{
			return _session.CreateFilterAsync(collection, queryExpression, cancellationToken);
		}

		public void Initialize()
		{
			_session.Initialize();
		}

		public void InitializeCollection(IPersistentCollection collection, bool writing)
		{
			_session.InitializeCollection(collection, writing);
		}

		public object InternalLoad(string entityName, object id, bool eager, bool isNullable)
		{
			return _session.InternalLoad(entityName, id, eager, isNullable);
		}

		public virtual object ImmediateLoad(string entityName, object id)
		{
			return _session.ImmediateLoad(entityName, id);
		}

		public IList List(IQueryExpression queryExpression, QueryParameters parameters)
		{
			return _session.List(queryExpression, parameters);
		}

		public IQuery CreateQuery(IQueryExpression queryExpression)
		{
			return _session.CreateQuery(queryExpression);
		}

		public void List(IQueryExpression queryExpression, QueryParameters queryParameters, IList results)
		{
			_session.List(queryExpression, queryParameters, results);
		}

		public IList<T> List<T>(IQueryExpression queryExpression, QueryParameters queryParameters)
		{
			return _session.List<T>(queryExpression, queryParameters);
		}

		public IList<T> List<T>(CriteriaImpl criteria)
		{
			return _session.List<T>(criteria);
		}

		public void List(CriteriaImpl criteria, IList results)
		{
			_session.List(criteria, results);
		}

		public IList List(CriteriaImpl criteria)
		{
			return _session.List(criteria);
		}

		public IEnumerable Enumerable(IQueryExpression query, QueryParameters parameters)
		{
			return _session.Enumerable(query, parameters);
		}

		public IEnumerable<T> Enumerable<T>(IQueryExpression query, QueryParameters queryParameters)
		{
			return _session.Enumerable<T>(query, queryParameters);
		}

		public IList ListFilter(object collection, string filter, QueryParameters parameters)
		{
			return _session.ListFilter(collection, filter, parameters);
		}

		public IList ListFilter(object collection, IQueryExpression queryExpression, QueryParameters parameters)
		{
			return _session.ListFilter(collection, queryExpression, parameters);
		}

		public IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			return _session.ListFilter<T>(collection, filter, parameters);
		}

		public IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters)
		{
			return _session.EnumerableFilter(collection, filter, parameters);
		}

		public IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			return _session.EnumerableFilter<T>(collection, filter, parameters);
		}

		public IEntityPersister GetEntityPersister(string entityName, object obj)
		{
			return _session.GetEntityPersister(entityName, obj);
		}

		public void AfterTransactionBegin(ITransaction tx)
		{
			_session.AfterTransactionBegin(tx);
		}

		public void BeforeTransactionCompletion(ITransaction tx)
		{
			_session.BeforeTransactionCompletion(tx);
		}

		public void FlushBeforeTransactionCompletion()
		{
			_session.FlushBeforeTransactionCompletion();
		}

		public void AfterTransactionCompletion(bool successful, ITransaction tx)
		{
			_session.AfterTransactionCompletion(successful, tx);
		}

		public object GetContextEntityIdentifier(object obj)
		{
			return _session.GetContextEntityIdentifier(obj);
		}

		public object Instantiate(string entityName, object id)
		{
			return _session.Instantiate(entityName, id);
		}

		public IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			return _session.List(spec, queryParameters);
		}

		public void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
		{
			_session.List(spec, queryParameters, results);
		}

		public IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			return _session.List<T>(spec, queryParameters);
		}

		public void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
		{
			_session.ListCustomQuery(customQuery, queryParameters, results);
		}

		public IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters)
		{
			return _session.ListCustomQuery<T>(customQuery, queryParameters);
		}

		public object GetFilterParameterValue(string filterParameterName)
		{
			return _session.GetFilterParameterValue(filterParameterName);
		}

		public IType GetFilterParameterType(string filterParameterName)
		{
			return _session.GetFilterParameterType(filterParameterName);
		}

		public IQuery GetNamedSQLQuery(string name)
		{
			return _session.GetNamedSQLQuery(name);
		}

		public IQueryTranslator[] GetQueries(IQueryExpression query, bool scalar)
		{
			return _session.GetQueries(query, scalar);
		}

		public object GetEntityUsingInterceptor(EntityKey key)
		{
			return _session.GetEntityUsingInterceptor(key);
		}

		public string BestGuessEntityName(object entity)
		{
			return _session.BestGuessEntityName(entity);
		}

		public string GuessEntityName(object entity)
		{
			return _session.GuessEntityName(entity);
		}

		public IQuery GetNamedQuery(string queryName)
		{
			return _session.GetNamedQuery(queryName);
		}

		public void Flush()
		{
			_session.Flush();
		}

		public int ExecuteNativeUpdate(NativeSQLQuerySpecification specification, QueryParameters queryParameters)
		{
			return _session.ExecuteNativeUpdate(specification, queryParameters);
		}

		public int ExecuteUpdate(IQueryExpression query, QueryParameters queryParameters)
		{
			return _session.ExecuteUpdate(query, queryParameters);
		}

		public void JoinTransaction()
		{
			_session.JoinTransaction();
		}

		public void CloseSessionFromSystemTransaction()
		{
			_session.CloseSessionFromSystemTransaction();
		}

		public IQuery CreateFilter(object collection, IQueryExpression queryExpression)
		{
			return _session.CreateFilter(collection, queryExpression);
		}

		public EntityKey GenerateEntityKey(object id, IEntityPersister persister)
		{
			return _session.GenerateEntityKey(id, persister);
		}

		public CacheKey GenerateCacheKey(object id, IType type, string entityOrRoleName)
		{
			return _session.GenerateCacheKey(id, type, entityOrRoleName);
		}

		public long Timestamp => _session.Timestamp;

		public ISessionFactoryImplementor Factory => _session.Factory;

		public IBatcher Batcher => _session.Batcher;

		public IDictionary<string, IFilter> EnabledFilters => _session.EnabledFilters;

		public IInterceptor Interceptor => _session.Interceptor;

		public EventListeners Listeners => _session.Listeners;

		public ConnectionManager ConnectionManager => _session.ConnectionManager;

		public bool IsEventSource => _session.IsEventSource;

		public IPersistenceContext PersistenceContext => _session.PersistenceContext;

		public CacheMode CacheMode
		{
			get => _session.CacheMode;
			set => _session.CacheMode = value;
		}

		public bool IsOpen => _session.IsOpen;

		public bool IsConnected => _session.IsConnected;

		public FlushMode FlushMode
		{
			get => _session.FlushMode;
			set => _session.FlushMode = value;
		}

		public string FetchProfile
		{
			get => _session.FetchProfile;
			set => _session.FetchProfile = value;
		}

		public DbConnection Connection => _session.Connection;

		public bool IsClosed => _session.IsClosed;

		public bool TransactionInProgress => _session.TransactionInProgress;

		public FutureCriteriaBatch FutureCriteriaBatch => _session.FutureCriteriaBatch;

		public FutureQueryBatch FutureQueryBatch => _session.FutureQueryBatch;

		public Guid SessionId => _session.SessionId;

		public ITransactionContext TransactionContext
		{
			get => _session.TransactionContext;
			set => _session.TransactionContext = value;
		}
	}
}