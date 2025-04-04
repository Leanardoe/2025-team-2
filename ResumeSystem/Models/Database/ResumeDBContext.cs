using Microsoft.EntityFrameworkCore;
using ResumeSystem.Models.Database;

namespace ResumeSystem.Models.Database
{
    public class ResumeDatabaseContext : DbContext
    {
        public ResumeDatabaseContext(DbContextOptions<ResumeDatabaseContext> options) : base(options)
        {
        }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<User> Users { get; set; }
    }
}