using Microsoft.EntityFrameworkCore;
using Pinjet.Models;

namespace Pinjet.Data
{
    public class PinjetDbContext : DbContext
    {
        public PinjetDbContext(DbContextOptions<PinjetDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshToken { get; set; }
    }
}
