using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightIdeas.Models
{
    public class Idea
    {
        [Key]
        public int IdeaId{get;set;}
        [Required]
        [MinLength(5)]
        public string Content {get;set;}
        public int UserId{get;set;}
        public User Creator{get;set;}
        public List<Like> Likes {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    }
}