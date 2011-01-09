using System;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;

namespace NHibernate.Envers.Configuration.Metadata
{
	public class CollectionMapperFactory
	{
		public IPropertyMapper CreateSetCollectionMapper(System.Type genericTypeArgument,
																CommonCollectionMapperData commonCollectionMapperData,
																System.Type collectionType,
																System.Type proxyType,
																MiddleComponentData elementComponentData)
		{
			var genericArgs = new[] { genericTypeArgument };
			var type = typeof(SetCollectionMapper<>).MakeGenericType(genericArgs);
			var proxyTypeGenerics = proxyType.MakeGenericType(genericArgs);
			var collectionTypeGenerics = collectionType.MakeGenericType(genericArgs);
			return
				(IPropertyMapper)
				Activator.CreateInstance(type, commonCollectionMapperData, collectionTypeGenerics, proxyTypeGenerics, elementComponentData);
		}

		public IPropertyMapper CreateBagCollectionMapper(System.Type genericTypeArgument,
																CommonCollectionMapperData commonCollectionMapperData,
																MiddleComponentData elementComponentData)
		{
			var type = typeof(BagCollectionMapper<>).MakeGenericType(new[] { genericTypeArgument });
			return
				(IPropertyMapper)
				Activator.CreateInstance(type, commonCollectionMapperData, elementComponentData);

		}

		public IPropertyMapper CreateListCollectionMapper(System.Type genericTypeArgument,
															CommonCollectionMapperData commonCollectionMapperData,
															MiddleComponentData elementComponentData,
															MiddleComponentData indexComponentData)
		{
			var type = typeof(ListCollectionMapper<>).MakeGenericType(new[] { genericTypeArgument });
			return (IPropertyMapper)
				   Activator.CreateInstance(type, commonCollectionMapperData, elementComponentData, indexComponentData);
		}

		public IPropertyMapper CreateMapCollectionMapper(System.Type keyType,
														System.Type valueType,
														CommonCollectionMapperData commonCollectionMapperData,
														System.Type dictionaryType,
														System.Type proxyType,
														MiddleComponentData elementComponentData,
														MiddleComponentData indexComponentData)
		{
			var genericArgs = new[] { keyType, valueType };
			var type = typeof(MapCollectionMapper<,>).MakeGenericType(genericArgs);
			var proxyTypeGeneric = proxyType.MakeGenericType(genericArgs);
			var dictionaryTypeGenerics = dictionaryType.MakeGenericType(genericArgs);
			return (IPropertyMapper)
				   Activator.CreateInstance(type, commonCollectionMapperData, dictionaryTypeGenerics, proxyTypeGeneric, elementComponentData,
											indexComponentData);
		}
	}
}