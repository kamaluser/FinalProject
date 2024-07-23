using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.Service.Dtos
{
    public class PaginatedList<T>
    {
        public PaginatedList(List<T> items, int totalPages, int pageIndex, int pageSize)
        {
            Items = items;
            TotalPages = totalPages;
            PageIndex = pageIndex;
            PageSize = pageSize;
            HasPrev = pageIndex > 1;
            HasNext = pageIndex < totalPages;
        }

        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrev { get; set; }
        public bool HasNext { get; set; }

        public static PaginatedList<T> Create(IQueryable<T> query, int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<T>(items, totalPages, pageIndex, pageSize);
        }
    }
}
