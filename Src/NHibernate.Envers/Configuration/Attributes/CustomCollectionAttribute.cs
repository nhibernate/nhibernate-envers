using System;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.UserTypes;

namespace NHibernate.Envers.Configuration.Attributes
{
	/// <summary>
	/// Defines a <see cref="IPropertyMapper"/> for a specific collection.
	/// Mandatory if a NH Core <see cref="IUserCollectionType"/> is used.
	/// This attribute is only applicable on collections.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class CustomCollectionAttribute : Attribute
	{
		public CustomCollectionAttribute(System.Type mapperType)
		{
			CustomCollectionFactory = mapperType;
		}

		/// <summary>
		/// Nees to be a type deriving from <see cref="ICustomCollectionFactory"/>
		/// </summary>
		public System.Type CustomCollectionFactory { get; private set; }
	}
}