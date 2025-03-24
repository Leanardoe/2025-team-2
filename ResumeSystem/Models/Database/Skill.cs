namespace ResumeSystem.Models.Database
{
    public class Skill
    {
        //TODO add data annotation
        public int SkillID { get; set; }
        public int CandidateID { get; set; }

        public Candidate Candidate { get; set; }
    }
}
