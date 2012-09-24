using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Iesi.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Tests.Entities.ManyToMany;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration.Serialize
{
	public class SerializableTest : TestBase
	{
		private const int ingId = 567;
		private SetOwnedEntity ed;

		public SerializableTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ed = new SetOwnedEntity { Id = ingId+1, Data = "data_ed_1" };
			var ing1 = new SetOwningEntity {Id = ingId, Data = "data_ing_1", References = new HashedSet<SetOwnedEntity> {ed}};

			var serializedCfg = serializeCfg(); //serialize before use
			using (var sFactory = serializedCfg.BuildSessionFactory())
			{
				using (var s = sFactory.OpenSession())
				{
					using (var tx = s.BeginTransaction())
					{
						Session.Save(ed);
						Session.Save(ing1);
						tx.Commit();
					}		
				}
			}
		}

		[Test]
		public void CanSerializeAndDeserializeCfg()
		{
			var serializedCfg = serializeCfg(); //serialize after use
			using (var sFactory = serializedCfg.BuildSessionFactory())
			{
				using (var s = sFactory.OpenSession())
				{
					var auditReader = AuditReaderFactory.Get(s);
					auditReader.Find<SetOwningEntity>(ingId, 1).References.Should().Have.SameSequenceAs(ed);
				}
			}
		}

		private Cfg.Configuration serializeCfg()
		{
			Cfg.Configuration serializedCfg;
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, Cfg);
				stream.Position = 0;
				serializedCfg = (Cfg.Configuration) formatter.Deserialize(stream);
			}
			serializedCfg.Properties[Environment.Hbm2ddlAuto] = string.Empty;
			return serializedCfg;
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.ManyToMany.Mapping.hbm.xml" };
			}
		}
	}
}