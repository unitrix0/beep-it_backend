using BeepBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Data
{
    public class ShoppingListRepo : IShoppingListRepo
    {
        private readonly BeepDbContext _context;

        public ShoppingListRepo(BeepDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShoppingListEntry>> GetShoppingListAsync(int environmentId)
        {
            List<ShoppingListEntry> listEntries = await _context.ShoppingList
                .Where(sl => sl.Needed > 0 && sl.EnvironmentId == environmentId)
                .GroupBy(sl => sl.StoreName, sl => sl)
                .Select(grp => new ShoppingListEntry() { StoreName = grp.Key, Articles = grp })
                .ToListAsync();

            return listEntries;
        }

        public async Task<IEnumerable<ShoppingListGroupEntry>> GetShoppingListGroupEntriesAsync(int environmentId)
        {
            List<ShoppingListGroupEntry> entries = await _context.ShoppingListGroups
                .Where(sl => sl.EnvironmentId == environmentId && sl.Needed > 0)
                .ToListAsync();

            return entries;
        }
    }
}
