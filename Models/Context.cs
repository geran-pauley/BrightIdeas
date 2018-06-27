using Microsoft.EntityFrameworkCore;

namespace BrightIdeas.Models
{
    public class Context : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<Users> users { get; set; }
        public DbSet<Posts> posts { get; set; }
        public DbSet<Likes> likes { get; set; }
    }
}