using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PrukoguiWebApp.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options) { }

        public DbSet<Item> Items => Set<Item>();
    }
}
