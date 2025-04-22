using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResumeSystem.Models.Database
{
    public class User : IdentityUser 
    {
        [Key]
        [Required]
        public int UserID { get; set; }
    }
}