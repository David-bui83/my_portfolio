using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace resume_site.Models
{
    public class HackerTracker
    {
        [Key]
        public int HackerId {get;set;}

        public string HackerDevice {get;set;}

        public string HackerIp {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}