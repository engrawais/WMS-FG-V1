using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS.Models;
using PagedList;
using System.Linq.Dynamic;
using WMS.Controllers.Filters;
using WMS.CustomClass;

namespace WMS.Controllers
{
    [CustomControllerAttributes]
    public class SectionController : Controller
    {
        private TAS2013Entities db = new TAS2013Entities();

        // GET: /Section/
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
             if (Session["LogedUserFullname"].ToString() != "")
            {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SectionSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            User LoggedInUser = HttpContext.Session["LoggedUser"] as User;
            QueryBuilder qb = new QueryBuilder();
            string query = qb.QueryForCompanyViewForLinq(LoggedInUser);
            var sections = db.Sections.Where(query).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                sections = sections.Where(s => s.SectionName.ToUpper().Contains(searchString.ToUpper())
                    || s.Department.DeptName.ToUpper().Contains(searchString.ToUpper())
                    || s.Company.CompName.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    sections = sections.OrderByDescending(s => s.SectionName);
                    break;
                default:
                    sections = sections.OrderBy(s => s.SectionName);
                    break;
            }
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(sections.ToPagedList(pageNumber, pageSize));
            }
             else
                 return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: /Section/Details/5
        [CustomActionAttribute]
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Section section = db.Sections.Find(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            return View(section);
        }

        // GET: /Section/Create
        [CustomActionAttribute]
        public ActionResult Create()
        {
            ViewBag.DeptID = new SelectList(db.Departments.OrderBy(s=>s.DeptName), "DeptID", "DeptName");
            ViewBag.CompanyID = new SelectList(db.Companies.OrderBy(s=>s.CompName), "CompID", "CompName");
            return View();
        }

        // POST: /Section/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomActionAttribute]
        public ActionResult Create([Bind(Include = "SectionID,SectionName,DeptID,CompanyID")] Section section)
        {
            if (db.Sections.Where(aa => aa.SectionName == section.SectionName && aa.DeptID==section.DeptID && aa.CompanyID == section.CompanyID).Count() > 0)
                ModelState.AddModelError("SectionName", "Name must be unique");
            if (section.CompanyID == null)
                ModelState.AddModelError("CompanyID", "Please selct a Company");
            if (section.SectionName == "")
                ModelState.AddModelError("SectionName", "Please enter Section Name");
            if (section.DeptID == null)
                ModelState.AddModelError("DeptID", "Please select Department");
            if (ModelState.IsValid)
            {
                db.Sections.Add(section);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeptID = new SelectList(db.Departments.OrderBy(s=>s.DeptName), "DeptID", "DeptName", section.DeptID);
            ViewBag.CompanyID = new SelectList(db.Companies.OrderBy(s=>s.CompName), "CompID", "CompName");
            return View(section);
        }

        // GET: /Section/Edit/5
        [CustomActionAttribute]
        public ActionResult Edit(short? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Section section = db.Sections.Find(id);
            ViewBag.DeptID = new SelectList(db.Departments.OrderBy(s=>s.DeptName), "DeptID", "DeptName",section.DeptID);
            ViewBag.CompanyID = new SelectList(db.Companies.OrderBy(s=>s.CompName), "CompID", "CompName",section.CompanyID);
            if (section == null)
            {
                return HttpNotFound();
            }
            
            return View(section);
        }

        // POST: /Section/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomActionAttribute]
        public ActionResult Edit([Bind(Include = "SectionID,SectionName,DeptID,CompanyID")] Section section)
        {
            if (section.CompanyID == null)
                ModelState.AddModelError("CompanyID", "Please selct a Company");
            if (section.SectionName == "")
                ModelState.AddModelError("SectionName", "Please enter Section Name");
            if (section.DeptID == null)
                ModelState.AddModelError("DeptID", "Please select Department");
            if (ModelState.IsValid)
            {
                db.Entry(section).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeptID = new SelectList(db.Departments.OrderBy(s=>s.DeptName), "DeptID", "DeptName", section.DeptID);
            return View(section);
        }

        // GET: /Section/Delete/5
        [CustomActionAttribute]
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Section section = db.Sections.Find(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            return View(section);
        }

        // POST: /Section/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomActionAttribute]
        public ActionResult DeleteConfirmed(short id)
        {
            Section section = db.Sections.Find(id);
            db.Sections.Remove(section);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult DepartmentList(string ID)
        {
            short Code = Convert.ToInt16(ID);
            var secs = db.Departments.Where(aa=>aa.CompanyID==Code);
            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(
                                secs.ToArray(),
                                "DeptID",
                                "DeptName")
                           , JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }
    }
}
