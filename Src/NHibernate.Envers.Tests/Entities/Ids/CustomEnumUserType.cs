using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Envers.Tests.Entities.Ids
{
	public class CustomEnumUserType : IUserType
	{
		private static readonly SqlType[] TYPES = { new SqlType(DbType.String) };

		public new bool Equals(object x, object y)
		{
			if (x == y)
			{
				return true;
			}
			if ((x == null) || (y == null))
			{
				return false;
			}
			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			return (x == null) ? 0 : x.GetHashCode();
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			var dbString = NHibernateUtil.String.NullSafeGet(rs, names[0]);
			return dbString.Equals("Yes") ? CustomEnum.Yes : CustomEnum.No;
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			var dbString = ((CustomEnum)value) == CustomEnum.Yes ? "Yes" : "No";
			NHibernateUtil.String.NullSafeSet(cmd, dbString, index);
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
			get { return typeof(CustomEnum); }
		}

		public bool IsMutable
		{
			get { return false; }
		}
	}
}