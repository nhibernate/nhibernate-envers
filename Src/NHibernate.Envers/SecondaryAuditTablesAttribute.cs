using System;

namespace NHibernate.Envers
{
	//todo - rk, remove this one?
	class SecondaryAuditTablesAttribute:Attribute
	{
		public JoinAuditTableAttribute[] Value { get; set; }
	}
}
