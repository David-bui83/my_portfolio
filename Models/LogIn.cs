using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class LogIn
    {
        [Required(ErrorMessage="Invalid Username/Password")]
        public string LoginUserName {get;set;}
        
        public string LoginPassword {get;set;}
    }
}