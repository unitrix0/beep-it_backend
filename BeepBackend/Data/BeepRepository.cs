using System;
using System.Linq;
using System.Threading.Tasks;
using BeepBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BeepBackend.Data
{
    public class BeepRepository : IBeepRepository
    {
        private readonly DataContext _context;

        public BeepRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> GetUser(int id)
        {
            throw new NotImplementedException();
        }
    }
}