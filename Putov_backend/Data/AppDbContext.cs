using Microsoft.EntityFrameworkCore;
using Putov_backend.Models;


namespace Putov_backend.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Training> Trainings { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<User> Users { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}