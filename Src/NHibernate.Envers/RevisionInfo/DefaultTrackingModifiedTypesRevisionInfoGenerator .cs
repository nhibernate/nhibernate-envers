using Iesi.Collections.Generic;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
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

		public override void AddEntityToRevision(string entityName, object revisionInfo)
		{
			base.AddEntityToRevision(entityName, revisionInfo);
			var modifiedEntityNames = (ISet<string>)modifiedEntityNamesGetter.Get(revisionInfo);
			if (modifiedEntityNames == null)
			{
				modifiedEntityNames = new HashedSet<string>();
			}
			modifiedEntityNames.Add(entityName);
			modifiedEntityNamesSetter.Set(revisionInfo, modifiedEntityNames);
		}

		public override void RemoveEntityFromRevision(string entityName, object revisionInfo)
		{
			base.RemoveEntityFromRevision(entityName, revisionInfo);
			var modifiedEntityNames = (ISet<string>)modifiedEntityNamesGetter.Get(revisionInfo);
			if (modifiedEntityNames == null)
			{
				return;
			}
			modifiedEntityNames.Remove(entityName);
			modifiedEntityNamesSetter.Set(revisionInfo, modifiedEntityNames);

		}
	}
}