using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public sealed class VersionsJoinTableRangeComponent<T> where T : VersionsJoinTableRangeTestEntityBaseClass
	{
		public VersionsJoinTableRangeComponent()
		{
			Range = new List<T>();
		}

		//[AuditJoinTable(Name = "JOIN_TABLE_COMPONENT_1_AUD", InverseJoinColumns = new[] { "VJTRTE_ID" })]
		public IList<T> Range { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as VersionsJoinTableRangeComponent<T>;
			return casted != null && Range.SequenceEqual(casted.Range);
		}

		public override int GetHashCode()
		{
			var ret = 133;
			foreach (var item in Range)
			{
				ret = ret ^ item.GetHashCode();
			}
			return ret;
		}
	}
}