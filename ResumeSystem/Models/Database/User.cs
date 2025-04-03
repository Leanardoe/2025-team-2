using System.ComponentModel.DataAnnotations;

namespace ResumeSystem.Models.Database
{
    public class User
    {
        [Required]
        public int UserID { get; set; }
        public ICollection<QuerySearch> QuerySearches { get; set; }

        //data required refer to doc for more

    }
}
