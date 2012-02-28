using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Event;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Memory
{
	[Explicit("Mem leak test")]
	public class MultipleConfigurationsTest
	{

		private const int LoopCount = 100;
		private string TestAssembly { get; set; }

		[SetUp]
		public void BaseSetup()
		{
			TestAssembly = GetType().Assembly.GetName().Name;
		}

		[Test]
		public void CreateConfigurationsWithoutEnvers()
		{
			// Does not have any memory leaks (minimal)
			LoopConfigurations(false);
		}

		[Test]
		public void CreateConfigurationsWithEnvers()
		{
			// Has a visual effect in memory usage
			LoopConfigurations(true);
		}

		private void LoopConfigurations(bool useEnvers)
		{
			for (int i = 0; i < LoopCount; i++)
			{
				//Console.WriteLine(i);
				var cfg = CreateConfig();
				if (useEnvers)
					cfg.IntegrateWithEnvers(new AuditEventListener(), AttributeConfiguration());
				using (cfg.BuildSessionFactory())
				{
					// Do nothing, automatically invokes Dispose
					AuditConfiguration.Remove(cfg);
				}

				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}
			Console.WriteLine("Mem: " + GC.GetTotalMemory(true) + " Envers in use: " + useEnvers);
		}

		private Cfg.Configuration CreateConfig()
		{
			var cfg = new Cfg.Configuration();
			cfg.Configure();
			addMappings(cfg);
			return cfg;
		}

		private AttributeConfiguration AttributeConfiguration()
		{
			return new AttributeConfiguration();
		}

		private string nameSpaceAssemblyExtracted()
		{
			var fullNamespace = GetType().Namespace;
			return fullNamespace.Remove(0, TestAssembly.Length + 1);
		}

		private IEnumerable<string> Mappings
		{
			get
			{
				return new[]
						  {
								nameSpaceAssemblyExtracted() + ".Mapping.hbm.xml",
								nameSpaceAssemblyExtracted() + ".Extended.hbm.xml"
						  };
			}
		}

		private void addMappings(Cfg.Configuration cfg)
		{
			var ass = Assembly.Load(TestAssembly);
			foreach (var mapping in Mappings)
			{
				cfg.AddResource(TestAssembly + "." + mapping, ass);
			}
		}

	}
}