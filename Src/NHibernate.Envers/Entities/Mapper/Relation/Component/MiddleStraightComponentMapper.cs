﻿using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	/// <summary>
	/// A mapper for reading and writing a property straight to/from maps. This mapper cannot be used with middle tables,
	/// but only with "fake" bidirectional indexed relations. 
	/// </summary>
	public sealed class MiddleStraightComponentMapper : IMiddleComponentMapper 
	{
		private readonly string _propertyName;

		public MiddleStraightComponentMapper(string propertyName) 
		{
			_propertyName = propertyName;
		}

		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary data, object dataObject, long revision)
		{
			return data.Contains(_propertyName) ? data[_propertyName] : null;
		}

		public void MapToMapFromObject(IDictionary<string, object> data, object obj) 
		{
			data.Add(_propertyName, obj);
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string prefix1, string prefix2) 
		{
			throw new NotSupportedException("Cannot use this mapper with a middle table!");
		}
	}
}
