using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class AuditFactoryAttribute : Attribute
	{
		private readonly System.Type _factoryType;
		private IEntityFactory _factory;

		public AuditFactoryAttribute()
		{
		}

		public AuditFactoryAttribute(System.Type factoryType)
		{
			_factoryType = factoryType;
		}

		public AuditFactoryAttribute(IEntityFactory instance)
		{
			_factory = instance;
		}

		public IEntityFactory Factory
		{
			get
			{
				if (_factory == null && _factoryType != null)
				{
					initFactoryFromFactoryType();
				}
				return _factory;
			}
			set
			{
				_factory = value;
			}
		}

		private void initFactoryFromFactoryType()
		{
			try
			{
				_factory = (IEntityFactory)Activator.CreateInstance(_factoryType);
			}
			catch (MissingMethodException)
			{
				throw new MappingException(string.Format("Factory must be of type IEntityFactory (but is: {0})", _factoryType));
			}
		}
	}
}