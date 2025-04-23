using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeSystem.Models.Database;
using System.Collections.Generic;

namespace ResumeSystem.Models
{
    public class ResumeFilterViewModel
    {
        // Holds the selected SkillID (or null = all)
        public List<int> SelectedSkillIDs { get; set; }

        // Populated from your Skills table
        public List<SelectListItem> SkillOptions { get; set; }

        //Search term
        public string? SearchTerm { get; set; }

        // The Resumes matching the filter
        public List<Resume> Resumes { get; set; }
    }
}
