using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
	/// <summary>
	/// Automatically adds entity names changed during current revision.
	/// <see cref="ModifiedEntityNamesAttribute"/>
	/// <see cref="DefaultTrackingModifiedTypesRevisionEntity"/>
	/// </summary>
	public class DefaultTrackingModifiedTypesRevisionInfoGenerator : DefaultRevisionInfoGenerator
	{
		private readonly ISetter modifiedEntityNamesSetter;
		private readonly IGetter modifiedEntityNamesGetter;

		public DefaultTrackingModifiedTypesRevisionInfoGenerator(string revisionInfoEntityName, 
																		System.Type revisionInfoType, 
																		IRevisionListener revisionListener, 
																		PropertyData revisionInfoTimestampData, 
																		bool timestampAsDate,
																		PropertyData modifiedEntityNamesData) 
							: base(revisionInfoEntityName, revisionInfoType, revisionListener, revisionInfoTimestampData, timestampAsDate)
		{
			modifiedEntityNamesGetter = ReflectionTools.GetGetter(revisionInfoType, modifiedEntityNamesData);
			modifiedEntityNamesSetter = ReflectionTools.GetSetter(revisionInfoType, modifiedEntityNamesData);
		}

		public override void EntityChanged(System.Type entityClass, string entityName, object entityId, RevisionType revisionType, object revisionEntity)
		{
			base.EntityChanged(entityClass, entityName, entityId, revisionType, revisionEntity);
			var modifiedEntityNames = (ISet<string>)modifiedEntityNamesGetter.Get(revisionEntity);
			if (modifiedEntityNames == null)
			{
				modifiedEntityNames = new HashedSet<string>();
				modifiedEntityNamesSetter.Set(revisionEntity, modifiedEntityNames);
			}
			modifiedEntityNames.Add(entityName);
		}
	}
}