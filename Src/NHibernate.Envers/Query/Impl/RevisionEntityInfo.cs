namespace NHibernate.Envers.Query.Impl
{
	public class RevisionEntityInfo<TEntity, TRevisionEntity> : IRevisionEntityInfo<TEntity, TRevisionEntity>
	{
		public RevisionEntityInfo(TEntity entity, TRevisionEntity revisionEntity, RevisionType operation)
		{
			Entity = entity;
			RevisionEntity = revisionEntity;
			Operation = operation;
		}

		public TEntity Entity { get; private set; }
		public TRevisionEntity RevisionEntity { get; private set; }
		public RevisionType Operation { get; private set; }
	}
}