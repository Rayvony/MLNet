using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MLNetProyecto.Web.Areas.Identity.Data;

public class MLNetProyectoDbContext : IdentityDbContext<User>
{
    public MLNetProyectoDbContext(DbContextOptions<MLNetProyectoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
