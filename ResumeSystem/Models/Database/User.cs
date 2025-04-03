using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResumeSystem.Models.Database
{
    public class User
    {
        [Key]
        [Required]
        public int UserID { get; set; }
    }
}
