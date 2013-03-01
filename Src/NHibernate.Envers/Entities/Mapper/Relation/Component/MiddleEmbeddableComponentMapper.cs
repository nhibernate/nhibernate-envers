using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Query;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
	public class MiddleEmbeddableComponentMapper : IMiddleComponentMapper, ICompositeMapperBuilder
	{
		private readonly MultiPropertyMapper _delegate;
		private readonly System.Type _componentClass;

		public MiddleEmbeddableComponentMapper(MultiPropertyMapper theDelegate, string componentClassName)
		{
			_delegate = theDelegate;
			_componentClass = Toolz.ResolveDotnetType(componentClassName);
		}

		public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary data, object dataObject, long revision)
		{
			var componentInstance = dataObject ?? ReflectionTools.CreateInstanceByDefaultConstructor(_componentClass);
			_delegate.MapToEntityFromMap(entityInstantiator.AuditConfiguration, componentInstance, data,null,
								entityInstantiator.AuditReaderImplementor, revision);
			return componentInstance;
		}

		public void MapToMapFromObject(ISessionImplementor session, IDictionary<string, object> idData, IDictionary<string, object> data, object obj)
		{
			_delegate.MapToMapFromEntity( session, data, obj, obj );
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string idPrefix1, string prefix1, string idPrefix2, string prefix2)
		{
			AddMiddleEqualToQuery(_delegate, parameters, idPrefix1, prefix1, idPrefix2, prefix2 );
		}

		protected void AddMiddleEqualToQuery(ICompositeMapperBuilder compositeMapper, Parameters parameters, string idPrefix1,
		                                     string prefix1, string idPrefix2, string prefix2)
		{
			foreach (var keyValue in compositeMapper.Properties)
			{
				var propertyName = keyValue.Key.Name;
				var nestedMapper = keyValue.Value;

				var nestedCompositeMapper = nestedMapper as ICompositeMapperBuilder;
				if (nestedCompositeMapper != null)
				{
					AddMiddleEqualToQuery(nestedCompositeMapper, parameters, idPrefix1, prefix1, idPrefix2, prefix2);
				}
				else
				{
					var nestedToOneIdMapper = nestedMapper as ToOneIdMapper;
					if (nestedToOneIdMapper != null)
					{
						nestedToOneIdMapper.AddMiddleEqualToQuery(parameters, idPrefix1, prefix1, idPrefix2, prefix2);
					}
					else
					{
						parameters.AddWhere(prefix1 + '.' + propertyName, false, "=", prefix2 + '.' + propertyName, false);
					}
				}
			}
		}

		public void Add(PropertyData propertyData)
		{
			_delegate.Add(propertyData);
		}

		public ICompositeMapperBuilder AddComponent(PropertyData propertyData, string componentClassName)
		{
			return _delegate.AddComponent(propertyData, componentClassName);
		}

		public void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper)
		{
			_delegate.AddComposite(propertyData, propertyMapper);
		}

		public IDictionary<PropertyData, IPropertyMapper> Properties
		{
			get { return _delegate.Properties; }
		}
	}
}