﻿using System;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
	[Serializable]
	public class RevisionInfoNumberReader 
	{
		private readonly IGetter _getter;

		public RevisionInfoNumberReader(System.Type revisionInfoType, PropertyData revisionInfoIdData)
		{
			_getter = ReflectionTools.GetGetter(revisionInfoType, revisionInfoIdData);
		}

		public long RevisionNumber(object revision)
		{
			return Convert.ToInt64(_getter.Get(revision));
		}
	}
}
