using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace resume_site.Models
{
    public class TechIdModel
    {   
      public int AddTechId {get;set;}
      public int RemoveTechId {get;set;}
    }
}