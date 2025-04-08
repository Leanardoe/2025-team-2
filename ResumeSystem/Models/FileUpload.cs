using ResumeSystem.Models.Database;
using System.Text.RegularExpressions;

namespace ResumeSystem.Models
{
    public class FileUpload
    {

		private readonly ResumeContext _context;

		public FileUpload(ResumeContext context)
		{
			_context = context;
		}
		public void ResumeUpload(string filePath, string AIData)
        {
			var resume = new Resume
			{
				RESUME_URL = filePath,
				RESUME_STRING = Resume.ResumeToString(filePath)
			};

			AIData = "{John Louis,email@email.com,555-808-5560}|C#,Java,Programming,Jelly Sandwich"; //remove this later

			var split = AIData.Split('|');
			Candidate candidate;
			List<Skill> skills;
			if (split.Length > 1)
			{
				candidate = FindCandidate(split[0]);
				skills = FindSkills(split[1]);

				var uniqueSkills = skills
					.Where(skill => !candidate.Skills.Any(c => c.SKILL_NAME == skill.SKILL_NAME))
					.ToList();

				foreach (var skill in uniqueSkills)
				{
					candidate.Skills.Add(skill);
				}

				candidate.Resumes.Add(resume);

				if (candidate.CandidateID != 0)
				{
					_context.Candidates.Update(candidate);
				}
				else
				{
					_context.Candidates.Add(candidate);
				}

				//save
				_context.SaveChanges();
			}
		}

        private Candidate FindCandidate(string CandidateInfo)
        {
			Candidate candidate = new Candidate();

			CandidateInfo = CandidateInfo.Replace("{", "").Replace("}", "").Trim();

			foreach (var str in CandidateInfo.Split(','))
			{
				if (isPhone(str)) candidate.CAN_PHONE = str;
				else if (isEmail(str)) candidate.CAN_EMAIL = str;
				else candidate.CAN_NAME = str;
			}

			int key = CandidateExists(candidate.CAN_NAME, candidate.CAN_EMAIL, candidate.CAN_PHONE);

			if (key == -1)
			{
				_context.Candidates.Add(candidate);
				//await _context.SaveChangesAsync(); //We can save in the resumeUpload function
			}
			else
			{
				var existingCandidate = _context.Candidates.Find(key);
				if (existingCandidate != null)
				{
					candidate = existingCandidate;
				}

				
			}

			return candidate;
		}

		private List<Skill> FindSkills(string SkillsInfo)
		{
			var stringList = SkillsInfo.Split(',').Distinct().Select(str => str.Trim());

			List<Skill> skillList = new List<Skill>();

			foreach (var skillName in stringList)
			{
				var newSkill = new Skill { SKILL_NAME = skillName };
				skillList.Add(newSkill);	
			}
			return skillList;
		}

		private int CandidateExists(string name, string? email, string? phone)
		{
			var candidate = _context.Candidates.FirstOrDefault(c =>
				c.CAN_NAME.ToLower() == name.ToLower() &&
				(c.CAN_EMAIL ?? "").ToLower() == (email ?? "").ToLower() &&
				(c.CAN_PHONE ?? "").ToLower() == (phone ?? "").ToLower());

			return candidate?.CandidateID ?? -1;
		}

		private bool isEmail(string str)
		{
			string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9-]+\.[a-zA-Z]{2,}$";

			Regex regex = new Regex(pattern);

			return regex.IsMatch(str);
		}

		private bool isPhone(string str)
		{
			string pattern = @"^(?:\+?\d{1,}[\s\-]?)?\(?(\d{3})\)?[\s\-]?\d{3}[\s\-]?\d{4}$";

			Regex regex = new Regex(pattern);

			return regex.IsMatch(str);
		}

		public void AssignSkillsToCandidate(Candidate candidate, List<Skill> skills)
		{
			var uniqueSkills = skills
				.Where(skill => !candidate.Skills.Any(c => c.SKILL_NAME == skill.SKILL_NAME))
				.ToList();
			candidate.Skills.ToList().AddRange(uniqueSkills);
		}
	}
}
