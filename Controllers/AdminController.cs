using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using resume_site.Models;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.Text.RegularExpressions;

namespace resume_site.Controllers
{
    public class AdminController : Controller
    {
        private MyContext dbContext;

        private readonly IHostingEnvironment hostingEnvironment;

        private IConfiguration configuration;

        public AdminController(MyContext context, IHostingEnvironment hostingEnvironment, IConfiguration iConfig)
        {
            this.hostingEnvironment = hostingEnvironment;
            dbContext = context;
            configuration = iConfig;
        }

        public IActionResult Index()
        {
            var userNum = dbContext.Users.Count();
            ViewBag.user = userNum;

            
            return View("Index");
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            var userNum = dbContext.Users.Count();
            if(ModelState.IsValid){

                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already taken");
                    ViewBag.user = userNum;
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> hasher = new PasswordHasher<User>();
                    user.Password = hasher.HashPassword(user,user.Password);
                    dbContext.Add(user);
                    dbContext.SaveChanges();

                    var signedInUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);

                    HttpContext.Session.SetInt32("id",signedInUser.UserId);
                    
                    return RedirectToAction("Dashboard");
                }
            }
            
            ViewBag.user = userNum;
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LogIn loginUser)
        {
            var userNum = dbContext.Users.Count();
            if(ModelState.IsValid)
            {
                var signedInUser = dbContext.Users.FirstOrDefault(u => u.UserName == loginUser.LoginUserName);
                if(signedInUser == null)
                {
                    ModelState.AddModelError("LoginUserName","Invalid Username/Password");
                    ViewBag.user = userNum;
                    return View("Index");
                }

                var hasher = new PasswordHasher<LogIn>();
                var result = hasher.VerifyHashedPassword(loginUser, signedInUser.Password, loginUser.LoginPassword); 

                if(result == 0)
                {
                    ModelState.AddModelError("LoginUserName", "Invalid Username/Password");
                    ViewBag.user = userNum;
                    return View("Index");
                }
                else
                {
                    HttpContext.Session.SetInt32("id", signedInUser.UserId);

                    return RedirectToAction("Dashboard");
                }     
            }

            ViewBag.user = userNum;
            return View("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            ViewBag.user = user;

            List<Technology> technologies = dbContext.Technologies.OrderBy(t => t.TechnologyName).ToList();
            ViewBag.technologies = technologies;

            List<Project> projects = dbContext.Projects.ToList();
            ViewBag.projects = projects;

            return View("Dashboard");
        }

        [HttpGet("edit/user/{id}")]
        public IActionResult EditUser()
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User userEdit = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            ViewBag.userEdit = userEdit;
            return View("EditUser");
        }

        [HttpPost("edit/user/{id}")]
        public IActionResult UpdateUser(UserViewModel userView)
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User userEdit = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            bool isValid = regex.IsMatch(userView.Email);

            if(ModelState.IsValid)
            {
                if(userView.FirstName == null || userView.LastName == null || userView.UserImage == null || !isValid)
                {
                    if(userView.FirstName == null)
                    {
                        ModelState.AddModelError("Firstname","First name is required");
                    }
                    if(userView.LastName == null)
                    {
                        ModelState.AddModelError("LastName","Last name is required");
                    }
                    if(!isValid)
                    {
                        ModelState.AddModelError("Email","Email needs to be in to correct format");
                    }
                    if(userView.UserImage == null)
                    {
                        ModelState.AddModelError("UserImage","User image is required");
                    }
                            
                    ViewBag.userEdit = userEdit;
                    return View("EditUser");
                }
                else
                {
                    string profileImageName = null;
                    string profileResumeName = null;
                    if(userView.UserImage != null)
                    {
                        string imagesFolder = Path.Combine(hostingEnvironment.WebRootPath,"Images");
                        profileImageName = Guid.NewGuid().ToString() + "-" + userView.UserImage.FileName;
                        string imageFileType = Path.GetExtension(profileImageName);
                        
                        if(imageFileType != ".png" && imageFileType != ".jpeg" && imageFileType !=".jpg" && imageFileType != ".gif" && imageFileType != ".tiff")
                        {
                            ModelState.AddModelError("UserImage", "Not a valid image type");
                            ViewBag.userEdit = userEdit;
                            return View("EditUser");
                        }
                        else
                        {
                            if(userEdit.UserImage != null)
                            {
                                List<string> oldFilePath = userEdit.UserImage.Split("/").ToList<string>();

                                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", oldFilePath[1]);

                                if(System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                            }
                            string imagePath = Path.Combine(imagesFolder,profileImageName);
                            string imagePathForDatabase = "Images/"+profileImageName;
                            userView.UserImage.CopyTo(new FileStream(imagePath,FileMode.Create));
                            userEdit.UserImage = imagePathForDatabase;
                            dbContext.Update(userEdit);
                            dbContext.SaveChanges();
                        }
                        
                        string resumeFolder = Path.Combine(hostingEnvironment.WebRootPath,"Images");
                        profileResumeName = Guid.NewGuid().ToString() + "-" + userView.UserResume.FileName;
                        string resumeFileType = Path.GetExtension(profileResumeName);

                        if(resumeFileType != ".pdf")
                        {
                            ModelState.AddModelError("UserResume", "Not a valid file pdf");
                            ViewBag.userEdit = userEdit;
                            return View("EditUser");
                        }
                        else
                        {
                            if(userEdit.UserResume != null)
                            {
                                List<string> oldFilePath = userEdit.UserResume.Split("/").ToList<string>();

                                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", oldFilePath[1]);

                                if(System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                            }
                            string resumePath = Path.Combine(resumeFolder,profileResumeName);
                            string resumePathForDatabase = "Images/"+profileResumeName;
                            userView.UserResume.CopyTo(new FileStream(resumePath,FileMode.Create));
                            userEdit.UserResume = resumePathForDatabase;
                            dbContext.Update(userEdit);
                            dbContext.SaveChanges();
                        }

                        userEdit.FirstName = userView.FirstName;
                        userEdit.LastName = userView.LastName;
                        userEdit.Email = userView.Email;
                        userEdit.Info = userView.UserInfo;
                        dbContext.Update(userEdit);
                        dbContext.SaveChanges();

                        return RedirectToAction("Dashboard");
                    }
                }
            }

            ViewBag.userEdit = userEdit;
            return View("EditUser");
        }

        [HttpGet("user/delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == id);

            List<Association> associations = dbContext.Associations.ToList();

            foreach(var association in associations)
            {
                dbContext.Associations.Remove(association);
                dbContext.SaveChanges();
            }
            
            List<Detail> details = dbContext.Details.ToList();

            foreach(var detail in details)
            {
                dbContext.Details.Remove(detail);
                dbContext.SaveChanges();
            }

            List<Project> projects = dbContext.Projects.ToList();

            foreach(var project in projects)
            {
                if(project.ProjectImage != null)
                {
                    List<string> oldProjectImagePath = project.ProjectImage.Split('/').ToList<string>();

                    var projectImagePath = Path.Combine(Directory.GetCurrentDirectory(),"Images", oldProjectImagePath[1]);

                    if(System.IO.File.Exists(projectImagePath))
                    {
                        System.IO.File.Delete(projectImagePath);
                    }
                }

                dbContext.Projects.Remove(project);
                dbContext.SaveChanges();
            }
            
            List<Technology> technologies = dbContext.Technologies.ToList();

            foreach(var tech in technologies)
            {
                if(tech.TechnologyImage != null)
                {
                    List<string> oldTechImagePath = tech.TechnologyImage.Split('/').ToList<string>();
                    
                    var techImagePath = Path.Combine(Directory.GetCurrentDirectory(),"Images",oldTechImagePath[1]);

                    if(System.IO.File.Exists(techImagePath))
                    {
                        System.IO.File.Delete(techImagePath);
                    }
                }

                dbContext.Technologies.Remove(tech);
                dbContext.SaveChanges();
            }

            if(user.UserImage != null){
                List<string> oldUserImagePath = user.UserImage.Split("/").ToList<string>();

                var userImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", oldUserImagePath[1]);

                if(System.IO.File.Exists(userImagePath))
                {
                    System.IO.File.Delete(userImagePath);
                }
            }

            if(user.UserResume != null)
            {
                List<string> oldUserResumePath = user.UserResume.Split("/").ToList<string>();

                var ResumePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", oldUserResumePath[1]);

                if(System.IO.File.Exists(ResumePath))
                {
                    System.IO.File.Delete(ResumePath);
                }
            }

            dbContext.Remove(user);
            dbContext.SaveChanges();


            return RedirectToAction("Index");
        }

        [HttpPost("reset/password/user/{id}")]
        public IActionResult ResetPasswordInside(UserViewModel reset, int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == id);

            if(ModelState.IsValid)
            {
                if(reset.NewPassword == null || reset.NewPassword.Length < 8 || reset.NewPasswordConfirmation != reset.NewPassword)
                {
                    if(reset.NewPassword == null || reset.NewPassword.Length < 8)
                    {
                        ModelState.AddModelError("NewPassword","Password must be at least 8 characters");
                    }
                    if(reset.NewPasswordConfirmation != reset.NewPassword)
                    {
                        ModelState.AddModelError("NewPasswordConfirmation","Passwords do not match");
                    }
                    ViewBag.user = user;
                    return View("EditUser");
                }

                PasswordHasher<User> hasher = new PasswordHasher<User>();
                user.Password = hasher.HashPassword(user,reset.NewPassword);
                dbContext.Users.Update(user);
                dbContext.SaveChanges();

                return RedirectToAction("Dashboard");
            }

            ViewBag.user = user;

            return View("EditUser");
        }

        [HttpGet("new/project")]
        public IActionResult NewProject()
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            ViewBag.user = user;
            
            return View("NewProject");
        }

        [HttpPost("new/project")]
        public IActionResult CreateProject(ProjectViewModel projectView)
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);

            if(ModelState.IsValid)
            {
                string ProjectImageName = null;
                if(projectView.ProjectImage != null)
                {
                    string imagesFolder = Path.Combine(hostingEnvironment.WebRootPath,"Images");
                    ProjectImageName = Guid.NewGuid().ToString() + "-" + projectView.ProjectImage.FileName;
                    string projectFileType = Path.GetExtension(ProjectImageName);
                        
                    if(projectFileType != ".png" && projectFileType != ".jpeg" && projectFileType !=".jpg" && projectFileType != ".gif" && projectFileType != ".tiff")
                    {
                        ModelState.AddModelError("ProjectImage", "Not a valid image type");
                        ViewBag.user = user;
                        return View("NewProject");
                    }
                    else
                    {
                        string imagePath = Path.Combine(imagesFolder,ProjectImageName);
                        string imagePathForDatabase = "Images/"+ProjectImageName;
                        projectView.ProjectImage.CopyTo(new FileStream(imagePath,FileMode.Create));

                        Project project = new Project();
                        project.ProjectName = projectView.ProjectName;
                        project.ProjectImage = imagePathForDatabase;
                        project.ProjectDescription = projectView.ProjectDescription;
                        project.ProjectLink = projectView.ProjectLink;
                        project.ProjectGitHub = projectView.ProjectGitHub;
                        project.IsDone = projectView.IsViewDone;
                        dbContext.Add(project);
                        dbContext.SaveChanges();

                        return RedirectToAction("Dashboard");
                    }
                }

                ModelState.AddModelError("ProjectImage", "Project image is required");
                ViewBag.user = user;

                return View("NewProject");
            }

            ViewBag.user = user;
            
            return View("NewProject");
        }

        [HttpGet("edit/project/{id}")]
        public IActionResult EditProject(int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User userEdit = dbContext.Users.FirstOrDefault(u => u.UserId == (int)uId);
            ViewBag.userEdit = userEdit;

            Project projectEdit = dbContext.Projects.FirstOrDefault(p => p.ProjectId == id);
            ViewBag.projectEdit = projectEdit;
           
            return View("EditProject");
        }

        [HttpPost("edit/project/{id}")]
        public IActionResult UpdateProject(ProjectViewModel UpdatedProject, int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User userEdit = dbContext.Users.FirstOrDefault(u => u.UserId == uId);

            Project project = dbContext.Projects.FirstOrDefault(p => p.ProjectId == id);

            if(ModelState.IsValid)
            {
                string projectImageName = null;
                if(UpdatedProject.ProjectImage != null)
                {
                    string imagesFolder = Path.Combine(hostingEnvironment.WebRootPath, "Images");
                    projectImageName = Guid.NewGuid().ToString() + "-" + UpdatedProject.ProjectImage.FileName;
                    string projectFileType = Path.GetExtension(projectImageName);
                        
                    if(projectFileType != ".png" && projectFileType != ".jpeg" && projectFileType !=".jpg" && projectFileType != ".gif" && projectFileType != ".tiff")
                    {
                        ModelState.AddModelError("ProjectImage", "Not a valid image type");
                        
                        ViewBag.userEdit = userEdit;
                        ViewBag.projectEdit = project;
                        
                        return View("EditProject");
                    }
                    else
                    {
                        if(project.ProjectImage != null)
                        {
                            List<string> oldFilePath = project.ProjectImage.Split("/").ToList<string>();
                            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/Images",oldFilePath[1]);
                            
                            if(System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }

                        string imagePath = Path.Combine(imagesFolder, projectImageName);
                        string imagePathForDatabase = "Images/"+projectImageName;
                        UpdatedProject.ProjectImage.CopyTo(new FileStream(imagePath,FileMode.Create));

                        project.ProjectName = UpdatedProject.ProjectName;
                        project.ProjectImage = imagePathForDatabase;
                        project.ProjectDescription = UpdatedProject.ProjectDescription;
                        project.ProjectLink = UpdatedProject.ProjectLink;
                        project.ProjectGitHub = UpdatedProject.ProjectGitHub;
                        project.IsDone = UpdatedProject.IsViewDone;

                        dbContext.Update(project);
                        dbContext.SaveChanges();

                        return RedirectToAction("Dashboard");
                    }
                }
                
                ModelState.AddModelError("ProjectImage", "Project image is required");
                
                ViewBag.userEdit = userEdit;
                ViewBag.projectEdit = project;
                
                return View("EditProject");
            }

            ViewBag.ProjectEdit = project;
            ViewBag.userEdit = userEdit;

            return View("EditProject");
        }

        [HttpGet("delete/project/{id}")]
        public IActionResult DeleteProject(int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            Project project = dbContext.Projects.FirstOrDefault(p => p.ProjectId == id);

            List<Association> associations = dbContext.Associations.ToList();
            foreach(var a in associations)
            {
                if(a.ProjectId == id)
                {
                    dbContext.Remove(a);
                    dbContext.SaveChanges();
                }
            }

            List<Detail> details = dbContext.Details.Where(p => p.ProjectId == id).ToList();
            foreach(var detail in details)
            {
                dbContext.Remove(detail);
                dbContext.SaveChanges();
            }

            if(project.ProjectImage != null)
            {
                List<string> oldFilePath = project.ProjectImage.Split("/").ToList<string>();

                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/Images",oldFilePath[1]);
                
                if(System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            dbContext.Projects.Remove(project);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpGet("view/project/{id}")]
        public IActionResult ViewProject(int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == (int)uId);
            ViewBag.user = user;

            Project project = dbContext.Projects.FirstOrDefault(p => p.ProjectId == (int)id);
            ViewBag.project = project;

            var notIncludedTech = dbContext.Technologies.Where(t => !t.ProjectAssociated.Any(p => p.ProjectId == (int)id)).OrderBy(t => t.TechnologyName);
            ViewBag.notTech = notIncludedTech;

            var includedTech = dbContext.Technologies.Where(t => t.ProjectAssociated.Any(p => p.ProjectId == (int)id)).OrderBy(t => t.TechnologyName);
            ViewBag.tech = includedTech;

            int count = dbContext.Technologies.Where(t => t.ProjectAssociated.Any(p => p.ProjectId == (int)id)).Count();
            ViewBag.count = count;

            return View("ViewProject");
        }

        [HttpGet("new/technology")]
        public IActionResult NewTechnology()
        {
            var uId = HttpContext.Session.GetInt32("id");
            
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User userTech = dbContext.Users.FirstOrDefault(u => u.UserId == (int)uId);
            ViewBag.userTech = userTech;
            
            return View("NewTechnology");
        }

        [HttpPost("new/technology")]
        public IActionResult CreateTechnology(TechViewModel tech)
        {
            var uId = HttpContext.Session.GetInt32("id");
            
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            Technology techExisted = dbContext.Technologies.FirstOrDefault(t => t.TechnologyName == tech.TechnologyName);
            
            if(ModelState.IsValid)
            {
                if(techExisted != null)
                {
                    ModelState.AddModelError("TechnologyName","Technology already existed");

                    ViewBag.userTech = user;
                    return View("NewTechnology");
                }

                string techName = null;
                if(tech.TechnologyImage != null)
                {
                    string imagesFolder = Path.Combine(hostingEnvironment.WebRootPath,"Images");
                    techName = Guid.NewGuid().ToString() + "-" + tech.TechnologyImage.FileName;
                    string techFileType = Path.GetExtension(techName);
                        
                    if(techFileType != ".png" && techFileType != ".jpeg" && techFileType !=".jpg" && techFileType != ".gif" && techFileType != ".tiff")
                    {
                        ModelState.AddModelError("TechnologyImage", "Not a valid image type");
                        ViewBag.userTech = user;
                    
                        return View("NewTechnology");
                    }
                    else
                    {
                        string imagePath = Path.Combine(imagesFolder,techName);
                        string imagePathForDatabase = "Images/"+techName;
                        tech.TechnologyImage.CopyTo(new FileStream(imagePath,FileMode.Create));

                        Technology newTech = new Technology();

                        newTech.TechnologyName = tech.TechnologyName;
                        newTech.TechnologyImage = imagePathForDatabase;
                        dbContext.Add(newTech);
                        dbContext.SaveChanges();

                        return RedirectToAction("Dashboard");
                    }
                }

                ModelState.AddModelError("TechnologyImage", "Technology image is required");
                ViewBag.user = user;
                
                return View("NewTechnology");
            }
            
            ViewBag.user = user;
            return View("NewTechnology");
        }

        [HttpGet("edit/technology/{id}")]
        public IActionResult EditTechnology(int id)
        {
            var uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            ViewBag.user = user;
            
            Technology tech = dbContext.Technologies.FirstOrDefault(t => t.TechnologyId == id);
            ViewBag.tech = tech;

            return View("EditTechnology");
        }

        [HttpPost("edit/technology/{id}")]
        public IActionResult UpdateTechnology(TechViewModel UpdateTech, int id)
        {
            var uId = HttpContext.Session.GetInt32("id");
            
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            
            Technology tech = dbContext.Technologies.FirstOrDefault(t => t.TechnologyId == id);
            
            if(ModelState.IsValid)
            {
                string techImageName =null;
                if(UpdateTech.TechnologyImage != null)
                {
                    
                    string imagesFolder = Path.Combine(hostingEnvironment.WebRootPath, "Images");
                    techImageName = Guid.NewGuid().ToString() + "-" + UpdateTech.TechnologyImage.FileName;
                    string techFileType = Path.GetExtension(techImageName);
                        
                    if(techFileType != ".png" && techFileType != ".jpeg" && techFileType !=".jpg" && techFileType != ".gif" && techFileType != ".tiff")
                    {
                        ModelState.AddModelError("TechnologyImage", "Not a valid image type");
                        ViewBag.user = user;
                        ViewBag.tech = tech;
                    
                        return View("EditTechnology");
                    }
                    else
                    {
                        if(tech.TechnologyImage != null)
                        {
                            List<string> oldFilePath = tech.TechnologyImage.Split("/").ToList<string>();

                            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/Images", oldFilePath[1]);
                            if(System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }
                        
                        string imagePath = Path.Combine(imagesFolder,techImageName);
                        string imagePathForDatabase = "Images/" + techImageName;
                        UpdateTech.TechnologyImage.CopyTo(new FileStream(imagePath,FileMode.Create));

                        tech.TechnologyName = UpdateTech.TechnologyName;
                        tech.TechnologyImage = imagePathForDatabase;
                        dbContext.Update(tech);
                        dbContext.SaveChanges();

                        return RedirectToAction("Dashboard");
                    }
                }

                ModelState.AddModelError("TechnologyImage","Technology image is required");
                
                ViewBag.user = user;
                ViewBag.tech = tech;

                return View("EditTechnology");
            }

            ViewBag.user = user;
            ViewBag.tech = tech;

            return View("EditTechnology");
        }

        [HttpGet("delete/technololgy/{id}")]
        public IActionResult DeleteTechnology(int id)
        {
            var uId = HttpContext.Session.GetInt32("id");
            
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            Technology tech = dbContext.Technologies.FirstOrDefault(t => t.TechnologyId == id);
            
            List<Association> association = dbContext.Associations.ToList();
            
            foreach(var t in association)
            {
                if(t.TechnologyId == id)
                {
                    dbContext.Remove(t);
                }
            }

            if(tech.TechnologyImage != null)
            {
                List<string> oldFilePath = tech.TechnologyImage.Split("/").ToList<string>();

                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/Images", oldFilePath[1]);
                if(System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            dbContext.Technologies.Remove(tech);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpPost("add/technology/{id}")]
        public IActionResult AddTechnology(TechIdModel tech, int id)
        {
            var uId = HttpContext.Session.GetInt32("id");
            
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            Association association = new Association();

            association.ProjectId = id;
            association.TechnologyId = tech.AddTechId;

            dbContext.Add(association);
            dbContext.SaveChanges();
            
            return RedirectToAction("ViewProject", new {id=id});
        }

        [HttpPost("remove/technology/{id}")]
        public IActionResult RemoveTechnology(TechIdModel tech, int id)
        {
            var uId = HttpContext.Session.GetInt32("id");
            
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            Association association = dbContext.Associations.FirstOrDefault(a =>a.ProjectId == id && a.TechnologyId == tech.RemoveTechId);
            dbContext.Remove(association);
            dbContext.SaveChanges();

            return RedirectToAction("viewProject", new {id=id});
        }

        [HttpGet("add/detail/{id}")]
        public IActionResult AddDetail(int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            Project project = dbContext.Projects.FirstOrDefault(p => p.ProjectId == id);
            var includedTech = dbContext.Technologies.Where(t => t.ProjectAssociated.Any(p => p.ProjectId == id)).ToList();
            ViewBag.tech = includedTech;
            List<Detail> details = dbContext.Details.Where(d => d.ProjectId == id).ToList();

            ViewBag.user = user;
            ViewBag.project = project;
            ViewBag.tech = includedTech;
            ViewBag.details = details;

            return View("Detail");
        }

        [HttpPost("create/detail/{id}")]
        public IActionResult CreateDetail(Detail detail, int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");
            
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            Project project = dbContext.Projects.FirstOrDefault(p => p.ProjectId == id);
            var includedTech = dbContext.Technologies.Where(t => t.ProjectAssociated.Any(p => p.ProjectId == id)).ToList();
            List<Detail> details = dbContext.Details.Where(d => d.ProjectId == id).ToList();

            if(ModelState.IsValid)
            {
                detail.ProjectId = id;
                dbContext.Details.Add(detail);
                dbContext.SaveChanges();

                return RedirectToAction("AddDetail", new {id=id});
            }

            ViewBag.user = user;
            ViewBag.project = project;
            ViewBag.tech = includedTech;
            ViewBag.details = details;

            return View("Detail");
        }

        [HttpGet("view/detail/{id}")]
        public IActionResult ViewDetail(int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }
            
            Detail detail = dbContext.Details.FirstOrDefault(d => d.DetailId == id);
            ViewBag.detail = detail;
            User userDetail = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            ViewBag.userDetail = userDetail;
            Project projectDetail = dbContext.Projects.FirstOrDefault(p => p.ProjectId == detail.ProjectId);
            ViewBag.projectDetail = projectDetail;
            List<Technology> includedTech = dbContext.Technologies.Where(t => t.ProjectAssociated.Any(p => p.ProjectId == projectDetail.ProjectId)).ToList();
            ViewBag.tech = includedTech;

            return View("ViewDetail");
        }

        [HttpGet("edit/detail/{id}")]
        public IActionResult EditDetail(int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }
            
            Detail detail = dbContext.Details.FirstOrDefault(d => d.DetailId == id);
            ViewBag.detail = detail;
            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            ViewBag.user = user;
            Project project = dbContext.Projects.FirstOrDefault(p => p.ProjectId == detail.ProjectId);
            ViewBag.project = project;
            List<Technology> includedTech = dbContext.Technologies.Where(t => t.ProjectAssociated.Any(p => p.ProjectId == project.ProjectId)).ToList();
            ViewBag.tech = includedTech;
            
            return View("EditDetail");
        }

        [HttpPost("edit/detail/{id}")]
        public IActionResult UpdateDetail(Detail UpdatedDetail, int id)
        {
            int? uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            Detail detail = dbContext.Details.FirstOrDefault(d => d.DetailId == id);
            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            Project project = dbContext.Projects.FirstOrDefault(p => p.ProjectId == detail.ProjectId);
            List<Technology> includedTech = dbContext.Technologies.Where(t => t.ProjectAssociated.Any(p => p.ProjectId == project.ProjectId)).ToList();
            
            if(ModelState.IsValid)
            {
                detail.DetailName = UpdatedDetail.DetailName;
                detail.DetailDetail = UpdatedDetail.DetailDetail;
                dbContext.Details.Update(detail);
                dbContext.SaveChanges();

                return RedirectToAction("AddDetail", new {id=project.ProjectId});
            }
            
            ViewBag.detail = detail;
            ViewBag.user = user;
            ViewBag.project = project;
            ViewBag.tech = includedTech;
            
            return View("EditDetail"); 
        }

        [HttpGet("delete/detail/{id}")]
        public IActionResult DeleteDetail(int id)
        {
            var uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            Detail detail = dbContext.Details.FirstOrDefault(d => d.DetailId == id);
            Project project = dbContext.Projects.FirstOrDefault(p => p.ProjectId == detail.ProjectId);

            dbContext.Details.Remove(detail);
            dbContext.SaveChanges();

            return RedirectToAction("AddDetail", new {id = project.ProjectId});
        }

        [HttpGet("site/stats")]
        public IActionResult SiteStats()
        {
            var uId = HttpContext.Session.GetInt32("id");
            
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            ViewBag.user = user;

            List<EmailMessage> messages = dbContext.EmailMessages.ToList();
            ViewBag.messages = messages;

            List<VisitorTracker> visitors = dbContext.VisitorTrackers.ToList();
            ViewBag.visitors = visitors;

            List<HackerTracker> hackers = dbContext.HackerTrackers.ToList();
            ViewBag.hackers = hackers;

            return View("SiteStats");
        }

        [HttpGet("message/delete/{id}")]
        public IActionResult DeleteEmailMessage(int id)
        {
            var uId = HttpContext.Session.GetInt32("id");
            
            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            EmailMessage message = dbContext.EmailMessages.FirstOrDefault(e => e.EmailId == id);

            dbContext.EmailMessages.Remove(message);
            dbContext.SaveChanges();

            return RedirectToAction("SiteStats");
        }

        [HttpGet("message/delete/all")]
        public IActionResult DeleteAllMessages()
        {
            var uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            List<EmailMessage> messages = dbContext.EmailMessages.ToList();
            
            foreach(var message in messages)
            {
                dbContext.EmailMessages.Remove(message);
                dbContext.SaveChanges();
            }

            return RedirectToAction("SiteStats");
        }

        [HttpGet("view/message/{id}")]
        public IActionResult ViewEmailMessage(int id)
        {
            var uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == uId);
            ViewBag.user = user;

            EmailMessage message = dbContext.EmailMessages.FirstOrDefault(e => e.EmailId == id);
            ViewBag.message = message;

            return View("ViewEmailMessage");
        }

        [HttpGet("visitor/delete/{id}")]
        public IActionResult DeleteVisitor(int id)
        {
            var uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            VisitorTracker visitor = dbContext.VisitorTrackers.FirstOrDefault(v => v.VisitorId == id);
            
            dbContext.VisitorTrackers.Remove(visitor);
            dbContext.SaveChanges();

            return RedirectToAction("SiteStats");
        }

        [HttpGet("visitor/delete/all")]
        public IActionResult DeleteAllVisitors()
        {
            var uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            List<VisitorTracker> visitors = dbContext.VisitorTrackers.ToList();

            foreach(var visitor in visitors)
            {
                dbContext.VisitorTrackers.Remove(visitor);
                dbContext.SaveChanges();
            }

            return RedirectToAction("SiteStats");
        }

        [HttpGet("hacker/delete/{id}")]
        public IActionResult DeleteHacker(int id)
        {
            var uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            HackerTracker hacker = dbContext.HackerTrackers.FirstOrDefault(h => h.HackerId == id);

            dbContext.HackerTrackers.Remove(hacker);
            dbContext.SaveChanges();

            return RedirectToAction("SiteStats");
        }

        [HttpGet("/hacker/delete/all")]
        public IActionResult DeleteAllHackers()
        {
            var uId = HttpContext.Session.GetInt32("id");

            if(uId == null)
            {
                return RedirectToAction("Danger");
            }

            List<HackerTracker> hackers = dbContext.HackerTrackers.ToList();

            foreach(var hacker in hackers)
            {
                dbContext.HackerTrackers.Remove(hacker);
                dbContext.SaveChanges();
            }

            return RedirectToAction("SiteStats");
        }
        

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return View("Index");
        }

        [HttpGet("password/recovery")]
        public IActionResult PasswordRecovery()
        {
            return View("PasswordRecovery");
        }

        [HttpPost("password/recovery")]
        public IActionResult PasswordRecovery(RecoveryModel recovery)
        {
            if(ModelState.IsValid)
            {
                User user = dbContext.Users.FirstOrDefault(u => u.UserName == recovery.RecoveryUserName);
                if(user != null)
                {
                    string code = "";
                    char[] newHex = {'0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','y','x','z','A','B','C','D','E','F','G','H','I','J','K','L','M','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

                    Random random = new Random();
                    
                    for(int i = 0; i < 10; i++)
                    {
                        int num = random.Next(0,63);
                        code += newHex[num];
                    }

                    string fromEmail = "portfoliosite83@gmail.com";
                    string subject = "password recovery";
                    string toEmail = user.Email;
                    var emailBody = "<h1 style='text-align: center;'>Email from My Profile Site</h1><hr><p>Enter the code into the form to reset password</p><p><span style='font-weight:bold;'>Code:</span> " + code + "</p><br><br><br><p>Do Not Reply...</p>";

                    MailMessage message = new MailMessage(fromEmail, toEmail, subject, emailBody);
                    message.IsBodyHtml = true;
                    
                    SmtpClient client = new SmtpClient("smtp.gmail.com",587);
                    client.EnableSsl = true;

                    client.Credentials = new System.Net.NetworkCredential("portfoliosite83@gmail.com","portfolio20!");
                    client.Send(message);

                    HttpContext.Session.SetString("code",code);
                    HttpContext.Session.SetString("userName",user.UserName);
                    return RedirectToAction("EnterCode");
                }
                ModelState.AddModelError("RecoveryUserName", "Invalid user name");
                return View("PasswordRecovery");
            }
            return View("PasswordRecovery");
        }

        [HttpGet("password/recovery/code")]
        public IActionResult EnterCode()
        {
            string userName = HttpContext.Session.GetString("userName");
            if(userName != null)
            {
                return View("EnterCode");
            }
            return RedirectToAction("Danger");
        }

        [HttpPost("password/recovery/code")]
        public IActionResult EnterCode(EnterCode enterCode)
        {
            string userName = HttpContext.Session.GetString("userName");
            if(userName != null)
            {
                if(ModelState.IsValid)
                {
                    string code = HttpContext.Session.GetString("code");

                    if(enterCode.RecoveryCode == code)
                    { 
                        return RedirectToAction("ResetPassword");
                    }
                    ModelState.AddModelError("RecoveryCode","Invalid code");
                    return View("EnterCode");
                }
                return View("EnterCode");
            }
            return View("Index");
        }

        [HttpGet("password/recovery/reset")]
        public IActionResult ResetPassword()
        {
            string userName = HttpContext.Session.GetString("userName");
            if(userName != null)
            {
                return View("ResetPassword");
            }
            return RedirectToAction("Danger");
        }

        [HttpPost("password/recovery/reset")]
        public IActionResult ResetPassword(NewPassword np)
        {
            string userName = HttpContext.Session.GetString("userName");
            if(userName != null)
            {
                if(ModelState.IsValid)
                {
                    User user = dbContext.Users.FirstOrDefault(u => u.UserName == userName);

                    PasswordHasher<NewPassword> hasher = new PasswordHasher<NewPassword>();
                    
                    user.Password = hasher.HashPassword(np,np.NewPass);
                    dbContext.Users.Update(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.Clear();

                    return View("Index");
                }

                return View("ResetPassword");
            }
            return RedirectToAction("Danger");
        }

        [HttpGet("password/recovery/cancel")]
        public IActionResult CancelRecovery()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }

        [HttpGet("security/danger")]
        public IActionResult Danger()
        {
            return View("Danger");
        }

        [HttpPost("security/danger")]
        public IActionResult Danger(HackerTracker hacker)
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine(hacker);
            dbContext.HackerTrackers.Add(hacker);
            dbContext.SaveChanges();

            string today = DateTime.Now.ToString("MM/dd/yyyy");
            string emailSubject = "Hacker Alert";
            string ToEmail = configuration.GetSection("Email").GetSection("Account").Value;
            string fromEmail = configuration.GetSection("Email").GetSection("Account").Value;
            string emailPassword = configuration.GetSection("Email").GetSection("Password").Value;
            string emailBody ="<h1 style='text-align: center;'>Email From My Portfolio Site</h1><hr><p><span style='font-weight: bold;'>Date</span>: "+ today + "</p><p><span style='font-weight: bold;'>Device</span>: " + hacker.HackerDevice + "</p><p><span style='font-weight: bold;'>IP Address</span>: " + hacker.HackerIp + "</p>";

            MailMessage message = new MailMessage(fromEmail,ToEmail,emailSubject,emailBody);
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient("smtp.gmail.com",587);
            client.EnableSsl = true;

            client.Credentials = new System.Net.NetworkCredential(fromEmail,emailPassword);
            client.Send(message);
            
            return Json(new {hackerDevice=hacker.HackerDevice,hackerIp=hacker.HackerIp});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}