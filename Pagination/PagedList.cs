using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Pagination
{
    public class PagedList<T> : List<T>
    {
        public MetaData? MetaData { get; set; }
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData(count, pageSize, pageNumber, (int)Math.Ceiling(count / (double)pageSize), pageNumber > 1, pageNumber < (int)Math.Ceiling(count / (double)pageSize));
            AddRange(items);
        }
        public async static Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {  
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);          
        }
    }

    public record MetaData(int TotalCount, int PageSize, int CurrentPage, int TotalPages, bool hasPrevious, bool hasNext);
}
