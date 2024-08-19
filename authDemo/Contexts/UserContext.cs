using authDemo.Model;
using Microsoft.EntityFrameworkCore;

namespace authDemo.Contexts
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }
    }
}
