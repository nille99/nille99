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


namespace LMS_Lexicon2015.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        public static bool FromPartitialView;
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

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            var applicationUsers = db.Users.ToList();

            ViewBag.Roles = db.Roles.ToList();
            //var gruppTest =  db.Groups.Find
            FromPartitialView = false;

            var model = db.Users.Select(r => new UserListViewModel
                {
                    Id = r.Id,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Email = r.Email,
                    Role = db.Roles.Where(R => R.Id == r.Roles.FirstOrDefault().RoleId).FirstOrDefault().Name,
                    Group = db.Groups.Where(G => G.Id == r.GroupId).FirstOrDefault().Name,
                    PhoneNumber = r.PhoneNumber
                }).ToList();

            return View(model);

        }

        // GET: Groups
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

        // GET: Users/Details/5
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

        // GET: Users/Create
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

        // GET: Users/Edit/5
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
                PhoneNumber = r.PhoneNumber,
                UserName = r.UserName

            }).FirstOrDefault();

           // if (String.IsNullOrEmpty(model.Group)) model.Group = "c"; //test ck

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        //vi har ändratedit pga lössenord ändras vid ändringar

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Lärare")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,GroupId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)

        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,Role,Group,PhoneNumber,UserName")] UserListViewModel listUser)
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
            //  både före och efter ändring
            //if (String.IsNullOrEmpty(listUser.Group) && (listUser.Role == "Lärare"))
            //{
            //    userInDb.Group.Name = listUser.Group;
            //}

            //userInDb.Group.Name.

            if (!String.IsNullOrEmpty(listUser.Group))
            {
             //   userInDb.Group = listUser.GroupId; //testar inte på förändring. Blir ingen skillnad. Mindre krångligt då Lärare byter från ingen grupp till en grupp
                
            }

            if (String.IsNullOrEmpty(listUser.Group) && (listUser.Role == "Elev"))
            {
                //error
            }

            if (listUser.Email != userInDb.Email && !String.IsNullOrEmpty(listUser.Email))
            {
                userInDb.Email = listUser.Email;
            }
            if (listUser.PhoneNumber != userInDb.PhoneNumber)
            {
                userInDb.PhoneNumber = listUser.PhoneNumber;
            }


            //    // other changed properties
            db.SaveChanges();

            //db.Users.Where(r => r.GroupId == id).First().Group.Name;

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            //            return View(applicationUser);
            // i fall det knasar måste nedan finnas så att det visas om

            return RedirectToAction("Edit");


        }

        // GET: Users/Delete/5
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
            return View(applicationUser);
        }

        // POST: Users/Delete/5
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
    }
}
