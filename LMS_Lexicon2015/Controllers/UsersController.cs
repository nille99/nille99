﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS_Lexicon2015.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.IO;



namespace LMS_Lexicon2015.Controllers
{
    
    
    [Authorize]
    public class UsersController : Controller
    {
       //private ApplicationSignInManager _signInManager;
        //private ApplicationUserManager _userManager;

        //public UsersController()
        //{
        //}

        //public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set
        //    {
        //        _signInManager = value;
        //    }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}
        public static bool FromPartitialView;

        private ApplicationDbContext db = new ApplicationDbContext();



        // GET: Skapar en lista med användare
        public ActionResult Index(string sortOrder, string searchString)
        {

            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "LastName_desc" : "LastName";
            ViewBag.RolesSortParm = sortOrder == "Roles" ? "Roles_desc" : "Roles";
  
            ViewBag.GroupSortParm = sortOrder == "Group" ? "Group_desc" : "Group";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.PhoneNumberSortParm = sortOrder == "PhoneNumber" ? "PhoneNumber_desc" : "PhoneNumber";

            ViewBag.searchString = searchString;

            var Users = from s in db.Users select s;
            if (!String.IsNullOrEmpty(searchString))
        {
                Users = Users.Where(s => s.FirstName.Contains(searchString)
                || s.LastName.Contains(searchString)
                || (s.FirstName + " " + s.LastName).Contains(searchString)
                
                //|| s.Roles.FirstOrDefault().RoleId.Contains(searchString)
                //|| ((s.Roles.FirstOrDefault().RoleId).ToString().(string).Name).Contains(searchString)
                 //|| s.Group.Name.Contains(searchString)
                //|| s.Email.Contains(searchString)
                //|| s.PhoneNumber.Contains(searchString)
                 );
            }



            switch (sortOrder)
            {
                case "FirstName_desc":
                    Users = Users.OrderByDescending(s => s.FirstName);
                    break;
                case "FirstName":
                    Users = Users.OrderBy(s => s.FirstName);
                    break;

                case "LastName_desc":
                    Users = Users.OrderByDescending(s => s.LastName);
                    break;
                case "LastName":
                    Users = Users.OrderBy(s => s.LastName);
                    break;
                case "Roles_desc":
                    Users = Users.OrderByDescending(s => s.Roles.FirstOrDefault().RoleId);
                    break;
                case "Roles":
                    Users = Users.OrderBy(s => s.Roles.FirstOrDefault().RoleId);
                    break;

                case "Group_desc":
                    Users = Users.OrderByDescending(s => s.Group.Name);
                    break;
                case "Group":
                    Users = Users.OrderBy(s => s.Group.Name);
                    break;

                case "Email_desc":
                    Users = Users.OrderByDescending(s => s.Email);
                    break;
                case "Email":
                    Users = Users.OrderBy(s => s.Email);
                    break;
                case "PhoneNumber_desc":
                    Users = Users.OrderByDescending(s => s.PhoneNumber);
                    break;
                case "PhoneNumber":
                    Users = Users.OrderBy(s => s.PhoneNumber);
                    break;
                default:
                    Users = Users.OrderByDescending(s => s.LastName);
                    break;
            }


            var checkBox = Request.Form["ActiveChoise"];
            var applicationUsers = Users;

            if (checkBox == "on")
            {
                applicationUsers = Users.Where(o => o.Group.EndDate > DateTime.Today && o.Group.StartDate < DateTime.Today);
            }

            if (User.IsInRole("Elev"))
           {
               var id = new LMS_Lexicon2015.Models.ApplicationDbContext().Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault().GroupId;
                applicationUsers = applicationUsers.Where(g => g.GroupId == id);
            }

            ViewBag.Roles = db.Roles.ToList();
            FromPartitialView = false;

            //var model = Users.Select(r => new UserListViewModel
            var model = applicationUsers.Select(r => new UserListViewModel
                {
                    Id = r.Id,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Email = r.Email,
                    Role = db.Roles.Where(R => R.Id == r.Roles.FirstOrDefault().RoleId).FirstOrDefault().Name,
                    Group = db.Groups.Where(G => G.Id == r.GroupId).FirstOrDefault().Name,
                    GroupId = db.Groups.Where(G => G.Id == r.GroupId).FirstOrDefault().Id,
                    PhoneNumber = r.PhoneNumber
                }).ToList();

           return View(model);


        }

        // GET: Skaparen sida med de användare som tillhör samma grupp
        //       public ActionResult DelGroup(int id)
        public ActionResult PartitialGroup(int? id)
        {
            ViewBag.Line1 = "/";
            ViewBag.Line2 = "-";

            var model = db.Users.Where(r => r.GroupId == id).ToList();
            if (model.Any())
            {
                ViewBag.GroupName = model.First().Group.Name;
            }
            else ViewBag.GroupName = "Tom Grupp";
            FromPartitialView = true;
            return View(model);
        }

        // GET: Visar en sida med detaljerad information av en användare
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser applicationUser = db.Users.Find(id);

            ViewBag.Role = db.Roles.Find((applicationUser.Roles).First().RoleId).Name;
            ViewBag.GroupName = "Gruppnamn";
            ViewBag.GroupStart = "Startdatum";
            ViewBag.GroupEnd = "Slutdatum";
            ViewBag.RoleHeader = "Roll";
            ViewBag.EmailHeader = "Epost";
            ViewBag.PhoneHeader = "Mobilnummer";
            ViewBag.UserName = "Användarnamn";

            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            return View(applicationUser);
        }

