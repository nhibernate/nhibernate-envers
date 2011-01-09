using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Collection;
using System.Collections;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Loader.Custom;
using NHibernate.Engine.Query.Sql;
using NHibernate.Type;
using NHibernate.Event;
using System.Data;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public abstract class AbstractDelegateSessionImplementor : ISessionImplementor 
	{
		protected ISessionImplementor delegat;

		public AbstractDelegateSessionImplementor(ISessionImplementor delegat) 
		{
			this.delegat = delegat;
		}

		public abstract Object doImmediateLoad(String entityName);

		public Object immediateLoad(String entityName, Object id) 
		{ 
			return doImmediateLoad(entityName);
		}

		// Delegate methods

		public IInterceptor getInterceptor() 
		{
			return delegat.Interceptor;
		}

		public void setAutoClear(bool enabled) 
		{
			//delegat.setAutoClear(enabled);// not implemented yet
			throw new NotImplementedException();
		}

		public bool isTransactionInProgress() 
		{
			return delegat.TransactionInProgress;
		}

		public void initializeCollection(IPersistentCollection collection, bool writing)  
		{
			delegat.InitializeCollection(collection, writing);
		}

		public Object internalLoad(String entityName, object id, bool eager, bool nullable) 
		{
			return delegat.InternalLoad(entityName, id, eager, nullable);
		}

		public long getTimestamp() 
		{
			return delegat.Timestamp;
		}

		public ISessionFactoryImplementor getFactory() 
		{
			return delegat.Factory;
		}

		public IBatcher getBatcher() 
		{
			return delegat.Batcher;
		}

		public IList list(String query, QueryParameters queryParameters)
		{
			return delegat.List(query, queryParameters);
		}
		// TODO in second implementation phase
		//public Iterator iterate(String query, QueryParameters queryParameters)  {
		//    return delegat.iterate(query, queryParameters);
		//}

		// TODO in second implementation phase
		//public ScrollableResults scroll(String query, QueryParameters queryParameters) throws HibernateException {
		//    return delegate.scroll(query, queryParameters);
		//}

			// TODO in second implementation phase
		//public ScrollableResults scroll(CriteriaImpl criteria, ScrollMode scrollMode) {
		//    return delegate.scroll(criteria, scrollMode);
		//}

		public IList list(CriteriaImpl criteria) {
			return delegat.List(criteria);
		}

		public IList listFilter(Object collection, String filter, QueryParameters queryParameters)  {
			return delegat.ListFilter(collection, filter, queryParameters);
		}
		// TODO in second implementation phase
		//public IEnumerator iterateFilter(Object collection, String filter, QueryParameters queryParameters){
		//    return delegat.iterateFilter(collection, filter, queryParameters);
		//}

		public IEntityPersister getEntityPersister(String entityName, Object obj)  {
			return delegat.GetEntityPersister(entityName, obj);
		}

		public Object getEntityUsingInterceptor(EntityKey key) {
			return delegat.GetEntityUsingInterceptor(key);
		}

		public void afterTransactionCompletion(bool successful, ITransaction tx) {
			delegat.AfterTransactionCompletion(successful, tx);
		}

		public void beforeTransactionCompletion(ITransaction tx) {
			delegat.BeforeTransactionCompletion(tx);
		}

		public object getContextEntityIdentifier(Object obj) {
			return delegat.GetContextEntityIdentifier(obj);
		}

		public String bestGuessEntityName(Object obj) {
			return delegat.BestGuessEntityName(obj);
		}

		public String guessEntityName(Object entity) {
			return delegat.GuessEntityName(entity);
		}

		public Object instantiate(String entityName, object id) {
			return delegat.Instantiate(entityName, id);
		}
		//// TODO in second implementation phase
		//public IList listCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters) {
		//    //IList list;
		//    //delegat.ListCustomQuery(customQuery, queryParameters, list);
		//    //return list;
		//}

		//public ScrollableResults scrollCustomQuery(CustomQuery customQuery, QueryParameters queryParameters) throws HibernateException {
		//    return delegate.scrollCustomQuery(customQuery, queryParameters);
		//}

		public IList list(NativeSQLQuerySpecification spec, QueryParameters queryParameters) {
			return delegat.List(spec, queryParameters);
		}
		// TODO in second implementation phase
		//public ScrollableResults scroll(NativeSQLQuerySpecification spec, QueryParameters queryParameters) throws HibernateException {
		//    return delegate.scroll(spec, queryParameters);
		//}

		public Object getFilterParameterValue(String filterParameterName) {
			return delegat.GetFilterParameterValue(filterParameterName);
		}

		public IType getFilterParameterType(String filterParameterName) {
			return delegat.GetFilterParameterType(filterParameterName);
		}

		public IDictionary<String, IFilter> getEnabledFilters() {
			return delegat.EnabledFilters;
		}

		public int getDontFlushFromFind() {
			return delegat.DontFlushFromFind;
		}

		public EventListeners getListeners() {
			return delegat.Listeners;
		}

		public IPersistenceContext getPersistenceContext() {
			return delegat.PersistenceContext;
		}

		public int executeUpdate(String query, QueryParameters queryParameters){
			return delegat.ExecuteUpdate(query, queryParameters);
		}

		public int executeNativeUpdate(NativeSQLQuerySpecification specification, QueryParameters queryParameters)  {
			return delegat.ExecuteNativeUpdate(specification, queryParameters);
		}

		public EntityMode getEntityMode() {
			return delegat.EntityMode;
		}

		public CacheMode getCacheMode() {
			return delegat.CacheMode;
		}

		public void setCacheMode(CacheMode cm) {
			delegat.CacheMode = cm;
		}

		public bool isOpen() {
			return delegat.IsOpen;
		}

		public bool isConnected() {
			return delegat.IsConnected;
		}

		public FlushMode getFlushMode() {
			return delegat.FlushMode;
		}

		public void setFlushMode(FlushMode fm) {
			delegat.FlushMode = fm ;
		}

		public IDbConnection connection() {
			return delegat.Connection; 
		}

		public void flush() {
			delegat.Flush();
		}

		public IQuery getNamedQuery(String name) {
			return delegat.GetNamedQuery(name);
		}

		public IQuery getNamedSQLQuery(String name) {
			return delegat.GetNamedSQLQuery(name);
		}

		public bool isEventSource() {
			return delegat.IsEventSource;
		}
		// TODO in second implementation phase  
		//public void afterScrollOperation() {
		//    delegate.afterScrollOperation();
		//}

		public void setFetchProfile(String name) {
			delegat.FetchProfile = name;
		}

		public String getFetchProfile() {
			return delegat.FetchProfile;
		}

		//public JDBCContext getJDBCContext() {
		//    return delegate.getJDBCContext();
		//}

		public bool isClosed() {
			return delegat.IsClosed;
		}

		#region ISessionImplementor Members

		public void Initialize()
		{
			delegat.Initialize();
		}

		public void InitializeCollection(IPersistentCollection collection, bool writing)
		{
			delegat.InitializeCollection(collection, writing);
		}

		public object InternalLoad(string entityName, object id, bool eager, bool isNullable)
		{
			return delegat.InternalLoad(entityName, id, eager, isNullable);
		}

		public object ImmediateLoad(string entityName, object id)
		{
			return doImmediateLoad(entityName);
		}

		public long Timestamp
		{
			get { return delegat.Timestamp; }
		}

		public ISessionFactoryImplementor Factory
		{
			get { return delegat.Factory; }
		}

		public IBatcher Batcher
		{
			get { return delegat.Batcher; }
		}

		public IList List(string query, QueryParameters parameters)
		{
			return delegat.List(query, parameters);
		}

		public IList List(IQueryExpression queryExpression, QueryParameters parameters)
		{
			return delegat.List(queryExpression, parameters);
		}

		public IQuery CreateQuery(IQueryExpression queryExpression)
		{
			return delegat.CreateQuery(queryExpression);
		}

		public void List(string query, QueryParameters parameters, IList results)
		{
			delegat.List(query, parameters, results);
		}

		public IList<T> List<T>(string query, QueryParameters queryParameters)
		{
			return delegat.List<T>(query, queryParameters);
		}

		public IList<T> List<T>(CriteriaImpl criteria)
		{
			return delegat.List<T>(criteria);
		}

		public void List(CriteriaImpl criteria, IList results)
		{
			delegat.List(criteria, results);
		}

		public IList List(CriteriaImpl criteria)
		{
			return delegat.List(criteria);
		}

		public IEnumerable Enumerable(string query, QueryParameters parameters)
		{
			return delegat.Enumerable(query, parameters);
		}

		public IEnumerable<T> Enumerable<T>(string query, QueryParameters queryParameters)
		{
			return delegat.Enumerable<T>(query, queryParameters);
		}

		public IList ListFilter(object collection, string filter, QueryParameters parameters)
		{
			return delegat.ListFilter(collection, filter, parameters);
		}

		public IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			return delegat.ListFilter<T>(collection, filter, parameters);
		}

		public IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters)
		{
			return delegat.EnumerableFilter(collection, filter, parameters);
		}

		public IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			return delegat.EnumerableFilter<T>(collection, filter, parameters);
		}

		public IEntityPersister GetEntityPersister(string entityName, object obj)
		{
			return delegat.GetEntityPersister(entityName, obj);
		}

		public void AfterTransactionBegin(ITransaction tx)
		{
			delegat.AfterTransactionBegin(tx);
		}

		public void BeforeTransactionCompletion(ITransaction tx)
		{
			delegat.BeforeTransactionCompletion(tx);
		}

		public void AfterTransactionCompletion(bool successful, ITransaction tx)
		{
			delegat.AfterTransactionCompletion(successful, tx);
		}

		public object GetContextEntityIdentifier(object obj)
		{
			return delegat.GetContextEntityIdentifier(obj);
		}

		public object Instantiate(string entityName, object id)
		{
			return delegat.Instantiate(entityName, id);
		}

		public IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			return delegat.List(spec, queryParameters);
		}

		public void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
		{
			delegat.List(spec, queryParameters, results);
		}

		public IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			return delegat.List<T>(spec, queryParameters);
		}

		public void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
		{
			delegat.ListCustomQuery(customQuery,queryParameters, results);
		}

		public IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters)
		{
			return delegat.ListCustomQuery<T>(customQuery, queryParameters);
		}

		public object GetFilterParameterValue(string filterParameterName)
		{
			return delegat.GetFilterParameterValue(filterParameterName);
		}

		public IType GetFilterParameterType(string filterParameterName)
		{
			return delegat.GetFilterParameterType(filterParameterName);
		}

		public IDictionary<string, IFilter> EnabledFilters
		{
			get { return delegat.EnabledFilters; }
		}

		public IQuery GetNamedSQLQuery(string name)
		{
			return delegat.GetNamedSQLQuery(name);
		}

		public NHibernate.Hql.IQueryTranslator[] GetQueries(string query, bool scalar)
		{
			return delegat.GetQueries(query, scalar);
		}

		public IInterceptor Interceptor
		{
			get { return delegat.Interceptor; }
		}

		public EventListeners Listeners
		{
			get { return delegat.Listeners; }
		}

		public int DontFlushFromFind
		{
			get { return delegat.DontFlushFromFind; }
		}

		public NHibernate.AdoNet.ConnectionManager ConnectionManager
		{
			get { return delegat.ConnectionManager; }
		}

		public bool IsEventSource
		{
			get { return delegat.IsEventSource; }
		}

		public object GetEntityUsingInterceptor(EntityKey key)
		{
			return delegat.GetEntityUsingInterceptor(key);
		}

		public IPersistenceContext PersistenceContext
		{
			get { return delegat.PersistenceContext; }
		}

		public CacheMode CacheMode
		{
			get
			{
				return delegat.CacheMode;
			}
			set
			{
				delegat.CacheMode = value;
			}
		}

		public bool IsOpen
		{
			get { return delegat.IsOpen; }
		}

		public bool IsConnected
		{
			get { return delegat.IsConnected; }
		}

		public FlushMode FlushMode
		{
			get
			{
				return delegat.FlushMode;
			}
			set
			{
				delegat.FlushMode = value;
			}
		}

		public string FetchProfile
		{
			get
			{
				return delegat.FetchProfile;
			}
			set
			{
				delegat.FetchProfile = value;
			}
		}

		public string BestGuessEntityName(object entity)
		{
			return delegat.BestGuessEntityName(entity);
		}

		public string GuessEntityName(object entity)
		{
			return delegat.GuessEntityName(entity);
		}

		public IDbConnection Connection
		{
			get { return delegat.Connection; }
		}

		public IQuery GetNamedQuery(string queryName)
		{
			return delegat.GetNamedQuery(queryName);
		}

		public bool IsClosed
		{
			get { return delegat.IsClosed; }
		}

		public void Flush()
		{
			delegat.Flush();
		}

		public bool TransactionInProgress
		{
			get { return delegat.TransactionInProgress; }
		}

		public EntityMode EntityMode
		{
			get { return delegat.EntityMode; }
		}

		public int ExecuteNativeUpdate(NativeSQLQuerySpecification specification, QueryParameters queryParameters)
		{
			return delegat.ExecuteNativeUpdate(specification, queryParameters);
		}

		public int ExecuteUpdate(string query, QueryParameters queryParameters)
		{
			return delegat.ExecuteUpdate(query, queryParameters);
		}

		public FutureCriteriaBatch FutureCriteriaBatch
		{
			get { return delegat.FutureCriteriaBatch; }
		}

		public FutureQueryBatch FutureQueryBatch
		{
			get { return delegat.FutureQueryBatch; }
		}

		public Guid SessionId
		{
			get { return delegat.SessionId; }
		}

		public NHibernate.Transaction.ITransactionContext TransactionContext
		{
			get
			{
				return delegat.TransactionContext;
			}
			set
			{
				delegat.TransactionContext = value;
			}
		}

		public void CloseSessionFromDistributedTransaction()
		{
			delegat.CloseSessionFromDistributedTransaction();
		}

		#endregion
	}
}
