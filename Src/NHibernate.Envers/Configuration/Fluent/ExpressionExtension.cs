using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public static class ExpressionExtension
	{
		public static MemberInfo MethodInfo<TBody>(this Expression<TBody> expression)
		{
			var realType = expression.Parameters[0].Type;
			var member = expression.Body.methodInfo();
			return realType.GetMember(member.Name)[0];
		}

		private static MemberInfo methodInfo(this Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.MemberAccess:
					var memberExpression = (MemberExpression)expression;
					return memberExpression.Member;
				case ExpressionType.Convert:
					var unaryExpression = (UnaryExpression)expression;
					return methodInfo(unaryExpression.Operand);
				default:
					throw new FluentException("Cannot resolve property or field " + expression);
			}
		}
	}
}