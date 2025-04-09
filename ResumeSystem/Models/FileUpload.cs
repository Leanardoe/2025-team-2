﻿using Microsoft.EntityFrameworkCore;
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

			AIData = "{Phil Louis,email@email.com,555-808-9987}|C#,Java,Music,Singing,The News"; //remove this later

			var split = AIData.Split('|');
			Candidate candidate;
			List<Skill> skills;
			if (split.Length > 1)
			{
				candidate = FindCandidate(split[0]);
				skills = FindSkills(split[1],candidate);

				if (candidate.CandidateID != 0)
				{
					candidate.Skills.Clear(); // Clear previous skills
				}

				// Go through each skill and add only if it's not already present
				foreach (var skill in skills)
				{
					try
					{
						// Check if the skill is already associated with the candidate by SkillID or SKILL_NAME
						var existingSkill = candidate.Skills.FirstOrDefault(s => s.SkillID == skill.SkillID || s.SKILL_NAME == skill.SKILL_NAME);

						// If the skill is not already in the candidate's skills, add it
						if (existingSkill == null)
						{
							candidate.Skills.Add(skill);
						}
					}
					catch
					{
						Console.WriteLine(skill.SkillID);
					}
				}

				// Add resume to the candidate's resume collection
				candidate.Resumes.Add(resume);

				// You only need to update the candidate once after modifying the skills and resumes
				if (candidate.CandidateID != 0)
				{
					_context.Candidates.Update(candidate);
				}
				else
				{
					_context.Candidates.Add(candidate);
				}

				// Save all changes to the database at once
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
				//_context.Candidates.Add(candidate);
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

		private List<Skill> FindSkills(string SkillsInfo, Candidate candidate)
		{
			var stringList = SkillsInfo.Split(',').Distinct().Select(str => str.Trim());

			List<Skill> skillList = new List<Skill>();

			foreach (var skillName in stringList)
			{
				var existingSkill = _context.Skills
					.FirstOrDefault(s => s.SKILL_NAME.Equals(skillName));

				if (existingSkill != null)
				{
					bool isSkillPresent = candidate.Skills.Any(c => c.SkillID == existingSkill.SkillID);
					if (!isSkillPresent)
					{
						skillList.Add(existingSkill);
					}
				}
				else
				{
					// If the skill doesn't exist, create a new one
					var newSkill = new Skill { SKILL_NAME = skillName };
					skillList.Add(newSkill);
				}
			}
			List<Skill> duplicateSkillsList = new List<Skill>();
			foreach (var item in skillList)
			{
				// Check if the skill already exists in the candidate's skills (either by SkillName or SkillID)
				bool skillExists = candidate.Skills.Any(c => c.SKILL_NAME == item.SKILL_NAME || c.SkillID == item.SkillID);

				if (!skillExists)
				{
					// If the skill doesn't exist, add it to the duplicateSkillsList
					duplicateSkillsList.Add(item);
				}
			}
			return duplicateSkillsList;
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

	public class SkillComparer : IEqualityComparer<Skill>
	{
		public bool Equals(Skill x, Skill y)
		{
			// Check if both skills are the same based on SKILL_NAME and SkillID
			if (x == null || y == null) return false;
			return x.SKILL_NAME == y.SKILL_NAME && x.SkillID == y.SkillID;
		}

		public int GetHashCode(Skill obj)
		{
			// Combine both SkillID and SKILL_NAME to generate a unique hash
			if (obj == null) return 0;
			return obj.SKILL_NAME.GetHashCode() ^ obj.SkillID.GetHashCode();
		}
	}
}
