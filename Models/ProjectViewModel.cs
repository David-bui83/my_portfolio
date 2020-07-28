using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace resume_site.Models
{
    public class ProjectViewModel
    {
        [Required(ErrorMessage="Project Name is required")]
        public string ProjectName {get;set;}
        public IFormFile ProjectImage {get;set;}

        [Required(ErrorMessage="Project description is required")]
        public string ProjectDescription {get;set;}

        [Required(ErrorMessage="Project link is required")]
        public string ProjectLink {get;set;}

        public string ProjectGitHub {get;set;}

        [Required(ErrorMessage="Project constuction selection is required")]
        public bool IsViewDone {get;set;}
    }
}