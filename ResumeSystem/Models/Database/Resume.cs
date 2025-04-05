using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ResumeSystem.Models.Database
{
    public class Resume
    {
        [Key]
        public int ResumeID { get; set; }

        [Required(ErrorMessage = "A candidate ID is required for a resume.")]
        public int CandidateID { get; set; }

        public Candidate Candidate { get; set; }

        [Required(ErrorMessage = "A resume URL is required.")]
        public string RESUME_URL { get; set; }

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

        public static string ResumeToString(string filePath)
        {
            //use  https://www.nuget.org/packages/documentformat.openxml to dump the resume to a string that should be lowercase.
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                var body = wordDoc.MainDocumentPart?.Document?.Body;
                if (body == null) { return "--NO STRING FOUND--"; }

                StringBuilder sb = new StringBuilder(1024);

                foreach (var text in body.Descendants<Text>())
                {
                    sb.Append(text.Text).Append(' ');
                }
                
                return sb.ToString().ToLower();
            }
        }
	}
}
