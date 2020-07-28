using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class EnterCode
    { 
        [Required(ErrorMessage="Recovery code is required")]
        public string RecoveryCode {get;set;}        
    }
}