using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Attributes
{
	public class AttributeConfiguration : IMetaDataProvider
	{
		public IDictionary<System.Type, IEntityMeta> CreateMetaData(Cfg.Configuration nhConfiguration)
		{
			var ret = new Dictionary<System.Type, IEntityMeta>();
			foreach (var persistentClass in nhConfiguration.ClassMappings)
			{
				addForEntity(persistentClass, ret);
				addForComponent(persistentClass.PropertyIterator, ret);
			}
			return ret;
		}

		private static void addForComponent(IEnumerable<Property> propertyIterator, IDictionary<System.Type, IEntityMeta> dicToFill)
		{
			foreach (var property in propertyIterator)
			{
				var propAsComponent = property.Value as Component;
				if (propAsComponent == null) continue;
				fillType(propAsComponent.ComponentClass, dicToFill);
				fillMembers(propAsComponent.ComponentClass, propAsComponent.PropertyIterator, dicToFill);
			}
		}

		private static void addForEntity(PersistentClass persistentClass, IDictionary<System.Type, IEntityMeta> dicToFill)
		{
			var typ = persistentClass.MappedClass;
			fillType(typ, dicToFill);
			var props = new List<Property>();
			props.AddRange(persistentClass.PropertyIterator);
			if (persistentClass.IdentifierProperty != null && !persistentClass.IdentifierProperty.IsComposite)
				props.Add(persistentClass.IdentifierProperty);

			fillMembers(typ, props, dicToFill);
		}

		private static void fillMembers(System.Type type, IEnumerable<Property> properties, IDictionary<System.Type, IEntityMeta> dicToFill)
		{
			foreach (var propInfo in PropertyAndMemberInfo.PersistentInfo(type, properties))
			{
				foreach (var attr in Attribute.GetCustomAttributes(propInfo.Member))
				{
					if (!dicToFill.ContainsKey(type))
						dicToFill[type] = new EntityMeta();
					((EntityMeta)dicToFill[type]).AddMemberMeta(propInfo.Member, attr);
				}
			}
		}

		private static void fillType(System.Type typ, IDictionary<System.Type, IEntityMeta> dicToFill)
		{
			foreach (Attribute attr in typ.GetCustomAttributes(false))
			{
				if (!dicToFill.ContainsKey(typ))
					dicToFill[typ] = new EntityMeta();
				((EntityMeta)dicToFill[typ]).AddClassMeta(attr);
			}
		}
	}
}