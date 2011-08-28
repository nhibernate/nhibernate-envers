using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public static class ExpressionExtension
	{
		public static MemberInfo MethodInfo<TBody>(this Expression<TBody> expression, string errText)
		{
			var realType = expression.Parameters[0].Type;
			var member = expression.Body.methodInfo(errText);
			return realType.GetMember(member.Name)[0];
		}

		private static MemberInfo methodInfo(this Expression expression, string errText)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.MemberAccess:
					var memberExpression = (MemberExpression)expression;
					return memberExpression.Member;
				case ExpressionType.Convert:
					var unaryExpression = (UnaryExpression)expression;
					return methodInfo(unaryExpression.Operand, errText);
				default:
					throw new ArgumentException("Cannot find property or field for " + errText);
			}
		}
	}
}