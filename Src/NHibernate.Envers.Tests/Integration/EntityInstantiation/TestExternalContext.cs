using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tests.Integration.EntityInstantiation
{
	public class TestExternalContext : IExternalContext
	{
		public string ContextName { get { return "Test Context"; } }
	}
}
