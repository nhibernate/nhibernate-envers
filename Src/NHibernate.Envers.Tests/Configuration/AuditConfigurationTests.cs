using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Event;
using NHibernate.Event;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Configuration
{
	[Audited]
	public class SimpleAuiditableForConf
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }
	}
	public class AuditConfigurationTests
	{
		const string SimpleMapping = @"<?xml version='1.0' encoding='utf-8'?>
<hibernate-mapping namespace='NHibernate.Envers.Tests.Configuration' assembly='NHibernate.Envers.Tests' xmlns='urn:nhibernate-mapping-2.2'>
  <class name='SimpleAuiditableForConf'>
    <id name='Id' type='Int32'>
      <generator class='assigned'/>
    </id>
    <property name='Data' />
  </class>
</hibernate-mapping>";

		[Test]
		public void WhenGetAuditConfMultipleTimesThenDoesNotThrowsForDupicatedMappings()
		{
			var cfg = new Cfg.Configuration();
			cfg.Configure();

			cfg.AddXml(SimpleMapping);
			AuditConfiguration.GetFor(cfg); //<< external call

			AuditConfiguration.SetConfigMetas(cfg, new AttributeConfiguration());
			var listeners = new[] { new AuditEventListener() };
			cfg.AppendListeners(ListenerType.PostInsert, listeners);
			cfg.AppendListeners(ListenerType.PostUpdate, listeners);
			cfg.AppendListeners(ListenerType.PostDelete, listeners);
			cfg.AppendListeners(ListenerType.PostCollectionRecreate, listeners);
			cfg.AppendListeners(ListenerType.PreCollectionRemove, listeners);
			cfg.AppendListeners(ListenerType.PreCollectionUpdate, listeners);

			Executing.This(() =>
										 {
											 using (var sf = cfg.BuildSessionFactory()) //<< call by listener initialization
											 {
												 // build the session factory to run initialization of listeners and be completelly sure
												 // there isn't problems
											 }
										 }).Should().NotThrow();
		}
	}
}