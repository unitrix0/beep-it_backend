using System.Collections.Generic;
using System.Threading.Tasks;
using BeepBackend.Models;

namespace BeepBackend.Data
{
    public interface IShoppingListRepo
    {
        Task<IEnumerable<ShoppingListEntry>> GetShoppingListAsync(int environmentId);
    }
}