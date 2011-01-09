using System;
using System.IO;
using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	[SetUpFixture]
	public class SetupNunit
	{
		[SetUp]
		public void RunOnce()
		{
			var log4netConf = new FileInfo(Environment.CurrentDirectory + @"\log4net.xml");
			log4net.Config.XmlConfigurator.Configure(log4netConf);
		}
		
	}
}