using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class EmailController : Controller
    {
        TAS2013Entities db = new TAS2013Entities();
        // GET: Email
        public ActionResult Index()
        {
            List<VMPVIndex> vmPvIndexes = new List<VMPVIndex>();
            foreach (var item in db.EmailFormChilds.ToList())
            {
                VMPVIndex vmPVIndex = new VMPVIndex();
                vmPVIndex.FilterTypeName = item.FilterTypeID;
                vmPVIndex.PID = item.PEmailFormChildID;
                switch (item.FilterTypeID)
                {
                    case "Location":
                        var locs = db.Locations.Where(aa => aa.LocID == item.FilterValueID).ToList();
                        vmPVIndex.FilterValueName = locs.First().LocName;
                        break;
                    case "Section":
                        var secs = db.ViewSections.Where(aa => aa.SectionID == item.FilterValueID).ToList();
                        vmPVIndex.FilterValueName = secs.First().SectionName + "/" + secs.First().DeptName + "/" + secs.First().CompName;
                        break;
                    case "Department":
                        var depts = db.ViewDepartments.Where(aa => aa.DeptID == item.FilterValueID).ToList();
                        vmPVIndex.FilterValueName = depts.First().DeptName + "/" + depts.First().CompName;
                        break;
                    case "Type":
                        var types = db.ViewEmpTypes.Where(aa => aa.TypeID == item.FilterValueID).ToList();
                        vmPVIndex.FilterValueName = types.First().TypeName + types.First().CompName;
                        break;
                    case "Category":
                        vmPVIndex.FilterValueName = db.Categories.Where(aa => aa.CatID == item.FilterValueID).First().CatName;
                        break;
                    case "Company":
                        vmPVIndex.FilterValueName = db.Companies.Where(aa => aa.CompID == item.FilterValueID).First().CompName;
                        break;
                    case "Self":
                        var emps = db.Emps.Where(aa => aa.EmpID == item.FilterValueID).ToList();
                        vmPVIndex.FilterValueName = emps.First().EmpNo + "/" + emps.First().EmpName;
                        break;
                }
                vmPvIndexes.Add(vmPVIndex);
            }
            return View(vmPvIndexes);
        }
        //public ActionResult IndexList()
        //{
        //    List<VMPVIndex> vmodel = new List<VMPVIndex>();
        //    VMPVIndex vm = new VMPVIndex();
        //    List<EmailFormChild> em = db.EmailFormChilds.ToList();
        //    vm.FilterTypeName = em.FirstOrDefault().FilterTypeID;
        //    vm.FilterValueName = em.FirstOrDefault().FilterValueID.ToString();
        //    vmodel.Add(vm);
        //    return View(vmodel);
        //}
        [HttpGet]
        public ActionResult EmailParent(int? pID)
        {

            VMEmailParent pvModel = new VMEmailParent();
            if(pID>0)
            {
                pvModel.PID = (int)pID;
                EmailFormParent emailParent = db.EmailFormParents.Where(aa => aa.PEmailFormParentID == pID).First();
                pvModel.IsPreviousDate = (bool)emailParent.IsPreviousDate;
                pvModel.ToEmail = emailParent.ToEmail;
                pvModel.CCEmail = emailParent.CcEmail;
            }
            return View(pvModel);
        }
        [HttpPost]
        public ActionResult EmailParent(VMEmailParent email)
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            EmailFormParent ep = new EmailFormParent();
            ep.ToEmail = email.ToEmail;
            ep.CcEmail = email.CCEmail;
            ep.CreatedBy = LoggedInUser.UserID;
            ep.CreatedDate = DateTime.Now;
            ep.IsPreviousDate = email.IsPreviousDate;
            ep.Status = true;
            db.EmailFormParents.Add(ep);
            db.SaveChanges();
            return Json(ep.PEmailFormParentID.ToString(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChildList(int? parentID)
        {
            List<VMPVIndex> vmPvIndexes = new List<VMPVIndex>();
            ViewBag.PID = parentID;
            foreach (var item in db.EmailFormChilds.Where(aa => aa.EmailFormParentID == parentID).ToList())
            {
                VMPVIndex vmPVIndex = new VMPVIndex();
                vmPVIndex.FilterTypeName = item.FilterTypeID;
                vmPVIndex.PID = item.PEmailFormChildID;
                switch (item.FilterTypeID)
                {
                    case "Location":
                        var locs = db.Locations.Where(aa => aa.LocID == item.FilterValueID).ToList();
                        vmPVIndex.FilterValueName = locs.First().LocName;
                        break;
                    case "Section":
                        var secs = db.ViewSections.Where(aa => aa.SectionID == item.FilterValueID).ToList();
                        vmPVIndex.FilterValueName = secs.First().SectionName+"/"+ secs.First().DeptName+"/"+secs.First().CompName;
                        break;
                    case "Department":
                        var depts = db.ViewDepartments.Where(aa => aa.DeptID == item.FilterValueID).ToList();
                        vmPVIndex.FilterValueName = depts.First().DeptName+"/"+ depts.First().CompName;
                        break;
                    case "Type":
                        var types = db.ViewEmpTypes.Where(aa => aa.TypeID == item.FilterValueID).ToList();
                        vmPVIndex.FilterValueName = types.First().TypeName+ types.First().CompName;
                        break;
                    case "Category":
                        vmPVIndex.FilterValueName = db.Categories.Where(aa => aa.CatID == item.FilterValueID).First().CatName;
                        break;
                    case "Company":
                        vmPVIndex.FilterValueName = db.Companies.Where(aa => aa.CompID == item.FilterValueID).First().CompName;
                        break;
                    case "Self":
                        var emps = db.Emps.Where(aa => aa.EmpID == item.FilterValueID).ToList();
                        vmPVIndex.FilterValueName = emps.First().EmpNo + "/" + emps.First().EmpName;
                        break;
                }
                vmPvIndexes.Add(vmPVIndex);
            }
            return View(vmPvIndexes);
        }
        [HttpGet]
        public ActionResult PVModel(int? pID)
        {
            ViewBag.LocID = new SelectList(db.Locations.OrderBy(s => s.LocName), "LocID", "LocName");
            ViewBag.SecID = new SelectList(ConvertSections(db.ViewSections.OrderBy(s => s.SectionName)), "SectionID", "SectionName");
            ViewBag.DeptID = new SelectList(ConvertDepartments(db.ViewDepartments.OrderBy(s => s.DeptName)), "DeptID", "DeptName");
            ViewBag.TypeID = new SelectList(ConvertType(db.ViewEmpTypes.OrderBy(s => s.TypeName)), "TypeID", "TypeName");
            ViewBag.CatID = new SelectList(db.Categories.OrderBy(s => s.CatName), "CatID", "CatName");
            ViewBag.CompanyID = new SelectList(db.Companies.OrderBy(s => s.CompName), "CompID", "CompName");
            PVModel pvModel = new PVModel();
            pvModel.EmailFormParentID = (int)pID;
            return View(pvModel);
        }
        public ActionResult Delete(short? id)
        {
            EmailFormChild emailFormChild = db.EmailFormChilds.Find(id);
            db.EmailFormChilds.Remove(emailFormChild);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult PVModel(PVModel item)
        {
            EmailFormChild emailFormChild = new EmailFormChild();
            emailFormChild.EmailFormParentID = item.EmailFormParentID;
            emailFormChild.FilterTypeID = item.FilterTypeID;
            
            switch (item.FilterTypeID)
            {
                case "Location":
                    emailFormChild.FilterValueID = item.LocID;
                    break;
                case "Section":
                    emailFormChild.FilterValueID = item.SecID;
                    break;
                case "Department":
                    emailFormChild.FilterValueID = item.DeptID;
                    break;
                case "Type":
                    emailFormChild.FilterValueID = item.TypeID;
                    break;
                case "Category":
                    emailFormChild.FilterValueID = item.CatID;
                    break;
                case "Company":
                    emailFormChild.FilterValueID = item.CompanyID;
                    break;
                case "Self":
                    emailFormChild.FilterValueID = item.EmpID;
                    break;
            }
            db.EmailFormChilds.Add(emailFormChild);
            db.SaveChanges();
            return RedirectToAction("EmailParent", new { pID = item.EmailFormParentID });
        }
        private IEnumerable ConvertType(IOrderedQueryable<ViewEmpType> orderedQueryable)
        {
            foreach (var item in orderedQueryable)
            {
                item.TypeName = item.TypeName + "/" + item.CompName;
            }
            return orderedQueryable;
        }
        private IEnumerable ConvertDepartments(IOrderedQueryable<ViewDepartment> orderedQueryable)
        {
            foreach (var item in orderedQueryable)
            {
                item.DeptName = item.DeptName + "/" + item.CompName;
            }
            return orderedQueryable;
        }
        private IEnumerable ConvertSections(IOrderedQueryable<ViewSection> orderedQueryable)
        {
            foreach (var item in orderedQueryable)
            {
                item.SectionName = item.SectionName + "/" + item.DeptName + "/" + item.CompName;
            }
            return orderedQueryable;
        }
    }
    public class VMEmailParent
    {
        public int PID { get; set; }
        public string ToEmail { get; set; }
        public string CCEmail { get; set; }
        public bool IsPreviousDate { get; set; }
        public string Status { get; set; }
    }
    public class PVModel
    {
        public int EmailFormParentID { get; set; }
        public string FilterTypeID { get; set; }
        public int LocID { get; set; }
        public int SecID { get; set; }
        public int DeptID { get; set; }
        public int CatID { get; set; }
        public int TypeID { get; set; }
        public int CompanyID { get; set; }
        public int EmpID { get; set; }
    }
    public class VMPVIndex
    {
        public int PID { get; set; }
        public string FilterTypeName { get; set; }
        public string FilterValueName { get; set; }
    }
}