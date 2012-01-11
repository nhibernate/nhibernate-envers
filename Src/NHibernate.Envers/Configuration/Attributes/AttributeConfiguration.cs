using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Attributes
{
	/// <summary>
	/// Tells Envers that attribute configuration is used
	/// </summary>
	public class AttributeConfiguration : IMetaDataProvider
	{
		private IList<System.Type> filledTypes; 

		public IDictionary<System.Type, IEntityMeta> CreateMetaData(Cfg.Configuration nhConfiguration)
		{
			filledTypes = new List<System.Type>();
			var ret = new Dictionary<System.Type, IEntityMeta>();
			foreach (var persistentClass in nhConfiguration.ClassMappings)
			{
				addForEntity(persistentClass, ret);
				addForComponent(persistentClass.PropertyIterator, ret);
			}
			return ret;
		}

		private void addForComponent(IEnumerable<Property> propertyIterator, IDictionary<System.Type, IEntityMeta> dicToFill)
		{
			foreach (var property in propertyIterator)
			{
				var propAsComponent = property.Value as Component;
				if (propAsComponent == null || propAsComponent.IsDynamic) continue;
				fillClass(propAsComponent.ComponentClass, dicToFill);
				fillMembers(propAsComponent.ComponentClass, propAsComponent.PropertyIterator, dicToFill);
			}
		}

		private void addForEntity(PersistentClass persistentClass, IDictionary<System.Type, IEntityMeta> dicToFill)
		{
			var typ = persistentClass.MappedClass;
			fillClass(typ, dicToFill);
			var props = new List<Property>();
			props.AddRange(persistentClass.PropertyIterator);
			if (persistentClass.IdentifierProperty != null && !persistentClass.IdentifierProperty.IsComposite)
				props.Add(persistentClass.IdentifierProperty);

			fillMembers(typ, props, dicToFill);
		}

		private void fillMembers(System.Type type, IEnumerable<Property> properties, IDictionary<System.Type, IEntityMeta> dicToFill)
		{
			foreach (var propInfo in PropertyAndMemberInfo.PersistentInfo(type, properties))
			{
				foreach (var attr in Attribute.GetCustomAttributes(propInfo.Member))
				{
					if (!dicToFill.ContainsKey(type))
						dicToFill[type] = new EntityMeta();
					var memberAttributeToAdd = MemberAttribute(attr, type, propInfo);
					((EntityMeta)dicToFill[type]).AddMemberMeta(propInfo.Member, memberAttributeToAdd);
				}
			}
		}

		protected virtual Attribute MemberAttribute(Attribute attribute, System.Type type, DeclaredPersistentProperty persistentProperty)
		{
			return attribute;
		}

		private void fillClass(System.Type type, IDictionary<System.Type, IEntityMeta> dicToFill)
		{
			if (!filledTypes.Contains(type))
			{
				foreach (Attribute attr in type.GetCustomAttributes(false))
				{
					if (!dicToFill.ContainsKey(type))
						dicToFill[type] = new EntityMeta();
					var classAttributeToAdd = ClassAttribute(attr, type);
					((EntityMeta)dicToFill[type]).AddClassMeta(classAttributeToAdd);
				}	
			}
			var baseType = type.BaseType;
			if(!type.IsInterface && baseType != typeof(object))
				fillClass(baseType, dicToFill);
		}

		protected virtual Attribute ClassAttribute(Attribute attribute, System.Type type)
		{
			return attribute;
		}
	}
}