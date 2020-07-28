using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using resume_site.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
// using Twilio;
// using Twilio.Rest.Api.V2010.Account;
// using Twilio.Types;

// using Twilio.TwiML;

namespace resume_site.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        // private readonly IOptions<TwilioSettings> _TwilioStettings;
        private IConfiguration configuration;
        public HomeController(MyContext context, IConfiguration iConfig)
        {  
            dbContext = context;
            configuration = iConfig;
        }

        // public HomeController(IOptions<TwilioSettings> twilioSettings, MyContext context, IConfiguration iConfig)
        // {  
        //     dbContext = context;
        //     _TwilioStettings = twilioSettings;
        //     configuration = iConfig;
        // }
       
        public IActionResult Index()
        {
            List<Project> projects = dbContext.Projects.ToList();
            ViewBag.projectList = projects.Take(4);
            
            User user = dbContext.Users.OrderBy(u => u.UserId).FirstOrDefault();
            ViewBag.user = user;
            return View("Index");
        }

        [HttpPost("visitor/tracker")]
        public IActionResult VisitorTracker(VisitorTracker visitor)
        {
            dbContext.VisitorTrackers.Add(visitor);
            dbContext.SaveChanges();

            return new EmptyResult();
        }

        [HttpGet("about")]
        public IActionResult About()
        {
            User user = dbContext.Users.OrderBy(u => u.UserId).FirstOrDefault();
            ViewBag.user = user;

            return View("About");
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            // string googleApiKey = configuration.GetSection("GoogleApi").GetSection("Key").Value;
            
            // ViewBag.googleApiKey = googleApiKey;
            
            return View("Contact");
        }

        [HttpPost("mail")]
        public IActionResult Mail(EmailMessage emailMessage)
        {
            string portfolioEmail = configuration.GetSection("Email").GetSection("Account").Value;
            string emailPassword = configuration.GetSection("Email").GetSection("Password").Value;

            if(ModelState.IsValid){

                emailMessage.ToEmail = portfolioEmail;
                dbContext.EmailMessages.Add(emailMessage);
                dbContext.SaveChanges();

                var emailBody = "<h1 style='text-align: center;'>Email From My Profile Site</h1><hr><p><span style='font-weight:bold;'>Name:</span> " + emailMessage.Name + "</p><p><span style='font-weight: bold;'>Email:</span> " + emailMessage.FromEmail + "</p><p><span style='font-weight: bold;'>Message: </span><p><p>" + emailMessage.Message + "</p><br><br><br><p>Do Not Reply...</p>";

                MailMessage message = new MailMessage(emailMessage.FromEmail,portfolioEmail,emailMessage.Subject,emailBody);
                
                message.IsBodyHtml = true;
                
                SmtpClient client = new SmtpClient("smtp.gmail.com",587);
                client.EnableSsl = true;
                
                // client.Credentials = new System.Net.NetworkCredential("email account here", "email password here");
                
                client.Credentials = new System.Net.NetworkCredential(portfolioEmail, emailPassword);
                client.Send(message);

                return Json(new {Name=emailMessage.Name});
            }
           
            // string googleApiKey = configuration.GetSection("GoogleApi").GetSection("Key").Value;
            // ViewBag.googleApiKey = googleApiKey;
            return View("Contact");
        }

        // [HttpPost("sendSms")]
        // public IActionResult SendSms(TextMessage text)
        // {
        //     if(ModelState.IsValid)
        //     {
        //         string accountSid = _TwilioStettings.Value.AccountSid;
        //         string authToken = _TwilioStettings.Value.AuthToken;


        //         TwilioClient.Init(accountSid,authToken);

        //         var message = MessageResource.Create(
        //             body: text.TxtName + "\n" + text.TxtPhone + "\n" + text.TxtMessage,
        //             from: new Twilio.Types.PhoneNumber(_TwilioStettings.Value.TwilioPhoneNumber),
        //             to: new Twilio.Types.PhoneNumber("enter phone number")
        //         );

        //         HttpContext.Session.SetString("send","text");

        //         return RedirectToAction("Success");
        //     }

        //     string googleApiKey = configuration.GetSection("GoogleApi").GetSection("Key").Value;
        //     ViewBag.googleApiKey = googleApiKey;
            
        //     return View("Contact");
        // }

        [HttpGet("project")]
        public IActionResult Project()
        {
            List<Project> projects = dbContext.Projects.ToList();
            ViewBag.projects = projects;

            return View("Project");
        }

        [HttpGet("project/view/{id}")]
        public IActionResult ViewProject(int id)
        {
            Project project = dbContext.Projects.FirstOrDefault(p => p.ProjectId == id);
            if(project == null)
            {
                return View("NoProject");
            }

            List<Technology> techs = dbContext.Technologies.Where(t => t.ProjectAssociated.Any(p => p.ProjectId == id)).ToList();
            ViewBag.techs = techs;

            List<Detail> details = dbContext.Details.Where(p => p.ProjectId == project.ProjectId).ToList();
            ViewBag.details = details;

            return View("View",project);
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View("Privacy");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("{*url}", Order = int.MaxValue)]
        public IActionResult CatchAll()
        {  
            this.Response.StatusCode = StatusCodes.Status404NotFound;  
            return View("CatchAll");
        }
    }
}
