﻿using Ganss.Text;
using Microsoft.EntityFrameworkCore;
using ResumeSystem.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResumeSystem.Models
{
    public class QuerySearch
    {
        private ResumeContext _context;

        public QuerySearch(ResumeContext ctx)
        {
            _context = ctx;
        }
        private List<Resume> Resumes { get; set; }

		public async Task FilterAsync(List<Skill> skills)
		{
			var skillIdSet = skills.Select(s => s.SkillID).ToHashSet();

			var candidates = await _context.Candidates
				.Where(c =>
					skillIdSet.All(id => c.CandidateSkills.Any(cs => cs.SkillID == id))
				)
				.Include(c => c.Resumes)
				.ToListAsync();

			Resumes = candidates.SelectMany(c => c.Resumes).ToList();
		}

		// list<resume>
		public List<Resume> KeywordSearch(List<string> keywords)
        {
            Resumes = new List<Resume>();
            var newResumes = new List<Resume>();

            foreach (var resume in Resumes)
            {
                resume.Score = 0;
                var ac = new AhoCorasick(CharComparer.OrdinalIgnoreCase, keywords);
                var results = ac.Search(resume.RESUME_STRING).ToList();

				resume.Score = ScoreList(results);
                resume.Matches = results.Count();
                newResumes.Add(resume);
			}
            //return newResumes;
			return newResumes.OrderByDescending(r => r.Score).ToList();
		}

        public int ScoreList(IList<WordMatch> resumeslist)
        {
            int lengthStrength = 10;
            var groups = resumeslist
            .GroupBy(s => s.Word)
            .Select(s => new {
            stuff = s.Key,
            Count = s.Count()
             });
            return ScoreResume(groups.ToDictionary(g => g.stuff, g => g.Count),lengthStrength);
        }

        public int ScoreResume(Dictionary<string, int> scores, int mult) 
        {
            int length = scores.Keys.Count;
            int score = length * mult;
            foreach (var resumes in scores) {
                score += resumes.Value;
            }

            return score;
        }
    }
}
