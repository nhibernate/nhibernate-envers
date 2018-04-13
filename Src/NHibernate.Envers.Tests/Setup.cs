using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	[SetUpFixture]
	public class Setup
	{
		[OneTimeSetUp]
		public void RunOnce()
		{
			var log4netConf = new FileInfo(Environment.CurrentDirectory + @"\log4net.xml");
			var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
			XmlConfigurator.Configure(logRepository, log4netConf);
		}
	}
}