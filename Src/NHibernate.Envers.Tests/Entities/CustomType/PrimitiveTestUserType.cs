using System;
using System.Data;
using NHibernate.Type;

namespace NHibernate.Envers.Tests.Entities.CustomType
{
	public class PrimitiveTestUserType : PrimitiveType
	{
		public PrimitiveTestUserType() : base(new SqlTypes.SqlType(DbType.Int32)) { }

		public override object DefaultValue
		{
			get { return 0; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(int); }
		}

		public override object FromStringValue(string xml)
		{
			return int.Parse(xml);
		}

		public override object Get(IDataReader rs, string name)
		{
			var ordinal = rs.GetOrdinal(name);
			return Get(rs, ordinal);
		}

		public override object Get(IDataReader rs, int index)
		{
			var o = rs[index];
			var value = Convert.ToInt32(o);
			return PrimitiveImmutableType.Get(value);
		}

		public override void Set(IDbCommand cmd, object value, int index)
		{
			var parameter = (IDataParameter)cmd.Parameters[index];
			var val = (PrimitiveImmutableType)value;
			parameter.Value = val.IntValue;
		}

		public override string Name
		{
			get { return "PrimitiveImmutableType"; }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(PrimitiveImmutableType); }
		}
	}
}
