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

namespace WMS.Controllers
{
    public class EmailFormController : Controller
    {
        private TAS2013Entities db = new TAS2013Entities();

        // GET: /EmailForm/
        public ActionResult Index()
        {
            var emailentryforms = db.EmailEntryForms.Include(e => e.EmpType).Include(e => e.Company).Include(e => e.Department).Include(e => e.Location).Include(e => e.Section);
            return View(emailentryforms.ToList());
        }

        // GET: /EmailForm/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailEntryForm emailentryform = db.EmailEntryForms.Find(id);
            if (emailentryform == null)
            {
                return HttpNotFound();
            }
            return View(emailentryform);
        }
        public ActionResult EmailSent(DateTime? dts)
        {
            if(dts==null)
                dts = DateTime.Today;
            List<ViewEmailSent> list = db.ViewEmailSents.Where(aa => aa.AttDate == dts).ToList();
            ViewBag.DTS = dts.Value.ToString("dd-MM-yyyy");
            return View(list);
        }
        // GET: /EmailForm/Create
        public ActionResult Create()
        {
            ViewBag.TypID = new SelectList(db.EmpTypes, "TypeID", "TypeName");
            ViewBag.CompanyID = new SelectList(db.Companies, "CompID", "CompName");
            ViewBag.DepartmentID = new SelectList(db.Departments, "DeptID", "DeptName");
            ViewBag.LocationID = new SelectList(db.Locations, "LocID", "LocName");
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName");
            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName");
            ViewBag.CriteriaComp = new SelectList(GetCriteriaComp(), "ID", "Name");
            ViewBag.CriteriaDepSec = new SelectList(GetCriteriaDeptSec(), "ID", "Name");
            ViewBag.HasTypeOrCat = new SelectList(GetCriteriaTypeCat(), "ID", "Name");
            return View();
        }

