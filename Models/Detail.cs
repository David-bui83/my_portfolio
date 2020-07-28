using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class Detail
    {
        [Key]
        public int DetailId {get;set;}
        public int ProjectId {get;set;}
        
        [Required(ErrorMessage="Name is required")]
        public string DetailName {get;set;}

        [Required(ErrorMessage="Detail is required")]
        public string DetailDetail {get;set;}
        public Project Project {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
    }
}