using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sample03
{
	public class ExpressionToFTSRequestTranslator : ExpressionVisitor
	{
		StringBuilder resultString;

		public string Translate(Expression exp)
		{
			resultString = new StringBuilder();
			Visit(exp);

			return resultString.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.DeclaringType == typeof(Queryable)
				&& node.Method.Name == "Where")
			{
				var predicate = node.Arguments[1];
                var isMethodCallInQuery = ((predicate as UnaryExpression)?.Operand as LambdaExpression)?.Body.NodeType == ExpressionType.Call;
                if (!isMethodCallInQuery) { Visit(predicate); return node; }
            }
            else if(node.Method.DeclaringType == typeof(string))
            {
                var propertyName = (node.Object as MemberExpression).Member.Name;
                resultString.Append(propertyName).Append(":").Append("(");
                switch (node.Method.Name)
                {
                    case "StartsWith":
                        resultString.Append(GetFirstArgumentValue(node)).Append("*");
                        break;
                    case "EndsWith":
                        resultString.Append("*").Append(GetFirstArgumentValue(node));
                        break;
                    case "Contains":
                        resultString.Append("*").Append(GetFirstArgumentValue(node)).Append("*");
                        break;
                }

                resultString.Append(")");
                return node;
            }
            return base.VisitMethodCall(node);
		}

        private static string GetFirstArgumentValue(MethodCallExpression node)
        {
            return ((ConstantExpression)node.Arguments.First()).Value.ToString();
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    MemberExpression argument = null;
                    ConstantExpression constant = null;
                    if (IsValidArgumentSearch(node.Left.NodeType, node.Right.NodeType))
                    {
                        argument = (MemberExpression)node.Left;
                        constant = (ConstantExpression)node.Right;
                    }
                    else if (IsValidArgumentSearch(node.Right.NodeType, node.Left.NodeType))
                    {
                        argument = (MemberExpression)node.Right;
                        constant = (ConstantExpression)node.Left;
                    }
                    else throw new NotSupportedException("One of the operands should be constant and second should be a property");

                    // Visit(argument);
                    resultString.Append(argument.Member.Name);
                    resultString.Append(":");
                    resultString.Append("(");
                    resultString.Append(constant.Value.ToString());
                    // Visit(constant);
                    resultString.Append(")");
                    break;
                default:
                    throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
            };

            return node;
        }

        private static bool IsValidArgumentSearch(ExpressionType argumentType, ExpressionType constantType)
        => argumentType == ExpressionType.MemberAccess && constantType == ExpressionType.Constant;
	}
}
