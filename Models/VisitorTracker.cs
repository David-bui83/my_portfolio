using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class VisitorTracker
    {
        [Key]
        public int VisitorId {get;set;}
        public string VisitorDevice {get;set;}
        public string VisitorIp {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}