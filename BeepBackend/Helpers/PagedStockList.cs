using BeepBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utrix.WebLib.Pagination;

namespace BeepBackend.Helpers
{
    public class PagedStockList : PagedList<StockEntryValue>
    {
        public int TotalStockAmount { get; set; }

        public PagedStockList(List<StockEntryValue> items, int count, int pageNumber, int pageSize, int totalStockAmount) :
            base(items, count, pageNumber, pageSize)
        {
            TotalStockAmount = totalStockAmount;
        }

        public new static async Task<PagedStockList> CreateAsync(IQueryable<StockEntryValue> source, int pageNumber, int pageSize)
        {
            var totalStockAmount = await source.SumAsync(sev => sev.AmountOnStock);
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedStockList(items, count, pageNumber, pageSize, totalStockAmount);
        }
    }
}
