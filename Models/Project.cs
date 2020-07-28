using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class Project
    {
        [Key]
        public int ProjectId {get;set;}

        [Required(ErrorMessage="Project name is required")]
        [MinLength(2,ErrorMessage="Project name must be at least 2 characters")]
        public string ProjectName {get;set;}

        [Required(ErrorMessage="Project Image is requried")]
        public string ProjectImage {get;set;}

        [Required(ErrorMessage="Project description is required")]
        [MinLength(10,ErrorMessage="Project description must be at least 10 characters")]
        public string ProjectDescription {get;set;}

        [Required(ErrorMessage="Project link is required")]
        public string ProjectLink {get;set;}

        public string ProjectGitHub {get;set;}

        [Required(ErrorMessage="Project construction selection is required")]
        public bool IsDone {get;set;}
        
        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public List<Association> TechnologyAssociated {get;set;}
    }
}