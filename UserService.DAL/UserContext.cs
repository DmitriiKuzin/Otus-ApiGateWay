using Microsoft.EntityFrameworkCore;
using UserService.DAL.Model;

namespace UserService.DAL;

public class UserContext: DbContext
{

    public UserContext()
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
        base.OnConfiguring(optionsBuilder);
    }

    public UserContext(DbContextOptions<UserContext> options): base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
}