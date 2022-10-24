using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            :base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }        
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Employment> Employments { get; set; }


    }
}
