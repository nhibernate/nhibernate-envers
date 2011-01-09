using System;
using System.Collections.Generic;
using NHibernate.Type;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.Engine;
using System.Collections;

namespace NHibernate.Envers.Compatibility
{
    /**
     * <tt>long</tt>: A type that maps an SQL BIGINT to a C# long.
     * @author Gavin King
     */
    public class LongType : PrimitiveType, IDiscriminatorType, IVersionType {

	    private static readonly long ZERO = 0;

		/// <summary></summary>
		public LongType() : base(SqlTypeFactory.UInt32)
		{
		}

		public override string Name
		{
			get { return "int"; }
		}

		public override object Get(IDataReader rs, int index)
		{
			try
			{
				return Convert.ToUInt32(rs[index]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override object Get(IDataReader rs, string name)
		{
			try
			{
				return Convert.ToUInt32(rs[name]);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[name]), ex);
			}
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(UInt32); }
		}

		public override void Set(IDbCommand rs, object value, int index)
		{
			((IDataParameter)rs.Parameters[index]).Value = value;
		}

		public object StringToObject(string xml)
		{
			return FromStringValue(xml);
		}

		public override object FromStringValue(string xml)
		{
			return UInt32.Parse(xml);
		}

		#region IVersionType Members

		public virtual object Next(object current, ISessionImplementor session)
		{
			return (UInt32)current + 1;
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return 1;
		}

		public IComparer Comparator
		{
			get { return Comparer<UInt32>.Default; }
		}

		#endregion

		public override System.Type PrimitiveClass
		{
			get { return typeof(UInt32); }
		}

		public override object DefaultValue
		{
			get { return LongType.ZERO; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}
	}
}
