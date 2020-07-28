using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class NewPassword
    {
        [Required(ErrorMessage="New Password is required")]
        [MinLength(8,ErrorMessage="New Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string NewPass {get;set;}

        [Required(ErrorMessage="New Confirm password is required")]
        [Compare("NewPass", ErrorMessage="Passwords do not match")]
        public string NewConfirmPass {get;set;}
    }
}