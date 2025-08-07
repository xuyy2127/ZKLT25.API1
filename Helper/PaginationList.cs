using Microsoft.EntityFrameworkCore;

namespace ZKLT25.API.Helper
{
    public class PaginationList<T> : List<T>
    {
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PaginationList(int totalCount, int currentPage, int pageSize, List<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(items);
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public static async Task<PaginationList<T>> CreateAsync(int currentPage, int pageSize, IQueryable<T> result)
        {
            var totalCount = await result.CountAsync();

            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            result = result.Take(pageSize);

            var items = await result.ToListAsync();
            return new PaginationList<T>(totalCount, currentPage, pageSize, items);
        }

        public static PaginationList<T> CreateFromList(int currentPage,int pageSize,List<T> sourceList)
        {
            // 参数校验
            if (currentPage < 1) throw new ArgumentException("页码不能小于1");
            if (pageSize < 1) throw new ArgumentException("页容量不能小于1");

            var totalCount = sourceList.Count;

            // 计算实际分页区间
            var skip = (currentPage - 1) * pageSize;
            var take = pageSize;

            // 处理越界情况
            if (skip >= totalCount)
                return new PaginationList<T>(totalCount, currentPage, pageSize, new List<T>());

            // 内存分页操作
            var pagedData = sourceList
                .Skip(skip)
                .Take(take)
                .ToList();

            return new PaginationList<T>(totalCount, currentPage, pageSize, pagedData);
        }

    }
}
