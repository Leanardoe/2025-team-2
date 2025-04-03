﻿using Ganss.Text;
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

        // list<resume>
        public List<String> KeywordSearch(List<string> keywords)
        {
            var resumes = new List<Resume>();
           
            var strings = new List<string>();

            //remove V 
            resumes.Add(new Resume { RESUME_STRING = "eggs meaT pinapple grape grapefruit apple Meat" });
            // test code ^

            //Takes a list of keywords and resumes (from filter) and returns a list of Resumes that are scored. This may help: https://stackoverflow.com/questions/687313/building-a-dictionary-of-counts-of-items-in-a-list
            foreach (var resume in resumes)
            {
                var ac = new AhoCorasick(CharComparer.OrdinalIgnoreCase, keywords);
                var results = ac.Search(resume.RESUME_STRING).ToList();

                foreach (var word in results)
                {
                    strings.Add(word.Word);
                }
            }
            return strings;
        }

        public Dictionary<string, int> ScoreList(IList<string> reusumeslist)
        {
            var groups = reusumeslist
           //change .GroupBy(s => s) to .GroupBy(s => s.Word) when it time to use Resumes
           .GroupBy(s => s)
            .Select(s => new {
            stuff = s.Key,
            Count = s.Count()
             });
            return groups.ToDictionary(g => g.stuff, g => g.Count);
        }
    }
}
