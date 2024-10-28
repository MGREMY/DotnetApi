using DotnetApi.Model;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}