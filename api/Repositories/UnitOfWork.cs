using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Models;

namespace api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PrnprojectContext _context;

        public UnitOfWork(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }

}
