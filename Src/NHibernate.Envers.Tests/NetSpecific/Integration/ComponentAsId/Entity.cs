using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ComponentAsId
{
	public interface IComponentAsIdFlag
	{

	}
	public class Entity<TId> : IComponentAsIdFlag
	{
		public virtual TId Id { get; set; }
	}

	[Serializable]
	public class UdfId<TId1, TId2> : IComponentAsIdFlag
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

	public class UDFDef : Entity<int>
	{
		public virtual string SomeCol0 { get; set; }
	}

	public class UDFDefNhibMap : ClassMapping<UDFDef>, IComponentAsIdFlag
	{
		public UDFDefNhibMap()
		{
			Id(x => x.Id);
		}
	}

	public class EntityUDF<TTypeToExtend, TId>
		: Entity<UdfId<UDFDef, TTypeToExtend>>
	{
		public virtual UDFDef Def { get => Id.UDFDef; set => Id.UDFDef = value; }
		public virtual TTypeToExtend Owner { get => Id.UDFOwnr; set => Id.UDFOwnr = value; }
		public virtual string SomeCol1 { get; set; }
	}

	public class EntityUDFNhibMap<TUDF, TTypeToExtend, TId> : ClassMapping<TUDF>, IComponentAsIdFlag
		where TUDF : EntityUDF<TTypeToExtend, TId>
	{
		protected EntityUDFNhibMap()
		{
			this.ComponentAsId(x => x.Id);

			this.NotMapped(x => x.Owner);
			this.NotMapped(x => x.Def);
		}
	}

	public class SomeEntity : Entity<int>
	{
		public virtual string SomeCol2 { get; set; }
	}

	public class SomeEntNhimMap : ClassMapping<SomeEntity>, IComponentAsIdFlag
	{
		public SomeEntNhimMap()
		{
			Id(x => x.Id);
		}
	}

	/// <summary>
	/// Look at audit table
	/// </summary>
	public class SomeEntUDF : EntityUDF<SomeEntity, int> { }

	public class SomeEntUdfMap : EntityUDFNhibMap<SomeEntUDF, SomeEntity, int> { }
}
