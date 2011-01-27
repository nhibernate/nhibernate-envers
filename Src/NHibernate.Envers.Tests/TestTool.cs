using System;
using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	public static class TestTool
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
	}
}