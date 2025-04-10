using System.ComponentModel.DataAnnotations;

namespace ResumeSystem.Models.Database
{
    public class Skill
    {
        [Key]
        public int SkillID { get; set; }

        [Required]
        public string SKILL_NAME { get; set; } = string.Empty;

		public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
	}
}
