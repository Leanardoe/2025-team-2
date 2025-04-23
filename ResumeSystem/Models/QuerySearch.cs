using Ganss.Text;
using Microsoft.EntityFrameworkCore;
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
        public List<Resume> Resumes { get; set; }

        // list<resume>
        public List<Resume> KeywordSearch(List<string> keywords)
        {
            var newResumes = new List<Resume>();

            foreach (var resume in Resumes)
            {
                resume.Score = 0;
                var ac = new AhoCorasick(CharComparer.OrdinalIgnoreCase, keywords);
                var results = ac.Search(resume.RESUME_STRING).ToList();

                var dictionary = ScoreList(results);
				resume.Score = ScoreResume(dictionary,10);
				resume.Match = (dictionary.Count == 0) ? 0 : (int)Math.Floor((dictionary.Count / (double)keywords.Count) * 100.0);
				newResumes.Add(resume);
			}
            //return newResumes;
			return newResumes.OrderByDescending(r => r.Score).ToList();
		}

        public Dictionary<string,int> ScoreList(IList<WordMatch> resumeslist)
        {
            int lengthStrength = 10;
            var groups = resumeslist
            .GroupBy(s => s.Word)
            .Select(s => new {
            stuff = s.Key,
            Count = s.Count()
             });
            return groups.ToDictionary(g => g.stuff, g => g.Count);
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
