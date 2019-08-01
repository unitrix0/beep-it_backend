using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeepBackend.Models;

namespace BeepBackend.Data
{
    public interface IBeepRepository
    {
        Task<User> GetUser(int id);
    }
}
