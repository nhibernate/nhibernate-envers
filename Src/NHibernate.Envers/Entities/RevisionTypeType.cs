using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Envers.Entities
{
	[Serializable]
	public class RevisionTypeType: IUserType 
	{
		//unnecessary "large" due to https://github.com/nhibernate/nhibernate-envers/issues/30
		private static readonly SqlType[] SQL_TYPES = { new SqlType(DbType.Int16) };

		public SqlType[] SqlTypes => SQL_TYPES;

		public System.Type ReturnedType => typeof(RevisionType);

		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			var enumAsInt = Convert.ToByte(NHibernateUtil.Int16.NullSafeGet(rs, names, session));
			return Enum.ToObject(typeof (RevisionType), enumAsInt);
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			if (null == value) 
			{
				cmd.Parameters[index] = null;
			} 
			else 
			{
				cmd.Parameters[index].Value = Convert.ToByte(value);
			}
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public bool IsMutable => false;

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public new bool Equals(object x, object y)
		{
			if (x == y) 
			{
				return true;
			}

			if (null == x || null == y) 
			{
				return false;
			}

			return x.Equals(y);
		}
	}
}
