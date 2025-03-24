using ResumeSystem.Models.Database;

namespace ResumeSystem.Models
{
    public class AIProcess
    {
        public Resume Resume;

        public string ProcessResume()
        {
            //use OpenAI to receive a string using https://www.nuget.org/packages/OpenAI-DotNet/
            return "";
        }

        public Candidate CreateCandidate(string str)
        {
            //Take the formatted response from ProcessResume, extract a candidate name, and create a new candidate or add skills to an existing one. 
            return new Candidate();
        }

    }
}
