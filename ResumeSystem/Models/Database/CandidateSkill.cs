namespace ResumeSystem.Models.Database
{
	public class CandidateSkill
	{
		public int CandidateID { get; set; }
		public Candidate Candidate { get; set; }

		public int SkillID { get; set; }
		public Skill Skill { get; set; }
	}
}
