using Microsoft.EntityFrameworkCore;

namespace DotnetApi;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}