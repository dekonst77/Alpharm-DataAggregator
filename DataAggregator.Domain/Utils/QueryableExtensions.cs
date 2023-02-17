using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Utils
{
    public static class QueryableExtensions
    {
        public static IQueryable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
               this IQueryable<TOuter> outer,
               IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector,
               Expression<Func<TInner, TKey>> innerKeySelector,
               Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            var query = outer
                .GroupJoin(inner, outerKeySelector, innerKeySelector, (o, i) => new { o, i })
                .SelectMany(o => o.i.DefaultIfEmpty(), (x, i) => new { x.o, i });
            return ApplySelector(query, x => x.o, x => x.i, resultSelector);
        }

        private static IQueryable<TResult> ApplySelector<TSource, TOuter, TInner, TResult>(
            IQueryable<TSource> source,
            Expression<Func<TSource, TOuter>> outerProperty,
            Expression<Func<TSource, TInner>> innerProperty,
            Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            var p = Expression.Parameter(typeof(TSource), $"param_{Guid.NewGuid()}".Replace("-", string.Empty));
            Expression body = resultSelector?.Body
                .ReplaceParameter(resultSelector.Parameters[0], outerProperty.Body.ReplaceParameter(outerProperty.Parameters[0], p))
                .ReplaceParameter(resultSelector.Parameters[1], innerProperty.Body.ReplaceParameter(innerProperty.Parameters[0], p));
            var selector = Expression.Lambda<Func<TSource, TResult>>(body, p);
            return source.Select(selector);
        }
    }

    public static class ExpressionExtensions
    {
        public static Expression ReplaceParameter(this Expression source, ParameterExpression toReplace, Expression newExpression)
            => new ReplaceParameterExpressionVisitor(toReplace, newExpression).Visit(source);
    }

    public class ReplaceParameterExpressionVisitor : ExpressionVisitor
    {
        public ReplaceParameterExpressionVisitor(ParameterExpression toReplace, Expression replacement)
        {
            this.ToReplace = toReplace;
            this.Replacement = replacement;
        }

        public ParameterExpression ToReplace { get; }
        public Expression Replacement { get; }
        protected override Expression VisitParameter(ParameterExpression node)
            => (node == ToReplace) ? Replacement : base.VisitParameter(node);
    }
}
