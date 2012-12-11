using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Event;

namespace NHibernate.Envers.Tests.Integration.EntityInstantiation
{
	public class PostInstantiationListener : IPostInstantiationListener
	{
		public void PostInstantiate(object entity)
		{
			var testEnt = (TestEntityWithContext)entity;
			testEnt.Context = new TestExternalContext();
		}
	}
}
