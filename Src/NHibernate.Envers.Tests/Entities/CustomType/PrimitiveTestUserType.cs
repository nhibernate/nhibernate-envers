using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Envers.Tests.Entities.CustomType
{
	public class PrimitiveTestUserType : PrimitiveType
	{
		public PrimitiveTestUserType() : base(new SqlTypes.SqlType(DbType.Int32)) { }

		public override object DefaultValue => 0;

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}

		public override System.Type PrimitiveClass => typeof(int);

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var parameter = (IDataParameter)cmd.Parameters[index];
			var val = (PrimitiveImmutableType)value;
			parameter.Value = val.IntValue;
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			var o = rs[index];
			var value = Convert.ToInt32(o);
			return PrimitiveImmutableType.Get(value);
		}

		public override string Name => "PrimitiveImmutableType";

		public override System.Type ReturnedClass => typeof(PrimitiveImmutableType);
	}
}
