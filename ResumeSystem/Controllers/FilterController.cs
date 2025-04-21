using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResumeSystem.Models;
using ResumeSystem.Models.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index(int? skillId)
        {
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
            if (skillId.HasValue && skillId.Value > 0)
            {
                query = query.Where(r =>
                    r.Candidate.Skills.Any(cs => cs.SkillID == skillId.Value)
                );
            }

            // 4) Then eager‑load Candidate and their Skills
            query = query
                .Include(r => r.Candidate)
                    .ThenInclude(c => c.Skills);

            // 5) Execute the query
            var resumes = await query.ToListAsync();

            // 6) Build and return the view‑model
            var vm = new ResumeFilterViewModel
            {
                SelectedSkillID = skillId,
                SkillOptions = skillOptions,
                Resumes = resumes
            };

            return View(vm);
        }
    }
}
