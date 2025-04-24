using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResumeSystem.Models;
using ResumeSystem.Models.Database;
using System.Text;

namespace ResumeSystem.Controllers
{
    public class FilterController : Controller
    {
        private readonly ResumeContext context;

        public FilterController(ResumeContext ctx)
        {
            context = ctx;
        }

        // GET: /Filter
        [HttpGet]
		[Authorize]
		public async Task<IActionResult> Filtering([FromQuery] string SelectedSkillIDs, [FromQuery] string? SearchTerm)
        {
            
			// 0) Populate the skills list
			var skillIds = new List<int>();

			if (!string.IsNullOrWhiteSpace(SelectedSkillIDs))
			{
				skillIds = SelectedSkillIDs
					.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(id => int.TryParse(id, out var parsed) ? parsed : (int?)null)
					.Where(id => id.HasValue)
					.Select(id => id.Value)
					.ToList();
			}
			// 1) Populate the skills dropdown
			var skillOptions = await context.Skills
                .OrderBy(s => s.SKILL_NAME)
                .Select(s => new SelectListItem
                {
                    Value = s.SkillID.ToString(),
                    Text = s.SKILL_NAME
                })
                .ToListAsync();

            // 2) Start with all resumes
            IQueryable<Resume> query = context.Resumes;

            // 3) If a skill filter was provided, restrict to resumes whose Candidate has that skill
            if (skillIds.Count > 0)
            {
				query = query.Where(r =>
	                skillIds.All(id =>
		                r.Candidate.CandidateSkills.Any(cs => cs.SkillID == id)
	                )
                );
			}

            // 4) Then eager-load Candidate and their Skills (CandidateSkills)
            query = query
                .Include(r => r.Candidate) // Include the Candidate
                    .ThenInclude(c => c.CandidateSkills) // Include Candidate's skills
                        .ThenInclude(cs => cs.Skill); // Assuming CandidateSkill has a navigation property to Skill


            // 5) Execute the query
            var resumes = await query.ToListAsync();


            // 6) search resumes
            var qs = new QuerySearch() { Resumes = resumes };
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
				SearchTerm = SearchTerm.Normalize(NormalizationForm.FormC).Trim();

				// If the last character after trimming is a comma, remove it
				if (SearchTerm.EndsWith(","))
				{
					SearchTerm = SearchTerm.Substring(0, SearchTerm.Length - 1).TrimEnd();
				}
				List<string> searchKeywords = SearchTerm
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                var searchedResumes = qs.KeywordSearch(searchKeywords);

                if (searchedResumes.Count > 0)
                {
                    resumes = searchedResumes.OrderByDescending(r => r.Score).ToList(); ;
                }
            }

            // 6) Build and return the view‑model
            var vm = new ResumeFilterViewModel
            {
                SelectedSkillIDs = skillIds,
                SkillOptions = skillOptions,
                SearchTerm = SearchTerm,
                Resumes = resumes
            };

            return View(vm);
        }

		[Authorize]
		public IActionResult Delete(int id)
        {
			// 1. Find the resume to delete
			var resume = context.Resumes
				.Include(r => r.Candidate)
				.FirstOrDefault(r => r.ResumeID == id);

			if (resume != null)
			{
				var candidate = resume.Candidate;

				// 2. Delete the resume
				context.Resumes.Remove(resume);
				context.SaveChanges();

				// 3. Check if the candidate has any resumes left
				bool hasOtherResumes = context.Resumes.Any(r => r.CandidateID == candidate.CandidateID);

				if (!hasOtherResumes)
				{
					// Remove candidate
					context.Candidates.Remove(candidate);
					context.SaveChanges();
				}

				// 4. Cleanup orphaned skills
				var orphanedSkillIds = context.Skills
					.Where(s => !context.CandidateSkills.Any(cs => cs.SkillID == s.SkillID))
					.Select(s => s.SkillID)
					.ToList();

				var orphanedSkills = context.Skills
					.Where(s => orphanedSkillIds.Contains(s.SkillID))
					.ToList();

				context.Skills.RemoveRange(orphanedSkills);
				context.SaveChanges();
			}

			return RedirectToAction("Filtering");
        }
	}
}
