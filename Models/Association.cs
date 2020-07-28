using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class Association
    {
        [Key]
        [Column("id")]
        public int AssociationId {get;set;}

        [Column("project_id")]
        public int? ProjectId {get;set;}

        [Column("technology_id")]
        public int? TechnologyId {get;set;}
        public Project Project {get;set;}
        public Technology Technology {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}