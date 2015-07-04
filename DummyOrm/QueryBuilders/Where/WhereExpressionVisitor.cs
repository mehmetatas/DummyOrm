using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DummyOrm.Meta;
using DummyOrm.QueryBuilders.Where.ExpressionBuilders;
using DummyOrm.QueryBuilders.Where.Expressions;

namespace DummyOrm.QueryBuilders.Where
{
    public class WhereExpressionVisitor : ExpressionVisitor
    {
        private readonly Stack<IWhereExpressionBuilder> _stack = new Stack<IWhereExpressionBuilder>();

        private IWhereExpressionBuilder _current;

        private WhereExpressionVisitor()
        {

        }

        public static IWhereExpression Build(Expression whereExpression)
        {
            var evaled = whereExpression;
            //var evaled = Evaluator.PartialEval(whereExpression);
            var visitor = new WhereExpressionVisitor();
            visitor.Visit(evaled);
            return visitor._current.Build();
        }

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression;
        }

        private void Push(IWhereExpressionBuilder exp)
        {
            _stack.Push(_current);
            _current = exp;
        }

        private void Pop()
        {
            var current = _stack.Pop();
            if (current != null)
            {
                _current.Build().Accept(current);
                _current = current;
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method.DeclaringType == typeof(string) && methodCallExpression.Method.Name == "StartsWith")
            {
                var expression = StripQuotes(methodCallExpression.Arguments[0]);

                Push(new LikeExpressionBuilder(SqlOperator.LikeStartsWith));

                Visit(methodCallExpression.Object);
                Visit(expression);
                Pop();

                return methodCallExpression;
            }
            if (methodCallExpression.Method.DeclaringType == typeof(string) && methodCallExpression.Method.Name == "EndsWith")
            {
                var expression = StripQuotes(methodCallExpression.Arguments[0]);

                Push(new LikeExpressionBuilder(SqlOperator.LikeEndsWith));

                Visit(methodCallExpression.Object);
                Visit(expression);
                Pop();

                return methodCallExpression;
            }
            if (methodCallExpression.Method.DeclaringType == typeof(string) && methodCallExpression.Method.Name == "Contains")
            {
                var expression = StripQuotes(methodCallExpression.Arguments[0]);

                Push(new LikeExpressionBuilder(SqlOperator.LikeContains));

                Visit(methodCallExpression.Object);
                Visit(expression);
                Pop();

                return methodCallExpression;
            }
            if (methodCallExpression.Method.DeclaringType == typeof(Enumerable) && methodCallExpression.Method.Name == "Contains")
            {
                var listExp = StripQuotes(methodCallExpression.Arguments[0]);
                var propExp = StripQuotes(methodCallExpression.Arguments[1]);

                Push(new InExpressionBuilder());

                Visit(propExp);
                Visit(listExp);
                Pop();

                return methodCallExpression;
            }
            if (methodCallExpression.Method.DeclaringType.IsGenericType && methodCallExpression.Method.DeclaringType.GetGenericTypeDefinition() == typeof(List<>) && methodCallExpression.Method.Name == "Contains")
            {
                var expression = StripQuotes(methodCallExpression.Arguments[0]);

                Push(new InExpressionBuilder());

                Visit(expression);
                Visit(methodCallExpression.Object);
                Pop();

                return methodCallExpression;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", methodCallExpression.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            switch (unaryExpression.NodeType)
            {
                case ExpressionType.Not:

                    Push(new NotExpressionBuilder());

                    Visit(unaryExpression.Operand);

                    Pop();
                    break;
                case ExpressionType.Convert:
                    Visit(unaryExpression.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported",
                        unaryExpression.NodeType));
            }

            return unaryExpression;
        }

        protected override Expression VisitBinary(System.Linq.Expressions.BinaryExpression binaryExpression)
        {
            var oper = GetBinaryOperator(binaryExpression.NodeType);

            var expressionBuilder = oper == SqlOperator.And || oper == SqlOperator.Or
                ? new LogicalExpressionBuilder(oper)
                : (IWhereExpressionBuilder) new BinaryExpressionBuilder(oper);

            Push(expressionBuilder);

            Visit(binaryExpression.Left);
            Visit(binaryExpression.Right);

            Pop();

            return binaryExpression;
        }

        protected override Expression VisitConstant(ConstantExpression constantExpression)
        {
            if (constantExpression.Value == null)
            {
                _current.Visit(new NullExpression());
            }
            else
            {
                switch (Type.GetTypeCode(constantExpression.Value.GetType()))
                {
                    case TypeCode.Object:
                        if (constantExpression.Value is IEnumerable && _current is InExpressionBuilder)
                        {
                            _current.Visit(new ValueExpression
                            {
                                Value = constantExpression.Value
                            });
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", constantExpression.Value));
                        }
                        break;
                    default:
                        _current.Visit(new ValueExpression
                        {
                            Value = constantExpression.Value
                        });
                        break;
                }
            }

            return constantExpression;
        }

        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            //    while (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
            //    {
            //        memberExpression = (MemberExpression)memberExpression.Expression;
            //    }

            if (memberExpression.Expression != null && memberExpression.Expression.NodeType == ExpressionType.Parameter)
            {
                var columnMeta = DbMeta.Instance.GetColumn((PropertyInfo)memberExpression.Member);
                _current.Visit(new ColumnExpression
                {
                    Column = columnMeta.Column
                });
                return memberExpression;
            }
            if (memberExpression.Expression != null && memberExpression.Expression.NodeType == ExpressionType.Constant)
            {
                var constantExpr = memberExpression.Expression as ConstantExpression;
                var prop = memberExpression.Member as FieldInfo;
                var value = prop.GetValue(constantExpr.Value);
                _current.Visit(new ValueExpression
                {
                    Value = value
                });
                return memberExpression;
            }
            //if (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
            //{
            //    var memberAccessExpr = memberExpression.Expression as MemberExpression;
            //    return Visit(memberAccessExpr.Expression);
            //}

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", memberExpression.Member.Name));
        }

        private static SqlOperator GetBinaryOperator(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return SqlOperator.And;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return SqlOperator.Or;
                case ExpressionType.Equal:
                    return SqlOperator.Equals;
                case ExpressionType.NotEqual:
                    return SqlOperator.NotEquals;
                case ExpressionType.LessThan:
                    return SqlOperator.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return SqlOperator.LessThanOrEquals;
                case ExpressionType.GreaterThan:
                    return SqlOperator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return SqlOperator.GreaterThanOrEquals;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", nodeType));
            }
        }
    }
}