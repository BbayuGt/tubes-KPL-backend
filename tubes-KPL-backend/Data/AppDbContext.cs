using Microsoft.EntityFrameworkCore;
using tubes_KPL_backend.Models;

namespace tubes_KPL_backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }

}