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
			var entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly != null)
			{
				var logRepository = LogManager.GetRepository(entryAssembly);
				XmlConfigurator.Configure(logRepository, log4netConf);
			}
		}
	}
}