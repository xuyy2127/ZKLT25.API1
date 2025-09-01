using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ZKLT25.API.Helper;

namespace ZKLT25.API.Helper
{
    /// <summary>
    /// 查询扩展方法
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// 转换为分页列表
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="query">查询对象</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页列表</returns>
        public static async Task<PaginationList<T>> ToPagedListAsync<T>(
            this IQueryable<T> query, 
            int pageNumber, 
            int pageSize) where T : class
        {
            return await PaginationList<T>.CreateAsync(pageNumber, pageSize, query);
        }
        
        /// <summary>
        /// 应用日期范围筛选
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="query">查询对象</param>
        /// <param name="dateProperty">日期属性表达式</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>筛选后的查询对象</returns>
        public static IQueryable<T> ApplyDateRangeFilter<T>(
            this IQueryable<T> query,
            Expression<Func<T, DateTime?>> dateProperty,
            DateTime? startDate,
            DateTime? endDate) where T : class
        {
            if (startDate.HasValue)
            {
                query = query.Where(x => EF.Property<DateTime?>(x, GetMemberName(dateProperty)) >= startDate.Value);
            }
            
            if (endDate.HasValue)
            {
                var endDatePlusOne = endDate.Value.AddDays(1);
                query = query.Where(x => EF.Property<DateTime?>(x, GetMemberName(dateProperty)) < endDatePlusOne);
            }
            
            return query;
        }
        
        /// <summary>
        /// 应用关键字筛选
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="query">查询对象</param>
        /// <param name="property">属性表达式</param>
        /// <param name="keyword">关键字</param>
        /// <returns>筛选后的查询对象</returns>
        public static IQueryable<T> ApplyKeywordFilter<T>(
            this IQueryable<T> query,
            Expression<Func<T, string?>> property,
            string? keyword) where T : class
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => EF.Property<string?>(x, GetMemberName(property)).Contains(keyword));
            }
            return query;
        }
        
        /// <summary>
        /// 获取成员名称
        /// </summary>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns>成员名称</returns>
        private static string GetMemberName<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            
            if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression unaryMemberExpression)
            {
                return unaryMemberExpression.Member.Name;
            }
            
            throw new ArgumentException("表达式必须是成员访问表达式");
        }
    }
}
