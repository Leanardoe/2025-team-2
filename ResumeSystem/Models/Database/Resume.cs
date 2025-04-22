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

		public DateOnly UPLOAD_DATE { get; set; } = DateOnly.FromDateTime(DateTime.Today);

		//ignore for database stuff for resume search
		[NotMapped]
        public int Score { get; set; }
		[NotMapped]
		public int Match { get; set; } = 100;

        public static string ResumeToString(string filePath)
        {
			//this is easier than I thought it would be
			try
			{
				//file to string
				return File.ReadAllText(filePath, Encoding.UTF8);
			}
			catch (Exception ex)
			{
				//handle error you know how a try catch works
				return $"Error reading file: {ex.Message}";
			}
		}
	}
}
