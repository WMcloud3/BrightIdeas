using System;
using System.ComponentModel.DataAnnotations;

namespace BrightIdeas.Models
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string LoginEmail {get;set;}
        [Required]
        [DataType(DataType.Password)]
        public string LoginPassword {get;set;}
    }
}