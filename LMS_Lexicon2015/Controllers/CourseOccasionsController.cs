﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS_Lexicon2015.Models;

namespace LMS_Lexicon2015.Controllers
{
    [Authorize]
    public class CourseOccasionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: CourseOccasions
        public ActionResult Index()
        {
            ViewBag.NoActivities = "Inga kurser listas här. De finns under respektive grupp";
            //ErrorMessageStartAfterEnd = false;
            return View(db.CourseOccasions.ToList());
        }

        // GET: Skapar en sida med detaljer för kursen
        public ActionResult Details(int? id, int? id2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseOccasion courseOccasion = db.CourseOccasions.Find(id);
            if (courseOccasion == null)
            {
                return HttpNotFound();
            }
            ViewBag.Line3 = " Till ";
            ViewBag.courseOccasionId = id;
            ViewBag.groupId = id2;
            return View(courseOccasion);
        }

        // GET: Starsidan från vilken man kan skapa en kurs
        [Authorize(Roles = "Lärare")]
        public ActionResult Create(int? id)
        {


                //var model = db.CourseOccasions.Select(c => new CourseOccasionViewModel
                //{
                //Id = c.Id,
                //Name = c.Name,
                //Description = c.Description,
                //StartDate = c.StartDate,
                //EndDate = c.EndDate,
                //GroupStartDate = db.Groups.Where(G => G.Id == c.GroupId).FirstOrDefault().StartDate     
                //}).ToList();


            ViewBag.GroupId = id;
            return View();
        }

        // POST: Skapar kursen ovan
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Lärare")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,GroupId")] CourseOccasion courseOccasion)
        {
            if (ModelState.IsValid)
            {

                DateTime GroupsStartDate = db.Groups.Where(g => g.Id == courseOccasion.GroupId).FirstOrDefault().StartDate;
                DateTime GroupsEndDate = db.Groups.Where(g => g.Id == courseOccasion.GroupId).FirstOrDefault().EndDate;

                if (courseOccasion.StartDate < GroupsStartDate)
                {
                    //AddErrors(ModelState);
                    ViewBag.GroupId = courseOccasion.GroupId;
                    ModelState.AddModelError("", "Du har angivit ett startdatumet före gruppens startdatumet");
                    return View(courseOccasion);
                }

                else if (courseOccasion.EndDate > GroupsEndDate)
                {
                    //AddErrors(ModelState);
                    ViewBag.GroupId = courseOccasion.GroupId;
                    ModelState.AddModelError("", "Du har angivit ett slutdatum efter gruppens slutdatum");
                    return View(courseOccasion);
                }
                
                else if (courseOccasion.StartDate > courseOccasion.EndDate)
                {
                    //AddErrors(ModelState);
                    ViewBag.GroupId = courseOccasion.GroupId;
                    ModelState.AddModelError("", "Du har angivit ett slutdatum före startdatumet ");
                    return View(courseOccasion);
                }

                else
                {
                    db.CourseOccasions.Add(courseOccasion);
                    db.SaveChanges();
                    return RedirectToAction("Details/" + (int)courseOccasion.GroupId, "Groups");
                }

            }
            ViewBag.GroupId = courseOccasion.GroupId;
            return View(courseOccasion);
        }



        // GET: Skapar en sida från vilken man kan ändra kurs data
        [Authorize(Roles = "Lärare")]
        public ActionResult Edit(int? id, int? id2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseOccasion courseOccasion = db.CourseOccasions.Find(id);
            if (courseOccasion == null)
            {
                return HttpNotFound();
            }
            ViewBag.courseOccasionId = id;
            ViewBag.groupId = id2;
    
            return View(courseOccasion);
        }

        // POST: Genomför ändringen angiven ovan
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Lärare")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,GroupId")] CourseOccasion courseOccasion)
        {
            if (ModelState.IsValid)
            {

                DateTime GroupsStartDate = db.Groups.Where(g => g.Id == courseOccasion.GroupId).FirstOrDefault().StartDate;
                DateTime GroupsEndDate = db.Groups.Where(g => g.Id == courseOccasion.GroupId).FirstOrDefault().EndDate;

                if (courseOccasion.StartDate < GroupsStartDate)
                {
                    //AddErrors(ModelState);
                    ViewBag.GroupId = courseOccasion.GroupId;
                    ModelState.AddModelError("", "Du har angivit ett startdatumet före gruppens startdatumet");
                    return View(courseOccasion);
                }

                else if (courseOccasion.EndDate > GroupsEndDate)
                {
                    //AddErrors(ModelState);
                    ViewBag.GroupId = courseOccasion.GroupId;
                    ModelState.AddModelError("", "Du har angivit ett slutdatum efter gruppens slutdatum");
                    return View(courseOccasion);
                }
                else if (courseOccasion.StartDate > courseOccasion.EndDate)
                {
                    //AddErrors(ModelState);
                    ViewBag.GroupId = courseOccasion.GroupId;
                    ModelState.AddModelError("", "Du har angivit ett slutdatum före startdatumet ");
                    return View(courseOccasion);
                }

                else
                {
                    db.Entry(courseOccasion).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details/" + (int)courseOccasion.Id + "/" + (int)courseOccasion.GroupId, "CourseOccasions");
                }
            }
            return View(courseOccasion);
        }

        // GET: Skapar en sida från vilken man kan ta bort en kurs
        [Authorize(Roles = "Lärare")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseOccasion courseOccasion = db.CourseOccasions.Find(id);
            if (courseOccasion == null)
            {
                return HttpNotFound();
            }
            ViewBag.Line1 = "/";
            ViewBag.Line2 = "-";
            ViewBag.Line3 = " Till ";
            return View(courseOccasion);
        }

        [Authorize(Roles = "Lärare")]
        // POST: Tar bort kursen enligt ovan
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseOccasion courseOccasion = db.CourseOccasions.Find(id);
            int GroupNr = (int)courseOccasion.GroupId;
            db.CourseOccasions.Remove(courseOccasion);
            db.SaveChanges();
            return RedirectToAction("Details/" + GroupNr, "Groups");
            //return RedirectToAction("Index");
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
