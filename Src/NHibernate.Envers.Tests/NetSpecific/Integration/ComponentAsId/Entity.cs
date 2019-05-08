using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ComponentAsId
{
	public class Entity<TId>
	{
		public virtual TId Id { get; set; }
	}

	[Serializable]
	public class UdfId<TId1, TId2>
	{
		public UdfId()
		{
			UDFDef = (TId1)Activator.CreateInstance(typeof(TId1));
			UDFOwnr = (TId2)Activator.CreateInstance(typeof(TId2));
		}

		public TId2 UDFOwnr { get; set; }
		public TId1 UDFDef { get; set; }

		public override bool Equals(object obj)
		{
			if (this is null && obj is null)
				return true;

			if (this is null || obj is null)
				return false;

			if (!(obj is UdfId<TId1, TId2>))
			{
				return (false);
			}

			if (Object.ReferenceEquals(this, obj) == true)
			{
				return (true);
			}

			UdfId<TId1, TId2> other = obj as UdfId<TId1, TId2>;

			var defEq = false;
			if (UDFDef == null && other.UDFDef == null)
				defEq = true;

			if ((UDFDef == null || other.UDFDef == null) && !defEq)
				return false;

			if (!defEq && UDFDef.Equals(other.UDFDef))
				defEq = true;



			var defOw = false;
			if (UDFOwnr == null && other.UDFOwnr == null)
				defOw = true;

			if ((UDFOwnr == null || other.UDFOwnr == null) && !defOw)
				return false;

			if (!defOw && UDFOwnr.Equals(other.UDFOwnr))
				defOw = true;

			return defEq && defOw;
		}

		public override int GetHashCode()
		{
			return 1;
		}

		public static bool operator ==(UdfId<TId1, TId2> first, UdfId<TId1, TId2> second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(UdfId<TId1, TId2> first, UdfId<TId1, TId2> second)
		{
			return !first.Equals(second);
		}
	}

	[Audited]
	public class UDFDef : Entity<int>
	{
		public virtual string SomeCol0 { get; set; }
	}

	public abstract class EntityUDF<TTypeToExtend, TId>
		: Entity<UdfId<UDFDef, TTypeToExtend>>
	{
		public virtual UDFDef Def { get => Id.UDFDef; set => Id.UDFDef = value; }
		public virtual TTypeToExtend Owner { get => Id.UDFOwnr; set => Id.UDFOwnr = value; }
		public virtual string SomeCol1 { get; set; }
	}

	[Audited]
	public class SomeEntity : Entity<int>
	{
		public virtual string SomeCol2 { get; set; }
	}

	[Audited]
	public class SomeEntUDF : EntityUDF<SomeEntity, int> { }

}
