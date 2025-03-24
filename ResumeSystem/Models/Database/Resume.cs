using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeSystem.Models.Database
{
    public class Resume
    {
        [Required]
        public int ResumeID { get; set; }
        [Required]
        public int CandidateID { get; set; }

        [Required]
        public string RESUME_URL { get; set; }
        [Required]
        public string RESUME_STRING { get; set; }

        //ignore for database stuff for resume
        [NotMapped]
        public int Score { get; set; }

        //ignore for keyword search you will need to add your own constructor
        public Resume(string str)
        {
            RESUME_STRING = str;
        }

        //default constructor just
        public Resume()
        {

        }

        public static string ResumeToString()
        {
            //use  https://www.nuget.org/packages/documentformat.openxml to dump the resume to a string that should be lowercase.
            return "";
        }

    }
}
