using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ResumeSystem.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ResumeSystem.Models
{
    public class QuerySearch
    {
        [Required]
        public int SearchID { get; set; }
        [Required]
        public int UserID { get; set; }

        public User User { get; set; }

        [NotMapped]
        private List<Resume> Resumes { get; set; }

        public void Filter()
        {
            //Returns a list of resumes that have the users specified skills and stores it in the private Resumes variable. May do filterAdd and filterRemove to save time.
        }

        public List<Resume> KeywordSearch(List<string> keywords)
        {
            var resumes = new List<Resume>();
            //Takes a list of keywords and resumes (from filter) and returns a list of Resumes that are scored. This may help: https://stackoverflow.com/questions/687313/building-a-dictionary-of-counts-of-items-in-a-list
            return resumes;
        }
    }
}
