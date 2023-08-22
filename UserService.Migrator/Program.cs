using Microsoft.EntityFrameworkCore;
using UserService.DAL;

var connectionString = "Server=localhost;Port=5432;Database=otus-auth;User Id=postgres;Password=somepassword";//Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
optionsBuilder.UseNpgsql(connectionString, builder =>
{
    builder.CommandTimeout(60);
    builder.EnableRetryOnFailure(100);
});
var context = new UserContext(optionsBuilder.Options);
context.Database.SetCommandTimeout(60);
var mig = context.Database.GetMigrations().Count();
Console.WriteLine(mig);
await context.Database.MigrateAsync();