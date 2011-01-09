using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.Envers.Tests.Entities.CustomType
{
	[Serializable]
	public class CompositeTestUserType : ICompositeUserType
	{
		public object GetPropertyValue(object component, int property)
		{
			var comp = (Component)component;
			if (property == 0)
			{
				return comp.Prop1;
			}
			return comp.Prop2;
		}

		public void SetPropertyValue(object component, int property, object value)
		{
			var comp = (Component)component;
			if (property == 0)
			{
				comp.Prop1 = (string) value;
			}
			else
			{
				comp.Prop2 = (int) value;
			}
		}

		public new bool Equals(object x, object y)
		{
			if (x == y)
			{
				return true;
			}

			if (x == null || y == null)
			{
				return false;
			}

			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
		{
			var prop1 = dr.GetString(dr.GetOrdinal(names[0]));
			if (prop1 == null) {
				return null;
			}
			var prop2 = dr.GetInt32(dr.GetOrdinal(names[1]));

			return new Component {Prop1 = prop1, Prop2 = prop2};
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
		{
			if(value==null)
			{
				((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
				((IDataParameter)cmd.Parameters[index+1]).Value = DBNull.Value;
			}
			else
			{
				var cmp = (Component) value;
				((IDataParameter)cmd.Parameters[index]).Value = cmp.Prop1;
				((IDataParameter)cmd.Parameters[index + 1]).Value = cmp.Prop2;
			}
		}

		public object DeepCopy(object value)
		{
			var comp = (Component)value;
			return new Component {Prop1 = comp.Prop1, Prop2 = comp.Prop2};
		}

		public object Disassemble(object value, ISessionImplementor session)
		{
			return value;
		}

		public object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return cached;
		}

		public object Replace(object original, object target, ISessionImplementor session, object owner)
		{
			return original;
		}

		public string[] PropertyNames
		{
			get { return new[] { "Prop1", "Prop2" }; }
		}

		public IType[] PropertyTypes
		{
			get { return new IType[] { NHibernateUtil.String, NHibernateUtil.Int32}; }
		}

		public System.Type ReturnedClass
		{
			get {return typeof(Component); }
		}

		public bool IsMutable
		{
			get { return true; }
		}
	}
}