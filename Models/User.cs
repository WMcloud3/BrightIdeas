using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrightIdeas.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [MinLength(3)]
        public string Name {get;set;}
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Use letters or numbers only please")]
        [MinLength(3)]
        public string Alias {get;set;}
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Must hae 8 characters for password")]
        public string Password {get;set;}
        [NotMapped]
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string Confirm {get;set;}
        public List<Idea> CreatedIdeas {get;set;}
        public List<Like> LikesGiven {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}