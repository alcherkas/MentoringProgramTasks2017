using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTransformer
{
    public class KnownParametersTransformer : ExpressionVisitor
    {
        private readonly IReadOnlyDictionary<string, object> _knownParameters;

        public KnownParametersTransformer(IReadOnlyDictionary<string, object> knownParameters) => _knownParameters = knownParameters;

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node == null) return base.VisitBinary(node);

            var leftParameter = ChangeParameterToConstant(node.Left);
            var rightParameter = ChangeParameterToConstant(node.Right);

            return node.Update(leftParameter, node.Conversion, rightParameter);
        }

        private Expression ChangeParameterToConstant(Expression node)
        {
            var nodeName = node.NodeType == ExpressionType.Convert ? ((UnaryExpression)node).Operand.ToString() : node.ToString();
            var isKnownParameter = _knownParameters.ContainsKey(nodeName);
            if (isKnownParameter)
            {
                var constantValue = _knownParameters[nodeName];
                if (node.NodeType == ExpressionType.Convert)
                {
                    var convertExpression = (UnaryExpression)node;
                    var updatedExpression =  Expression.Convert(Expression.Constant(constantValue), convertExpression.Type);
                    return updatedExpression;
                }

                return Expression.Constant(constantValue);
            }

            var binaryExpression = node as BinaryExpression;
            return binaryExpression == null ? node : VisitBinary(binaryExpression);
        }
    }

    [TestClass]
    public class KnownParametersTransformerTest
    {
        [TestMethod]
        public void ShouldReplaceKnowConstants()
        {
            var knownConstants = new Dictionary<string, object>
            {
                { "a", 10 },
                { "b", 7.0 },
                { "c", 13.0 }
            };

            Expression<Func<int, double, double, double, double>> sourceExpression =
                (a, b, c, d) => a * b / c - d + a;

            var resultExpression = new KnownParametersTransformer(knownConstants).VisitAndConvert(sourceExpression, string.Empty);
            LambdaExpression pretifiedResult = PretifyLambda(knownConstants, resultExpression);

            Console.WriteLine(sourceExpression);
            Console.WriteLine(resultExpression);
            Console.WriteLine(pretifiedResult);

            var sourceResult = sourceExpression.Compile().Invoke(10, 7, 13, 12);
            var invokeResult = (double)pretifiedResult.Compile().DynamicInvoke(12.0);
            Assert.AreEqual(sourceResult, invokeResult);
        }


        private static LambdaExpression PretifyLambda(Dictionary<string, object> knownConstants, Expression<Func<int, double, double, double, double>> resultExpression)
        {
            var usedParameters = new List<ParameterExpression>();
            foreach (var parameter in resultExpression.Parameters)
                if (!knownConstants.ContainsKey(parameter.Name)) usedParameters.Add(parameter);

            return Expression.Lambda(resultExpression.Body, usedParameters);
        }
    }
}
