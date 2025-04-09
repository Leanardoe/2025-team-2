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

		[Required(ErrorMessage = "Resume content is required.")]
		public string RESUME_STRING { get; set; }

		//public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();

		//ignore for database stuff for resume
		[NotMapped]
        public int Score { get; set; }

        public static string ResumeToString(string filePath)
        {
			try
			{
				// Read all text from the file at the given path
				return File.ReadAllText(filePath, Encoding.UTF8);
			}
			catch (Exception ex)
			{
				// Handle or log the error as needed
				return $"Error reading file: {ex.Message}";
			}
		}
	}
}
