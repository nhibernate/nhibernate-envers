using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Persister.Collection;
using NHibernate.Properties;
using NHibernate.UserTypes;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.CustomLists
{
	class CustomListTests
	{
		[Test]
		public void TestCustomLists()
		{
			var cfg = new Cfg.Configuration();
			cfg.Configure();
			cfg.AddResource("NHibernate.Envers.Tests.NetSpecific.UnitTests.CustomLists.Mapping.hbm.xml", GetType().Assembly);

			var ecfg = new FluentConfiguration();
			ecfg.Audit<AuditParent>();
			ecfg.Audit<AuditChild>();

			// Throws exceptions without custon list hooks
			cfg.IntegrateWithEnvers(ecfg);
		}
	}
}
