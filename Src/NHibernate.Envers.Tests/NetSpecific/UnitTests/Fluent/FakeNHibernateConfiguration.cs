using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	public static class FakeNHibernateConfiguration
	{
		public static Cfg.Configuration For<T>() where T : class
		{
			var nhConf = new Cfg.Configuration();
			nhConf.DataBaseIntegration(db => db.Dialect<MsSql2005Dialect>());

			var bahs = new ModelMapper();
			bahs.Class<T>(c => c.Id("Id", idMapper => idMapper.Generator(Generators.Assigned)));
			nhConf.AddMapping(bahs.CompileMappingForAllExplicitlyAddedEntities());

			return nhConf;
		}
	}
}