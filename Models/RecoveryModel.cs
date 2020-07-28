using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class RecoveryModel
    {
        [Required(ErrorMessage="User name is required")]
        public string RecoveryUserName {get;set;}
    }
}