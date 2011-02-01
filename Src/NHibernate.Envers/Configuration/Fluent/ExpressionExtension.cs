using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public static class ExpressionExtension
	{
		public static MemberInfo MethodInfo(this Expression expression, string errText)
		{
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:                      
                    var memberExpression = (MemberExpression)expression; 
                    return memberExpression.Member; 
                case ExpressionType.Convert:                      
                    var unaryExpression = (UnaryExpression)expression;
                    return MethodInfo(unaryExpression.Operand, errText); 
                default:
                    throw new ArgumentException("Cannot find property or field for " + errText);
            }  
		}
	}
}