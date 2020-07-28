using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace resume_site.Models
{
    public class TechViewModel
    {
        [Required(ErrorMessage="Technology name is required")]        
        public string TechnologyName {get;set;}

        public IFormFile TechnologyImage {get;set;}
    }
}