﻿namespace NHibernate.Envers.RevisionInfo
{
	public interface IRevisionInfoGenerator
	{
		void SaveRevisionData(ISession session, object revisionData);
		object Generate();
		void AddEntityToRevision(string entityName, object revisionInfo);
		void RemoveEntityFromRevision(string entityName, object revisionInfo);
	}
}
