using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogEngine.Models;

namespace BlogEngine.Data;

public class ApplicationDbContext : IdentityDbContext<Author>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Post> Post { get; set; } = default!;
}
