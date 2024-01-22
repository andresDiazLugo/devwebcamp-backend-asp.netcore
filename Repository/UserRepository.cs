using dev_backend_net.Models;
using Microsoft.EntityFrameworkCore;


namespace dev_backend_net.Repository
{
    public class UserRepository : DbContext
    {
        public UserRepository(DbContextOptions<UserRepository> options) : base(options)
        {

        }
        public DbSet<User> users { get; set; }


    }
}