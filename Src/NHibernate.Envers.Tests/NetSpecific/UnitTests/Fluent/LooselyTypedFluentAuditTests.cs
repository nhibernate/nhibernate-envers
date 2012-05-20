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
		public void WhenGetClassAttributeThenSingleAuditedAttribute()
		{
			var attributeProvider = new LooselyTypedFluentAudit(typeof(MyClass));
			attributeProvider.Attributes(null).Single().Attribute.Should().Be.InstanceOf<AuditedAttribute>();
		}
	}
}