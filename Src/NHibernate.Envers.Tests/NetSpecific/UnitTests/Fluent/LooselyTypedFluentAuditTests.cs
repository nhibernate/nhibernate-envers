using System;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Fluent;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class LooselyTypedFluentAuditTests
	{
		private class MyClass
		{

		}
		[Test]
		public void WhenCreateWithNullThenThrows()
		{
			Executing.This(() => new LooselyTypedFluentAudit(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenTypeIsValueTypeThenThrows()
		{
			Executing.This(() => new LooselyTypedFluentAudit(typeof(int))).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void WhenCreatedThenHasType()
		{
			var expected = typeof(MyClass);
			var attributeProvider = new LooselyTypedFluentAudit(expected);
			attributeProvider.Type.Should().Be(expected);
		}

		[Test]
		public void WhenGetClassAttributeThenSingleAuditedAttribute()
		{
			var attributeProvider = new LooselyTypedFluentAudit(typeof(MyClass));
			attributeProvider.CreateClassAttributes().Single().Should().Be.InstanceOf<AuditedAttribute>();
		}

		[Test]
		public void WhenGetMemberAttributesThenEmpty()
		{
			var attributeProvider = new LooselyTypedFluentAudit(typeof(MyClass));
			attributeProvider.CreateMemberAttributes().Should().Be.Empty();
		}
	}
}