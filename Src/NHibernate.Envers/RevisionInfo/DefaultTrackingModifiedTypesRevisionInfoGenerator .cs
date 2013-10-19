using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
	/// <summary>
	/// Automatically adds entity class names, that have been changed during current revision, to revision entity.
	/// <see cref="ModifiedEntityNamesAttribute"/>
	/// <see cref="DefaultTrackingModifiedEntitiesRevisionEntity"/>
	/// </summary>
	[Serializable]
	public class DefaultTrackingModifiedEntitiesRevisionInfoGenerator : DefaultRevisionInfoGenerator
	{
		private readonly ISetter modifiedEntityTypesSetter;
		private readonly IGetter modifiedEntityTypesGetter;

		public DefaultTrackingModifiedEntitiesRevisionInfoGenerator(string revisionInfoEntityName, 
																		System.Type revisionInfoType, 
																		IRevisionListener revisionListener, 
																		PropertyData revisionInfoTimestampData, 
																		bool timestampAsDate,
																		PropertyData modifiedEntityNamesData) 
							: base(revisionInfoEntityName, revisionInfoType, revisionListener, revisionInfoTimestampData, timestampAsDate)
		{
			modifiedEntityTypesGetter = ReflectionTools.GetGetter(revisionInfoType, modifiedEntityNamesData);
			modifiedEntityTypesSetter = ReflectionTools.GetSetter(revisionInfoType, modifiedEntityNamesData);
		}

		public override void EntityChanged(System.Type entityClass, string entityName, object entityId, RevisionType revisionType, object revisionEntity)
		{
			base.EntityChanged(entityClass, entityName, entityId, revisionType, revisionEntity);
			var modifiedEntityNames = (ISet<string>)modifiedEntityTypesGetter.Get(revisionEntity);
			if (modifiedEntityNames == null)
			{
				modifiedEntityNames = new HashSet<string>();
				modifiedEntityTypesSetter.Set(revisionEntity, modifiedEntityNames);
			}
			modifiedEntityNames.Add(entityName);
		}
	}
}