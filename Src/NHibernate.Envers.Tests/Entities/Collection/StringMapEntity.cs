using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Collection
{
    public class StringMapEntity
    {
    	public StringMapEntity()
    	{
    		Strings = new Dictionary<string, string>();
    	}

        public virtual int Id { get; set; }
        [Audited]
		public virtual IDictionary<string, string> Strings { get; set; }
    }
}