namespace ResumeSystem.Models.Database
{
    public class Candidate
    {
        public int CandidateID { get; set; }
        public int SkillID { get; set; }

        public ICollection<Resume> Resumes { get; set; }
        public ICollection<Skill> Skills { get; set; }
    }
}
