using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace resume_site.Models
{
    public class EmailMessage
    {
        [Key]
        public int EmailId {get;set;}

        [Required(ErrorMessage="Name is required")]
        [MinLength(2,ErrorMessage="Name must be at least 2 characters")]
        public string Name {get;set;}

        [Required(ErrorMessage="Subject is required.")]
        [MinLength(2,ErrorMessage="Subject must be at least 2 characters")]
        public string Subject {get;set;}

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"\(?\d{3}\)?-?.? *\d{3}-?.? *-?\d{4}|\d{1}?-?.? *\(?\d{3}\)?-?.? *\d{3}-?.? *-?\d{4}",ErrorMessage="Not in correct format")]
        public string PhoneNumber {get;set;}

        [Required(ErrorMessage="Message is required.")]
        [MinLength(10,ErrorMessage="Message must be at least 10 characters")]
        public string Message {get;set;}

        public string ToEmail {get;set;}

        [Required(ErrorMessage="Email is required")]
        [EmailAddress(ErrorMessage="Email needs to be in a valid email format")]
        [DataType(DataType.EmailAddress)]
        public string FromEmail {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}