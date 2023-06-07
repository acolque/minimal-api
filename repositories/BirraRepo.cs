using Microsoft.EntityFrameworkCore;

public class BirraRepo : DbContext
{
    public BirraRepo(DbContextOptions<BirraRepo> options) : base(options)
    {
    }

    public DbSet<Birra> Birras => Set<Birra>();
}