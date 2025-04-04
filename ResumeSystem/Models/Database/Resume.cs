using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    }
}

