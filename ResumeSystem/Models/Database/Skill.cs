using System.ComponentModel.DataAnnotations;

namespace ResumeSystem.Models.Database
{
    public class Skill
    {
        [Key]
        public int SkillID { get; set; }

        public string SKILL_NAME { get; set; } = string.Empty;
    }
}
