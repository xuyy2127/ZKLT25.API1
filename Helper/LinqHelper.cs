using System.Linq.Expressions;

namespace ZKLT25.API.Helper
{
    public static class LinqHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ExpAnd<T>(this Expression<Func<T, bool>>? exp1, Expression<Func<T, bool>> exp2)
        {
            if (exp1 == null)
            {
                return exp2;
            }
            var invokeExp = Expression.Invoke(exp2, exp1.Parameters);
            var combinedExp = Expression.Lambda<Func<T, bool>>(Expression.And(exp1.Body, invokeExp), exp1.Parameters);
            return combinedExp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp1"></param>
        /// <param name="flag"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ExpAndIf<T>(this Expression<Func<T, bool>>? exp1, bool flag, Expression<Func<T, bool>> exp2)
        {
            if (!flag)
            {
                return exp1;
            }

            if (exp1 == null)
            {
                return exp2;
            }

            var invokeExp = Expression.Invoke(exp2, exp1.Parameters);
            var combinedExp = Expression.Lambda<Func<T, bool>>(Expression.And(exp1.Body, invokeExp), exp1.Parameters);
            return combinedExp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ExpOr<T>(this Expression<Func<T, bool>>? exp1, Expression<Func<T, bool>> exp2)
        {
            if (exp1 == null)
            {
                return exp2;
            }
            var invokeExp = Expression.Invoke(exp2, exp1.Parameters);
            var combinedExp = Expression.Lambda<Func<T, bool>>(Expression.Or(exp1.Body, invokeExp), exp1.Parameters);
            return combinedExp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp1"></param>
        /// <param name="flag"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ExpOrIf<T>(this Expression<Func<T, bool>>? exp1, bool flag, Expression<Func<T, bool>> exp2)
        {
            if (!flag)
            {
                return exp1;
            }

            if (exp1 == null)
            {
                return exp2;
            }

            var invokeExp = Expression.Invoke(exp2, exp1.Parameters);
            var combinedExp = Expression.Lambda<Func<T, bool>>(Expression.Or(exp1.Body, invokeExp), exp1.Parameters);
            return combinedExp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="orderWhere"></param>
        /// <returns></returns>
        public static IQueryable<T> OrderByIf<T, TKey>(this IQueryable<T> source, bool flag, Expression<Func<T, TKey>> orderWhere) where T : class
        {
            if (!flag)
            {
                return source;
            }
            return source.OrderBy(orderWhere);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="orderWhere"></param>
        /// <returns></returns>
        public static IQueryable<T> OrderByDescendingIf<T, TKey>(this IQueryable<T> source, bool flag, Expression<Func<T, TKey>> orderWhere) where T : class
        {
            if (!flag)
            {
                return source;
            }
            return source.OrderByDescending(orderWhere);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="expWhere"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool flag, Expression<Func<T, bool>> expWhere) where T : class
        {
            if (!flag)
            {
                return source;
            }
            return source.Where(expWhere);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="expWhere"></param>
        /// <returns></returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool flag, Expression<Func<T, bool>> expWhere) where T : class
        {
            if (!flag)
            {
                return source;
            }
            return source.Where(expWhere.Compile());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToPageQuery<T>(this IEnumerable<T> source, int page, int size) where T : class
        {
            if (page <= 0 || size <= 0)
            {
                return source.Where(x => false);
            }
            return source.Skip((page - 1) * size).Take(size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IQueryable<T> ToPageQuery<T>(this IQueryable<T> source, int page, int size, out int count) where T : class
        {
            count = source.Count();

            if (page <= 0 || size <= 0)
            {
                return source.Where(x => false);
            }
            return source.Skip((page - 1) * size).Take(size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IQueryable<T> ToPageQueryIf<T>(this IQueryable<T> source, bool flag, int page, int size, out int count) where T : class
        {
            count = source.Count();

            if (!flag)
            {
                return source;
            }

            if (page <= 0 || size <= 0)
            {
                return source.Where(x => false);
            }
            return source.Skip((page - 1) * size).Take(size);
        }

        /// <summary>
        /// 左连接扩展
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="joinQuery"></param>
        /// <param name="querySelector"></param>
        /// <param name="joinSelector"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IQueryable<TResult> LeftJoin<T1, T2, Tkey, TResult>(
            this IQueryable<T1> source,
            IQueryable<T2> joinQuery,
            Expression<Func<T1, Tkey>> querySelector,
            Expression<Func<T2, Tkey>> joinSelector,
            Func<T1, T2, TResult> selector)
        {
            return source.
                GroupJoin(joinQuery, querySelector, joinSelector, (a, b) => new { a, b })
                .SelectMany(ab => ab.b.DefaultIfEmpty(), (a, b) => selector(a.a, b));
        }
    }
}
