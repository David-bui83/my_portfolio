using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required(ErrorMessage="First name is required")]
        [MinLength(2,ErrorMessage="First name must be at least 2 characters")]
        public string FirstName {get;set;}

        [Required(ErrorMessage="Last name is required")]
        [MinLength(2,ErrorMessage="last name must be at least 2 characters")]
        public string LastName {get;set;}

        [Required(ErrorMessage="User name is required")]
        [MinLength(2,ErrorMessage="Username must be at least 2 characters")]
        public string UserName {get;set;}

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage="Email is required")]
        [EmailAddress(ErrorMessage="Email must be in the correct email format")]
        public string Email {get;set;}

        public string UserImage {get;set;}

        public string UserResume {get;set;}

        public string Info {get;set;}

        [DataType(DataType.Password)]
        [Required(ErrorMessage="Password is required")]
        [MinLength(8,ErrorMessage="Password must be 8 characters or longer")]

        public string Password {get;set;}

        [NotMapped]
        [Compare("Password",ErrorMessage="Passwords do not macth")]
        [DataType(DataType.Password)]
        public string ConfirmPassword {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<Project> Projects {get;set;}

    }
}