using ResumeSystem.Models.Database;

namespace ResumeSystem.Models
{
    public class FileUpload
    {
        public static void ResumeUpload(string filePath, string AIData)
        {
            var resume = new Resume(filePath);

            AIData = "{John Doe,email@email.com,555-808-5560}\nC#,Java,Programming,Python"; //remove this later


        }

        private static Candidate FindCandidate(string CandidateInfo)
        {

        }

        private static IEnumerable<Skill> FindSkills(string SkillsInfo)
        {

        }
    }
}
