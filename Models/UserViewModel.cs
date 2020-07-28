using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace resume_site.Models
{
    public class UserViewModel
    {
        public int UserId {get;set;}

        // [Required(ErrorMessage="First name is required")]
        // [MinLength(2,ErrorMessage="First name must be at least 2 characters")]
        public string FirstName {get;set;}

        // [Required(ErrorMessage="Last name is required")]
        // [MinLength(2,ErrorMessage="last name must at least 2 characters")]
        public string LastName {get;set;}

        public string UserName{get;set;}

        // [DataType(DataType.EmailAddress)]
        // [Required(ErrorMessage="Eamil is required")]
        // [EmailAddress(ErrorMessage="Email must be in the correct email format")]
        public string Email {get;set;}
        public IFormFile UserImage {get;set;}

        public IFormFile UserResume {get;set;}

       [DataType(DataType.MultilineText)]
        public string UserInfo {get;set;} = "";

        public string NewPassword {get;set;}

        public string NewPasswordConfirmation {get;set;}
    }
}