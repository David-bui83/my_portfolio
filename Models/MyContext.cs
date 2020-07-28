using Microsoft.EntityFrameworkCore;

namespace resume_site.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) {}

        public DbSet<User> Users {get;set;}
        public DbSet<Technology> Technologies {get;set;}
        public DbSet<Project> Projects {get;set;}
        public DbSet<Detail> Details {get;set;}
        public DbSet<Association> Associations {get;set;}
        public DbSet<HackerTracker> HackerTrackers {get;set;}
        public DbSet<VisitorTracker> VisitorTrackers {get;set;}
        public DbSet<EmailMessage> EmailMessages {get;set;}
    }
}