using ResumeSystem.Models.Database;
using System.ComponentModel.DataAnnotations;

namespace ResumeSystem.Models.Database
{
    public class Skill
    {
        [Key]
        public int SkillID { get; set; }

        [Required(ErrorMessage = "A candidate ID is required for a skill.")]
        public int CandidateID { get; set; }

        public Candidate Candidate { get; set; }
    }
}

