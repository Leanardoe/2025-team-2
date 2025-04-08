using Ganss.Text;
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

        // list<resume>
        public List<Resume> KeywordSearch(List<string> keywords)
        {
            Resumes = new List<Resume>();
            var newResumes = new List<Resume>();

            foreach (var resume in Resumes)
            {
                resume.Score = 0;
                var ac = new AhoCorasick(CharComparer.OrdinalIgnoreCase, keywords);
                var results = ac.Search(resume.RESUME_STRING).ToList();

				resume.Score = ScoreList(results);
                newResumes.Add(resume);
			}
            return newResumes;
		}

        public int ScoreList(IList<WordMatch> resumeslist)
        {
            int lengthStrength = 10;
            var groups = resumeslist
            .GroupBy(s => s.Word)
            .Select(s => new {
            stuff = s.Key,
            Count = s.Count()
             });
            return ScoreResume(groups.ToDictionary(g => g.stuff, g => g.Count),lengthStrength);
        }

        public int ScoreResume(Dictionary<string, int> scores, int mult) 
        {
            int length = scores.Keys.Count;
            int score = length * mult;
            foreach (var resumes in scores) {
                score += resumes.Value;
            }

            return score;
        }
    }
}
