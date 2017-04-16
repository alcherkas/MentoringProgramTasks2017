using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionTransformer
{
    public class IncDecTransformer : ExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if ((node.NodeType == ExpressionType.Add || node.NodeType == ExpressionType.Subtract)
                && node.Right.NodeType == ExpressionType.Constant
                && ((int)((ConstantExpression)node.Right).Value) == 1)
            {
                return node.NodeType == ExpressionType.Add ? Expression.Increment(node.Left) : Expression.Decrement(node.Left);
            }

            return base.VisitBinary(node);
        }
    }

    [TestClass]
    public class IncDecTransformerTest
    {
        [TestMethod]
        public void IncDecTransformerTestMethod()
        {
            var intValue = 10;
            Expression<Func<int>> source = () => intValue - 1;
            var convertedExpression = new IncDecTransformer().VisitAndConvert(source, string.Empty);
            var result = convertedExpression.Compile().Invoke();
            var sourceResult = source.Compile().Invoke();
            Assert.AreEqual(result, sourceResult);
        }
    }
}