        private List<DPValue> GetCriteriaComp()
        {
            List<DPValue> dpvs = new List<DPValue>();
            DPValue dpv = new DPValue();
            dpv.ID = "C";
            dpv.Name = "Company";
            dpvs.Add(dpv);
            DPValue dpva = new DPValue();
            dpva.ID = "L";
            dpva.Name = "Location";
            dpvs.Add(dpva);

            return dpvs;

        }
        private List<DPValue> GetCriteriaDeptSec()
        {
            List<DPValue> dpvs = new List<DPValue>();
            DPValue dpv = new DPValue();
            dpv.ID = "S";
            dpv.Name = "Section";
            dpvs.Add(dpv);
            DPValue dpva = new DPValue();
            dpva.ID = "D";
            dpva.Name = "Department";
            dpvs.Add(dpva);

            DPValue dpvaa = new DPValue();
            dpvaa.ID = "A";
            dpvaa.Name = "All";
            dpvs.Add(dpvaa);

            return dpvs;

        }
        private List<DPValue> GetCriteriaTypeCat()
        {
            List<DPValue> dpvs = new List<DPValue>();
            DPValue dpv = new DPValue();
            dpv.ID = "T";
            dpv.Name = "Type";
            dpvs.Add(dpv);
            DPValue dpva = new DPValue();
            dpva.ID = "C";
            dpva.Name = "Category";
            dpvs.Add(dpva);

            DPValue dpvaa = new DPValue();
            dpvaa.ID = "A";
            dpvaa.Name = "All";
            dpvs.Add(dpvaa);

            return dpvs;

        }
        public class DPValue
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }
        // POST: /EmailForm/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EmailAddress,CCAddress,CompanyID,DepartmentID,SectionID,CriteriaDepSec,ReportCurrentDate,LocationID,TypID,HasTypeOrCat,HasLoc,CatID")] EmailEntryForm emailentryform)
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            string compval = Request.Form["CriteriaComp"].ToString();
            emailentryform.CriteriaComLoc = compval;
            //string depval = Request.Form["Criteria"].ToString();
            //emailentryform.CriteriaDepSec = depval;
            if (ModelState.IsValid)
            {
                emailentryform.UserID = LoggedInUser.UserID;
                db.EmailEntryForms.Add(emailentryform);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TypID = new SelectList(db.EmpTypes, "TypeID", "TypeName");
            ViewBag.CompanyID = new SelectList(db.Companies, "CompID", "CompName", emailentryform.CompanyID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DeptID", "DeptName", emailentryform.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocID", "LocName", emailentryform.LocationID);
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", emailentryform.SectionID);
            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName", emailentryform.CatID);
            ViewBag.CriteriaComp = new SelectList(GetCriteriaComp(), "ID", "Name",emailentryform.CriteriaComLoc);
            ViewBag.CriteriaDepSec = new SelectList(GetCriteriaDeptSec(), "ID", "Name", emailentryform.CriteriaDepSec);
            ViewBag.HasTypeOrCat = new SelectList(GetCriteriaTypeCat(), "ID", "Name",emailentryform.HasTypeOrCat);
            return View(emailentryform);
        }

        // GET: /EmailForm/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailEntryForm emailentryform = db.EmailEntryForms.AsNoTracking().Where(aa=>aa.ID==id).First();
            if (emailentryform == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypID = new SelectList(db.EmpTypes, "TypeID", "TypeName");
            ViewBag.CompanyID = new SelectList(db.Companies, "CompID", "CompName", emailentryform.CompanyID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DeptID", "DeptName", emailentryform.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocID", "LocName", emailentryform.LocationID);
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", emailentryform.SectionID);
            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName", emailentryform.CatID); ViewBag.CriteriaComp = new SelectList(GetCriteriaComp(), "ID", "Name", emailentryform.CriteriaComLoc);
            ViewBag.CriteriaDepSec = new SelectList(GetCriteriaDeptSec(), "ID", "Name", emailentryform.CriteriaDepSec);
            ViewBag.HasTypeOrCat = new SelectList(GetCriteriaTypeCat(), "ID", "Name", emailentryform.HasTypeOrCat);
            return View(emailentryform);
        }

        // POST: /EmailForm/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EmailAddress,CCAddress,CompanyID,DepartmentID,SectionID,CriteriaDepSec,ReportCurrentDate,LocationID,TypID,HasTypeOrCat,HasLoc,CatID,UserID")] EmailEntryForm emailentryform)
        {
            //if (Request.Form["CriteriaComp"].ToString() == "C")
            //string compval = Request.Form["CriteriaComp"].ToString();
            //emailentryform.CriteriaComLoc = compval;
                
            //else
            //    emailentryform.CriteriaComLoc = compval;
            if (ModelState.IsValid)
            {
                EmailEntryForm _emailEmtryForm= db.EmailEntryForms.Where(aa => aa.ID == emailentryform.ID).First();
                _emailEmtryForm.EmailAddress = emailentryform.EmailAddress;
                _emailEmtryForm.CCAddress = emailentryform.CCAddress;
                db.Entry(_emailEmtryForm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypID = new SelectList(db.EmpTypes, "TypeID", "TypeName");
            ViewBag.CompanyID = new SelectList(db.Companies, "CompID", "CompName", emailentryform.CompanyID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DeptID", "DeptName", emailentryform.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocID", "LocName", emailentryform.LocationID);
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", emailentryform.SectionID);
            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName", emailentryform.CatID); ViewBag.CriteriaComp = new SelectList(GetCriteriaComp(), "ID", "Name", emailentryform.CriteriaComLoc);
            ViewBag.CriteriaDepSec = new SelectList(GetCriteriaDeptSec(), "ID", "Name", emailentryform.CriteriaDepSec);
            ViewBag.HasTypeOrCat = new SelectList(GetCriteriaTypeCat(), "ID", "Name", emailentryform.HasTypeOrCat);
            return View(emailentryform);
        }
        // GET: /EmailForm/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailEntryForm emailentryform = db.EmailEntryForms.Find(id);
            if (emailentryform == null)
            {
                return HttpNotFound();
            }
            return View(emailentryform);
        }
        public ActionResult ViewEmail(DateTime date)
        {
            List<ViewEmailSent> email = new List<ViewEmailSent>();
            email = db.ViewEmailSents.Where(aa => aa.AttDate == date).ToList();
            return View(email);
        }
        // POST: /EmailForm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmailEntryForm emailentryform = db.EmailEntryForms.Find(id);
            db.EmailEntryForms.Remove(emailentryform);
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
        public ActionResult GetDepartment(string ID)
        {
            short Code = Convert.ToInt16(ID);
            var secs = db.Departments.Where(aa=>aa.CompanyID==Code).OrderBy(s=>s.DeptName);
            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(
                                secs.ToArray(),
                                "DeptID",
                                "DeptName")
                           , JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }
        public ActionResult GetSection(string ID)
        {
            short Code = Convert.ToInt16(ID);
            var secs = db.Sections.Where(aa => aa.CompanyID == Code).OrderBy(s => s.SectionName);
            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(
                                secs.ToArray(),
                                "SectionID",
                                "SectionName")
                           , JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }
        public ActionResult GetEmpType(string ID)
        {
            short Code = Convert.ToInt16(ID);
            var secs = db.EmpTypes.Where(aa => aa.CompanyID == Code).OrderBy(s => s.TypeName);
            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(
                                secs.ToArray(),
                                "TypeID",
                                "TypeName")
                           , JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }
        public ActionResult GetCategory(string ID)
        {
            short Code = Convert.ToInt16(ID);
            var secs = db.Categories;
            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(
                                secs.ToArray(),
                                "CatID",
                                "CatName")
                           , JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailEntryForm emailentryform = db.EmailEntryForms.Find(id);
            List<EmpView> empViews = new List<EmpView>();
            empViews = db.EmpViews.Where(aa => aa.Status==true).ToList();
            switch (emailentryform.CriteriaComLoc)
            {
                case "C":
                    empViews = empViews.Where(aa => aa.CompanyID == emailentryform.CompanyID).ToList();
                    break;
                case "L":
                    empViews = empViews.Where(aa => aa.LocID == emailentryform.LocationID).ToList();
                    break;
            }
            switch (emailentryform.CriteriaDepSec)
            {
                case "S":
                    empViews = empViews.Where(aa => aa.SecID == emailentryform.SectionID).ToList();
                    break;
                case "D":
                    empViews = empViews.Where(aa => aa.DeptID == emailentryform.DepartmentID).ToList();
                    break;
                case "A":
                    break;
            }
            switch (emailentryform.HasTypeOrCat)
            {
                case "C":
                    empViews = empViews.Where(aa => aa.CatID == emailentryform.CatID).ToList();
                    break;
                case "T":
                    empViews = empViews.Where(aa => aa.TypeID == emailentryform.TypID).ToList();
                    break;
                case "A":
                    break;
            }
            if (emailentryform == null)
            {
                return HttpNotFound();
            }
            int pageSize = 500;
            return View(empViews.ToPagedList(1, pageSize));
        }
        
    }
}
