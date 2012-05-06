using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Envers.Tests.Entities.CustomType
{
	public class CLOBTestUserType : IUserType
	{
		private static readonly SqlType[] TYPES = new[] { new StringClobSqlType() };

		public new bool Equals(object x, object y)
		{
			//noinspection ObjectEquality
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

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			return NHibernateUtil.String.NullSafeGet(rs, names[0]);
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			NHibernateUtil.String.NullSafeSet(cmd, value, index);
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		public SqlType[] SqlTypes
		{
			get { return TYPES; }
		}

		public System.Type ReturnedType
		{
			get { return typeof(string); }
		}

		public bool IsMutable
		{
			get { return false; }
		}

	}
}