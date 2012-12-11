using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tests.Integration.EntityInstantiation
{
	public interface IExternalContext
	{
		string ContextName { get; }
	}
}
