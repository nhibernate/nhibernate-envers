using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Envers.Entities
{
	[Serializable]
	public class RevisionTypeType: IUserType 
	{
		private static readonly SqlType[] SQL_TYPES = { new SqlType(DbType.Byte) };

		public SqlType[] SqlTypes 
		{ 
			get 
			{
				return SQL_TYPES;
			}
		}

		public System.Type ReturnedType 
		{
			get
			{
				return typeof(RevisionType);
			}
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			var enumAsInt = Convert.ToByte(NHibernateUtil.Int16.NullSafeGet(rs, names));
			return Enum.ToObject(typeof (RevisionType), enumAsInt);
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			if (null == value) 
			{
				cmd.Parameters[index] = null;
			} 
			else 
			{
				((IDbDataParameter)cmd.Parameters[index]).Value = value;
			}
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public bool IsMutable
		{
			get { return false; }
		}

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
