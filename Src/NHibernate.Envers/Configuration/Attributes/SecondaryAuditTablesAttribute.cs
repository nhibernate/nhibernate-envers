using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	//todo - rk, remove this one?
	class SecondaryAuditTablesAttribute:Attribute
	{
		public JoinAuditTableAttribute[] Value { get; set; }
	}
}
