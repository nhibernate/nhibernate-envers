using System.Data;
using System.Data.Common;
using NHibernate.Engine;
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
			if (x == null || y == null)
			{
				return false;
			}
			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			return x == null ? 0 : x.GetHashCode();
		}

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			var dbString = NHibernateUtil.String.NullSafeGet(rs, names[0], session);
			return dbString.Equals("Yes") ? CustomEnum.Yes : CustomEnum.No;
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var dbString = (CustomEnum)value == CustomEnum.Yes ? "Yes" : "No";
			NHibernateUtil.String.NullSafeSet(cmd, dbString, index, session);
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

		public SqlType[] SqlTypes => TYPES;

		public System.Type ReturnedType => typeof(CustomEnum);

		public bool IsMutable => false;
	}
}