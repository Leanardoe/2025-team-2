using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResumeSystem.Models.Database;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

//TODO implement encryption

namespace ResumeSystem.Models
{
    public class FileUpload
    {
        private readonly ResumeContext _context;

        public FileUpload(ResumeContext context)
        {
            _context = context;
        }

        // ResumeUpload with blob support and manual candidate
        public void ResumeUpload(string filePath, string AIData, string resumeBody, Candidate candidate, string blobUrl)
        {
            var resume = new Resume
            {
                RESUME_URL = blobUrl,
                RESUME_STRING = resumeBody
            };

            var split = AIData.Split('|'); //split the data into candidate info
            List<Skill> skills;
            if (split.Length > 1)
            {
                skills = FindSkills(split[1], candidate);

                //check if candidate already has skill and if not add it.
                foreach (var skill in skills)
                {
                    try
                    {
                        //check if candidate has skill
                        var existingLink = _context.Set<CandidateSkill>()
                            .FirstOrDefault(cs => cs.CandidateID == candidate.CandidateID && cs.SkillID == skill.SkillID);
                        //we are manually handling the junction table automatically as I had problems with EFCore
                        if (existingLink == null)
                        {
                            candidate.CandidateSkills.Add(new CandidateSkill
                            {
                                Candidate = candidate,
                                Skill = skill
                            });
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Error adding skill {skill.SkillID} - {skill.SKILL_NAME}");
                    }
                }

                //add resume to candidate
                candidate.Resumes.Add(resume);

                //update candidate if it exists, otherwise add a candidate
                if (candidate.CandidateID != 0)
                {
                    _context.Candidates.Update(candidate);
                }
                else
                {
                    _context.Candidates.Add(candidate);
                }
                //save the database
                _context.SaveChanges();
            }
        }

        // ResumeUpload with blob support only
        public void ResumeUpload(string filePath, string AIData, string resumeBody, string blobUrl)
        {
            var candidate = new Candidate();
            ResumeUpload(filePath, AIData, resumeBody, candidate, blobUrl);
        }

        // ResumeUpload (legacy single file)
        //this will be used when the user enters candidate info manually
        public void ResumeUpload(string filePath, string AIData, string resumeBody)
        {
            var resume = new Resume
            {
                RESUME_URL = filePath,
                RESUME_STRING = resumeBody
            };

            var split = AIData.Split('|'); //split the data into candidate info
            Candidate candidate;
            List<Skill> skills;
            if (split.Length > 1)
            {
                candidate = FindCandidate(split[0]);
                skills = FindSkills(split[1], candidate);

                //check if candidate already has skill and if not add it.
                foreach (var skill in skills)
                {
                    try
                    {
                        //check if candidate has skill
                        var existingLink = _context.Set<CandidateSkill>()
                            .FirstOrDefault(cs => cs.CandidateID == candidate.CandidateID && cs.SkillID == skill.SkillID);

                        //we are manually handling the junction table automatically as I had problems with EFCore
                        if (existingLink == null)
                        {
                            candidate.CandidateSkills.Add(new CandidateSkill
                            {
                                Candidate = candidate,
                                Skill = skill
                            });
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Error adding skill {skill.SkillID} - {skill.SKILL_NAME}");
                    }
                }

                //add resume to candidate

                candidate.Resumes.Add(resume);

                //update candidate if it exists, otherwise add a candidate

                if (candidate.CandidateID != 0)
                {
                    _context.Candidates.Update(candidate);
                }
                else
                {
                    _context.Candidates.Add(candidate);
                }

                //save the database

                _context.SaveChanges();
            }
        }

        //determine if a candidate exists based on name, email and phone, extracted from CandidateInfo
        private Candidate FindCandidate(string CandidateInfo)
        {
            Candidate candidate = new Candidate();

            CandidateInfo = CandidateInfo.Trim();
            var candidateInfo = CandidateInfo.Split(',');

            for (int i = 0; i < candidateInfo.Length; i++)
            {
                var str = candidateInfo[i].Trim();
                if (i == 0)
                {
                    candidate.CAN_NAME = str;
                    continue;
                }
                else if (isPhone(str)) candidate.CAN_PHONE = str;
                else if (isEmail(str)) candidate.CAN_EMAIL = str;
            }

            int key = CandidateExists(candidate.CAN_NAME, candidate.CAN_EMAIL, candidate.CAN_PHONE);

            if (key == -1) //candidate does not exist in the database
            {
                _context.Candidates.Add(candidate);
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

        //determine if any skills in the list already exists in the database
        private List<Skill> FindSkills(string SkillsInfo, Candidate candidate)
        {
            var stringList = SkillsInfo.Split('␟').Distinct().Select(str => str.Trim());
            List<Skill> skillList = new List<Skill>();

            foreach (var skillName in stringList)
            {
                var existingSkill = _context.Skills
                    .FirstOrDefault(s => s.SKILL_NAME.ToLower().Equals(skillName.ToLower()));

                if (existingSkill != null)
                {
                    skillList.Add(existingSkill);
                }
                else
                {
                    //does not exist add a new one
                    var newSkill = new Skill { SKILL_NAME = skillName };
                    skillList.Add(newSkill);
                }
            }

            return skillList;
        }

        public int CandidateExists(string name, string? email, string? phone)
        {
            var candidate = _context.Candidates.FirstOrDefault(c =>
                c.CAN_NAME.ToLower() == name.ToLower() &&
                (c.CAN_EMAIL ?? "").ToLower() == (email ?? "").ToLower() &&
                (c.CAN_PHONE ?? "").ToLower() == (phone ?? "").ToLower());

            return candidate?.CandidateID ?? -1;
        }

        private bool isEmail(string str) //determines if something meets the criteria of an email, just a fallback if AI is out of order
        {
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(str);
        }

        private bool isPhone(string str) //determines if something meets the criteria of a phone number, just a fallback if AI is out of order
        {
            string pattern = @"^(?:\+?\d{1,}[\s\-]?)?\(?\d{3}\)?[\s\-]?\d{3}[\s\-]?\d{4}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(str);
        }

        public static string FileConverter(string filePath)
        {
            if (Path.GetExtension(filePath).ToLower() == ".docx") // word document
            {
                var sb = new StringBuilder();
                using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false))
                {
                    var body = doc.MainDocumentPart.Document.Body;
                    sb.Append(body.InnerText + " ");
                }
                return sb.ToString();
            }
            else if (Path.GetExtension(filePath).ToLower() == ".txt") // text file
            {
                using var reader = new StreamReader(filePath, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            else if (Path.GetExtension(filePath).ToLower() == ".pdf") // pdf file
            {
                var sb = new StringBuilder();
                using (var document = PdfDocument.Open(filePath))
                {
                    foreach (var page in document.GetPages())
                    {
                        sb.AppendLine(page.Text); // Extracts only text from the pages
                    }
                }
                return sb.ToString();
            }

            return "--!!XX!!--";
        }

        public static string FileConverter(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".docx") // Word document
            {
                var sb = new StringBuilder();
                using (var stream = file.OpenReadStream())
                using (var doc = WordprocessingDocument.Open(stream, false))
                {
                    var body = doc.MainDocumentPart.Document.Body;
                    sb.Append(body.InnerText);
                }
                return sb.ToString();
            }
            else if (extension == ".txt") // Text file
            {
                using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                return reader.ReadToEnd();
            }
            else if (extension == ".pdf") // PDF file
            {
                var sb = new StringBuilder();
                using (var stream = file.OpenReadStream())
                using (var document = PdfDocument.Open(stream))
                {
                    foreach (var page in document.GetPages())
                    {
                        sb.AppendLine(page.Text);
                    }
                }
                return sb.ToString();
            }

            return "--!!XX!!--"; // Unsupported file
        }
    }
}
