﻿using Iesi.Collections.Generic;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
	/// <summary>
	/// Returns modified entity types from a persisted revision info entity.
	/// </summary>
	public class ModifiedEntityNamesReader
	{
		private readonly IGetter modifiedEntityTypesGetter;

		public ModifiedEntityNamesReader(System.Type revisionInfoClass, PropertyData modifiedEntityTypesData)
		{
			modifiedEntityTypesGetter = ReflectionTools.GetGetter(revisionInfoClass, modifiedEntityTypesData);
		}

		public ISet<string> ModifiedEntityTypes(object revisionEntity)
		{
			return (ISet<string>)modifiedEntityTypesGetter.Get(revisionEntity);
		}
	}
}