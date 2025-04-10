﻿using Microsoft.EntityFrameworkCore;
using ResumeSystem.Models.Database;

namespace ResumeSystem.Models.Database
{
    public class ResumeContext : DbContext
    {
        public ResumeContext(DbContextOptions<ResumeContext> options) : base(options)
        {
        }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<CandidateSkill>()
		    .HasKey(cs => new { cs.CandidateID, cs.SkillID });

			modelBuilder.Entity<CandidateSkill>()
				.HasOne(cs => cs.Candidate)
				.WithMany(c => c.CandidateSkills)
				.HasForeignKey(cs => cs.CandidateID);

			modelBuilder.Entity<CandidateSkill>()
				.HasOne(cs => cs.Skill)
				.WithMany(s => s.CandidateSkills)
				.HasForeignKey(cs => cs.SkillID);
		}
    }
}