using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Helpers
{
    /// <summary>
    /// 翻页类，进行分页操作并保存分页信息（视频P35)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T>
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// 单页条目数
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 总条目数
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> items,int count,int pageNumber,int pageSize)
        {
            TotalPages = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source,int pageNumber,int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
