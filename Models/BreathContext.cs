using Microsoft.EntityFrameworkCore;
 
namespace Breathtaking.Models
{
    public class BreathContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public BreathContext(DbContextOptions<BreathContext> options) : base(options) { }

        public DbSet<User> users {get;set;}
        public DbSet<Review> reviews {get;set;}
    }
}