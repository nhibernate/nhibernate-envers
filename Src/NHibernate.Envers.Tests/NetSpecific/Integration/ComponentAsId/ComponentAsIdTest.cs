using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Envers.Configuration;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ComponentAsId
{
	public class ComponentAsIdTest : TestBase
	{
		public ComponentAsIdTest(AuditStrategyForTest strategyType) : base(strategyType)
		{

		}
		[Test]
		public void ComponentAsIdTestMethod()
		{
			Assert.DoesNotThrow(() =>
			{
				var udfDef = new UDFDef
				{
					Id = 1,
					SomeCol0 = "EEE"
				};

				Save(udfDef);

				var someEnt = new SomeEntity()
				{
					Id = 1,
					SomeCol2 = "RRR"
				};

				Save(someEnt);

				var udf = new SomeEntUDF
				{
					Id = new UdfId<UDFDef, SomeEntity>
					{
						UDFDef = udfDef,
						UDFOwnr = someEnt
					},
					SomeCol1 = "TTT"
				};

				Save(udf);

				Del(udf);
				Del(udfDef);
			});
		}

		void Save(object o)
		{
			using (var tran = Session.BeginTransaction())
			{
				Session.Save(o);
				tran.Commit();
			}
		}

		void Del(object o)
		{
			using (var tran = Session.BeginTransaction())
			{
				Session.Delete(o);
				tran.Commit();
			}
		}

		protected override void Initialize()
		{
			
		}
	}
}
