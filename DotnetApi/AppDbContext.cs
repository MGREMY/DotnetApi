using DotnetApi.Model;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
}