﻿using NHibernate.Envers.Entities.Mapper.Relation;

namespace NHibernate.Envers.Entities.Mapper
{
	/// <summary>
	/// Creates a user defined <see cref="IPropertyMapper"/> for a collection.
	/// </summary>
	public interface ICustomCollectionMapperFactory
	{
		IPropertyMapper Create(ICollectionProxyFactory collectionProxyFactory, 
						CommonCollectionMapperData commonCollectionMapperData,
						MiddleComponentData elementComponentData, 
						MiddleComponentData indexComponentData);
	}
}