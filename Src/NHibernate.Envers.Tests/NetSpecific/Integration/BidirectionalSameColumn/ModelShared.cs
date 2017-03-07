using System.Collections.Generic;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BidirectionalSameColumn
{
	public class ModelShared
	{
		public virtual int Id { get; set; }

		public virtual ISet<ModelConfigurationShared> ModelConfigurations { get; set; }
	}
}