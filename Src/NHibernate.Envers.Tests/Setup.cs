using System;
using System.IO;
using log4net.Config;
using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	[SetUpFixture]
	public class Setup
	{
		[SetUp]
		public void RunOnce()
		{
			var log4netConf = new FileInfo(Environment.CurrentDirectory + @"\log4net.xml");
			XmlConfigurator.Configure(log4netConf);
		}
	}
}