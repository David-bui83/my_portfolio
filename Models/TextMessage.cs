using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class TextMessage
    {
        [Required(ErrorMessage="Name is required")]
        [MinLength(2,ErrorMessage="Name must be at least 2 characters")]
        public string TxtName {get;set;}

        [Required(ErrorMessage="Phone number is required")]
        public string TxtPhone {get;set;}

        [Required(ErrorMessage="Message is required")]
        [MinLength(10,ErrorMessage="Message needs to be between 10 to 240 characters")]
        [MaxLength(240,ErrorMessage="Message needs to be between 10 to 240 characters")]
        public string TxtMessage {get;set;}
    }
}