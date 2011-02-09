using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Tools
{
	public static class AssertExtensions
	{
		/// <summary>
		/// Asserts less then with some marginal due to non precise db value
		/// </summary>
		public static void LessOrEqualTo(this DateTime org, DateTime larger)
		{
			var compareWith = larger.AddMilliseconds(5);
			Assert.LessOrEqual(org, compareWith);
		}

		/// <summary>
		/// Asserts less then with some marginal due to non precise db value
		/// </summary>
		public static void GreaterThan(this DateTime org, DateTime smaller)
		{
			var compareWith = smaller.AddMilliseconds(-5);
			Assert.Greater(org, compareWith);
		}


		public static void OnlyContains<T>(this IEnumerable<Attribute> attributes) where T : Attribute
		{
			attributes.Select(x => x.GetType()).Should().Have.SameSequenceAs(new[] { typeof(T) });
		}
	}
}