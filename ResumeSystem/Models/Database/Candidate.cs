﻿using System.ComponentModel.DataAnnotations;

namespace ResumeSystem.Models.Database
{
    public class Candidate
    {
        [Key]
        public int CandidateID { get; set; }

        [Required(ErrorMessage = "A candidate must have a name.")]
        public string CAN_NAME { get; set; }

        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        public string? CAN_EMAIL { get; set; }

        [Phone(ErrorMessage = "The phone number is not valid.")]
        public string? CAN_PHONE { get; set; }

        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
		public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
	}
}