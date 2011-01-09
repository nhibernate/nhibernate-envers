using System;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
	public class RevisionInfoNumberReader 
	{
		private readonly IGetter getter;

		public RevisionInfoNumberReader(System.Type revisionInfoType, PropertyData revisionInfoIdData)
		{
    		getter = ReflectionTools.GetGetter(revisionInfoType, revisionInfoIdData);
		}

		public long RevisionNumber(object revision)
		{
			return Convert.ToInt64(getter.Get(revision));
		}
	}
}
