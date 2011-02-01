using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public static class ExpressionExtension
	{
		//todo - doesn't currently work with fields
		public static MemberInfo MethodInfo(this Expression expression, string errText)
		{
			var memberEx = expression as MemberExpression;
			
			if (memberEx == null)
			{
				throw new ArgumentException("Only properties or fields can be used for " + errText);
			}
			return memberEx.Member;
		}
	}
}