using Microsoft.EntityFrameworkCore;
using TestPlatform.Models;

namespace TestPlatform.Database;

public class TestPlatformContext : DbContext
{
    public TestPlatformContext(DbContextOptions<TestPlatformContext> options)
        : base(options) { }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<Quetion> Quetions { get; set; }

    public virtual DbSet<Result> Results { get; set; }
    
    public virtual DbSet<Answer> Answeres { get; set; }
}