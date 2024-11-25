using Microsoft.EntityFrameworkCore;
using Putov_backend.Models;

public class AppDbContext : DbContext
{
    public DbSet<Training> Trainings { get; set; }
    public DbSet<Participant> Participants { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
