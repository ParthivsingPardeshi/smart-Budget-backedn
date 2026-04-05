using Microsoft.EntityFrameworkCore;
using finance_backend.Models;

namespace finance_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<FriendTransaction> FriendTransactions { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