        // GET: Visar en sida från vilken man kan skapa en användare
        [Authorize(Roles = "Lärare")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,GroupId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }
        [Authorize(Roles = "Lärare")] //var används denna action?
        public ActionResult Register()
        {
            return View();
        }

        // GET: Visar en sida från vilken man kan ändra info om en användare
        [Authorize(Roles = "Lärare")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model =
            db.Users.Where(u => u.Id == id).Select(r => new UserListViewModel
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Email = r.Email,
                Role = db.Roles.Where(R => R.Id == r.Roles.FirstOrDefault().RoleId).FirstOrDefault().Name,
                Group = db.Groups.Where(G => G.Id == r.GroupId).FirstOrDefault().Name,
                GroupId = db.Groups.Where(G => G.Id == r.GroupId).FirstOrDefault().Id,
                PhoneNumber = r.PhoneNumber,
                UserName = r.UserName

            }).FirstOrDefault();


            if (model == null)
            {
                return HttpNotFound();
            }

            List<Group> g = db.Groups.ToList();
            g.Insert(0, null);


            //lägg till en gång till "If we got this far, something failed, redisplay form"
            ViewBag.Role = new SelectList(db.Roles, "Name", "Name");//en bäg för rullningslistan på formuläret 
            ViewBag.GroupTeacher = new SelectList(g, "Id", "Name");//en bäg för rullningslistan på formuläret
            ViewBag.GroupId = new SelectList(g, "Id", "Name", model.GroupId);//en bäg för rullningslistan på formuläret
            //return View();


            return View(model);
        }

        //vi har ändratedit pga lössenord ändras vid ändringar

        // POST: Sparar ändringarna ovan
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Lärare")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,GroupId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)

        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,Role,GroupId,PhoneNumber,UserName,GroupStudent,GroupTeacher")] UserListViewModel listUser)
        {

            //var currentUser = applicationUser.Id;
            var userInDb = db.Users.Where(u => u.Id == listUser.Id).FirstOrDefault();

            if (listUser.LastName != userInDb.LastName && !String.IsNullOrEmpty(listUser.LastName))
            {
                userInDb.LastName = listUser.LastName;
            }


            if (listUser.FirstName != userInDb.FirstName && !String.IsNullOrEmpty(listUser.FirstName))
            {
                userInDb.FirstName = listUser.FirstName;
            }


            //lärare kan vara utan grupp 
            //  både före och efter ändring FIXA. GroupId=12, Group=NULL
            if (!listUser.GroupId.HasValue && listUser.Role == "Lärare")
            {
                userInDb.GroupId = listUser.GroupId;
            }

            //userInDb.Group.Name.

            if (listUser.GroupId.HasValue)
            {
                userInDb.GroupId = listUser.GroupId; //testar inte på förändring. Blir ingen skillnad. Mindre krångligt då Lärare byter från ingen grupp till en grupp
                
            }

            if (!listUser.GroupId.HasValue && listUser.Role == "Elev")
            {
                ModelState.AddModelError("", "Du försökte att ta bort grupp från en elev. Elev kan bara byta grupp.");
                //return RedirectToAction("Edit");


                List<Group> g = db.Groups.ToList();
                g.Insert(0, null);


                //lägg till en gång till "If we got this far, something failed, redisplay form"
                ViewBag.Role = new SelectList(db.Roles, "Name", "Name");//en bäg för rullningslistan på formuläret 
                ViewBag.GroupTeacher = new SelectList(g, "Id", "Name");//en bäg för rullningslistan på formuläret
                ViewBag.GroupId = new SelectList(g, "Id", "Name", userInDb.GroupId);//en bäg för rullningslistan på formuläret
                //return View();

                return View(listUser);
                
            }

            if (listUser.Email != userInDb.Email && !String.IsNullOrEmpty(listUser.Email))
            {
                userInDb.Email = listUser.Email;
            }
            if (listUser.PhoneNumber != userInDb.PhoneNumber)
            {
                userInDb.PhoneNumber = listUser.PhoneNumber;
            }


            db.SaveChanges();

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            // i fall det knasar måste nedan finnas så att det visas om
            return RedirectToAction("Edit");


        }

        // GET: Visar en sida från vilken man kan ta bort en användare
        [Authorize(Roles = "Lärare")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupName = "Gruppnamn";
            ViewBag.RoleHeader = "Roll";
            ViewBag.EmailHeader = "Epost";
            ViewBag.PhoneHeader = "Mobilnummer";
            ViewBag.UserName = "Användarnamn";




            return View(applicationUser);
        }

        // POST: Tar bort ovan nämnda användare
        [Authorize(Roles = "Lärare")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(string baseData)
        {
            if (HttpContext.Request.Files.AllKeys.Any())
            {
                for (int i = 0; i <= HttpContext.Request.Files.Count; i++)
                {
                    var uploadFile = HttpContext.Request.Files["files" + i];
                    if (uploadFile != null)
                    {

                        //spara uploadFile i databas
                        // spara GetRandomFileName i databas

                        string fileExtension = uploadFile.FileName.Split('.').Last();
                        var fileName = Path.GetRandomFileName() + '.' + fileExtension;


                        var fileSavePath = Path.Combine(Server.MapPath("/Files"), fileName);

                        uploadFile.SaveAs(fileSavePath);
                    }
                }


            }
            return View();
        }

        public ActionResult Download()
        {
            string[] files = Directory.GetFiles(Server.MapPath("/Files"));
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            ViewBag.Files = files;
            return View();
        }

        public FileResult DownloadFile(string fileName)
        {
            var filepath = System.IO.Path.Combine(Server.MapPath("/Files/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }




    }
}
