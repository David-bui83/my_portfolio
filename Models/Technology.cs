using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class Technology
    {
        [Key]
        public int TechnologyId {get;set;}

        [Required(ErrorMessage="Name is required")]
        [MinLength(2,ErrorMessage="Name must be at least 2 characters")]
        public string TechnologyName {get;set;}

        [Required(ErrorMessage="Image is required")]
        public string TechnologyImage {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<Association> ProjectAssociated {get;set;}
    }
}