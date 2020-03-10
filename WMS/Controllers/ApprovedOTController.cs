using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS.CustomClass;
using WMS.Models;

namespace WMS.Controllers
{
    public class ApprovedOTController : Controller
    {
        private TAS2013Entities db = new TAS2013Entities();

        // GET: /ApprovedOT/
        public ActionResult Index()
        {
            if (Session["EditAttendanceDate"] == null)
            {
                ViewData["datef"] = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                ViewData["datet"] = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            }
            else
            {
                ViewData["datef"] = Session["EditAttendanceDate"].ToString();
                ViewData["datet"] = Session["EditAttendanceDate"].ToString();
            }
            User LoggedInUser = Session["LoggedUser"] as User;
            ViewBag.CompanyID = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);
         
            return View(db.ApprovedOTs.ToList());
        }

        // GET: /ApprovedOT/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApprovedOT approvedot = db.ApprovedOTs.Find(id);
            if (approvedot == null)
            {
                return HttpNotFound();
            }
            return View(approvedot);
        }

        // GET: /ApprovedOT/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ApprovedOT/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="EmpDate,EmpID,Dated,OTMin,ApprovedMin,UserID")] ApprovedOT approvedot)
        {
            if (ModelState.IsValid)
            {
                db.ApprovedOTs.Add(approvedot);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(approvedot);
        }

        // GET: /ApprovedOT/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApprovedOT approvedot = db.ApprovedOTs.Find(id);
            if (approvedot == null)
            {
                return HttpNotFound();
            }
            return View(approvedot);
        }

        // POST: /ApprovedOT/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="EmpDate,EmpID,Dated,OTMin,ApprovedMin,UserID")] ApprovedOT approvedot)
        {
            if (ModelState.IsValid)
            {
                db.Entry(approvedot).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(approvedot);
        }

        // GET: /ApprovedOT/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApprovedOT approvedot = db.ApprovedOTs.Find(id);
            if (approvedot == null)
            {
                return HttpNotFound();
            }
            return View(approvedot);
        }

        // POST: /ApprovedOT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApprovedOT approvedot = db.ApprovedOTs.Find(id);
            db.ApprovedOTs.Remove(approvedot);
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


        public ActionResult SingleEmpEditOT()
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            ViewBag.CompanyID = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);
         
            if (Request.Form["EmpNo"].ToString() != "" && Request.Form["DateFrom"].ToString() != "" && Request.Form["DateTo"].ToString() != "")
            {
                string _EmpNo = Request.Form["EmpNo"].ToString();
                int CompID = Convert.ToInt32(Request.Form["CompanyID"].ToString());
                DateTime _AttDataFrom = Convert.ToDateTime(Request.Form["DateFrom"].ToString());
                DateTime _AttDateTo = Convert.ToDateTime(Request.Form["DateTo"].ToString());
                List<ViewAttData> dailyAttendance = new List<ViewAttData>();
                List<CustomShiftModel> shifts = new List<CustomShiftModel>();
                shifts = GetCustomShifts();
                dailyAttendance = db.ViewAttDatas.Where(aa => aa.EmpNo == _EmpNo && aa.CompanyID == CompID && aa.AttDate >= _AttDataFrom && aa.AttDate <= _AttDateTo).ToList();
                if(dailyAttendance.Count>0)
                {
                    int empid = (int)dailyAttendance.FirstOrDefault().EmpID;
                    List<AttDataAppOT> attDataOTApp = new List<AttDataAppOT>();
                    List<OTApprovalList> vm = new List<OTApprovalList>();
                   // {"Invalid object name 'dbo.AttDataAppOT'."}
                    attDataOTApp = db.AttDataAppOTs.Where(aa => aa.AEmpID == empid && aa.AAttDate >= _AttDataFrom && aa.AAttDate <= _AttDateTo).ToList();
                    int totalNorHours = 0;
                    int totalAppHours = 0;
                    foreach (var att in dailyAttendance)
                    {
                        OTApprovalList v = new OTApprovalList();
                        v.ActualOTHours = ConvertIntoTime(att.OTMin, att.GZOTMin);
                        v.ApprOTHours = GetApprovedOT(attDataOTApp.Where(aa => aa.AEmpDate == att.EmpDate));
                        v.ShiftID = GetShiftID(attDataOTApp.Where(aa => aa.AEmpDate == att.EmpDate),shifts);
                        totalAppHours = totalAppHours + GetApprovedOTMins(attDataOTApp.Where(aa => aa.AEmpDate == att.EmpDate));
                        if (att.OTMin > 0)
                            totalNorHours = (int)(totalNorHours + att.OTMin);
                        if (att.GZOTMin > 0)
                            totalNorHours = (int)(totalNorHours + att.GZOTMin);
                        v.Name = att.EmpName;
                        v.EmpDate = att.EmpDate;
                        v.EmpNo = att.EmpNo;
                        v.DesignationName = att.DesignationName;
                        v.SectionName = att.SectionName;
                        v.AttDate = Convert.ToDateTime(att.AttDate).ToString() ;
                        vm.Add(v);
                    }
                    OTApprovalList ota = new OTApprovalList();
                    ota.ActualOTHours = ((int)(totalNorHours/60)).ToString();
                    ota.ApprOTHours = ((int)(totalAppHours / 60)).ToString();
                    ota.ShiftID = "";
                    ota.Name = "";
                    ota.EmpDate = "";
                    ota.EmpNo = "Total";
                    ota.DesignationName = "";
                    ota.SectionName = "";
                    ota.AttDate = "";
                    vm.Add(ota);

                    VMOTEditor vmOT = new VMOTEditor();
                    vmOT.OTList = vm;
                    vmOT.Shifts = shifts;
                    vmOT.EmpNo = _EmpNo;
                    vmOT.CompanyID = CompID;
                    vmOT.dtFrom = _AttDataFrom;
                    vmOT.dtTo = _AttDateTo;
                    vmOT.Count = vm.Count-1;
                    vmOT.EmpID = empid;
                    return View(vmOT);
                }
               
            }

            return View();
        }

        private string GetShiftID(IEnumerable<AttDataAppOT> list, List<CustomShiftModel> shifts)
        {
            string id = "06-14";
            foreach (var item in list)
            {
                if (item.ShiftName != null)
                {
                    if (shifts.Where(aa => aa.ShiftName == item.ShiftName).Count() > 0)
                    {
                        id = shifts.First(aa => aa.ShiftName == item.ShiftName).ShiftID;
                    }
                }
            }
            return id;
        }

        private List<CustomShiftModel> GetCustomShifts()
        {
            List<CustomShiftModel> csm = new List<CustomShiftModel>();
            CustomShiftModel cm1 = new CustomShiftModel();
            cm1.ShiftID = "A";
            cm1.ShiftName = "A";
            csm.Add(cm1);
            CustomShiftModel cm2 = new CustomShiftModel();
            cm2.ShiftID = "B";
            cm2.ShiftName = "B";
            csm.Add(cm2);
            CustomShiftModel cm3 = new CustomShiftModel();
            cm3.ShiftID = "C";
            cm3.ShiftName = "C";
            csm.Add(cm3);
            return csm;

        }

        private int GetShiftID(IEnumerable<AttDataAppOT> list, List<Shift> shifts, byte? shiftName)
        {
            int id = shifts.FirstOrDefault().ShiftID;
            foreach (var item in list)
            {
                if (item.ShiftName != null)
                {
                    if (shifts.Where(aa => aa.ShiftName == item.ShiftName).Count() > 0)
                    {
                        id = shifts.First(aa => aa.ShiftName == item.ShiftName).ShiftID;
                    }
                    else
                    {
                        id = shifts.First(aa => aa.ShiftID == shiftName).ShiftID;
                    }
                }
                else
                { id = shifts.First(aa => aa.ShiftID == shiftName).ShiftID; }
            }
            if(list.Count()==0)
                id = shifts.First(aa => aa.ShiftID == shiftName).ShiftID;
            return id;
        }
        public ActionResult SaveEmpOT()
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            string _EmpNo = Request.Form["EmpNo"].ToString();
            int CompID = Convert.ToInt32(Request.Form["CompanyID"].ToString());
            DateTime _AttDataFrom = Convert.ToDateTime(Request.Form["DateFrom"].ToString());
            DateTime _AttDateTo = Convert.ToDateTime(Request.Form["DateTo"].ToString());
            int _EmpID = Convert.ToInt32(Request.Form["EmpID"].ToString());
            int count = Convert.ToInt32(Request.Form["Count"].ToString());
            List<AttDataAppOT> attDataOT = new List<AttDataAppOT>();
            List<CustomShiftModel> shifts = GetCustomShifts();
            attDataOT = db.AttDataAppOTs.Where(aa => aa.AEmpID == _EmpID && aa.AAttDate >= _AttDataFrom && aa.AAttDate <= _AttDateTo).ToList();
            for (int i = 0; i < count; i++)
            {
                string empDate = Request.Form["EmpDate" + i.ToString()].ToString();
                string ApprovedOT = Request.Form["ApprovedOT" + i.ToString()].ToString();
                string ActualOT = Request.Form["ActualOT" + i.ToString()].ToString();
                string ShiftID = Request.Form["Shift" + i.ToString()].ToString();
                DateTime date = Convert.ToDateTime(Request.Form["AttDate" + i.ToString()].ToString());
                if (ApprovedOT != "" && ApprovedOT!="0000")
                {
                    if (attDataOT.Where(aa => aa.AEmpDate == empDate).Count() > 0)
                    {
                        AttDataAppOT attOTApp = attDataOT.First(aa => aa.AEmpDate == empDate);
                        attOTApp.ApprovedOT = ConvertTime(ApprovedOT);
                        attOTApp.UserID = LoggedInUser.UserID;
                        attOTApp.ShiftName = shifts.First(aa=>aa.ShiftID==ShiftID).ShiftName;
                        db.SaveChanges();
                    }
                    else
                    {
                        AttDataAppOT attOTApp = new AttDataAppOT();
                        attOTApp.AAttDate = date;
                        attOTApp.AEmpDate = empDate;
                        attOTApp.AEmpID = _EmpID;
                        attOTApp.ApprovedOT = ConvertTime(ApprovedOT);
                        attOTApp.UserID = LoggedInUser.UserID;
                        db.AttDataAppOTs.Add(attOTApp);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (attDataOT.Where(aa => aa.AEmpDate == empDate).Count() > 0)
                    {
                        AttDataAppOT attOTApp = attDataOT.First(aa => aa.AEmpDate == empDate);
                        db.AttDataAppOTs.Remove(attOTApp);
                        db.SaveChanges();
                    }

                    }
            }
            return RedirectToAction("Index");
        }

        public static short ConvertTime(string p)
        {
            try
            {
                string hour = "";
                string min = "";
                int count = 0;
                int chunkSize = 2;
                int stringLength = 4;

                for (int i = 0; i < stringLength; i += chunkSize)
                {
                    count++;
                    if (count == 1)
                    {
                        hour = p.Substring(i, chunkSize);
                    }
                    if (count == 2)
                    {
                        min = p.Substring(i, chunkSize);
                    }
                    if (i + chunkSize > stringLength)
                    {
                        chunkSize = stringLength - i;
                    }
                }
                short _currentTime = (short)(Convert.ToInt32(hour)*60 + Convert.ToInt32(min));
                return _currentTime;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        private string GetApprovedOT(IEnumerable<AttDataAppOT> list)
        {
            string val = "";
            foreach (var item in list)
            {
                if (item.ApprovedOT > 0)
                {
                    int h = (int)(item.ApprovedOT / 60);
                    int m = (int)(item.ApprovedOT - h * 60);
                    val = h.ToString("00") + m.ToString("00");
                }
            }
            return val;
        }
        private int GetApprovedOTMins(IEnumerable<AttDataAppOT> list)
        {
            int val = 0;
            foreach (var item in list)
            {
                if (item.ApprovedOT > 0)
                {
                    val = (int)(val + item.ApprovedOT);
                }
            }
            return val;
        }

        private string ConvertIntoTime(short? NOT, short? GOT)
        {
            string val = "";
            if(NOT>0)
            {
                int h = (int)(NOT / 60);
                int m = (int)(NOT - h * 60);
                val = h.ToString("00") + m.ToString("00");
            }
            if(GOT>0)
            {
                int h = (int)(GOT / 60);
                int m = (int)(GOT - h * 60);
                val = h.ToString("00") + m.ToString("00");

            }
            return val;
        }
       
    }
    public class VMOTEditor
    {
        public List<OTApprovalList> OTList { get; set; }
        public List<CustomShiftModel> Shifts { get; set; }
        public DateTime dtFrom { get; set; }
        public string EmpNo { get; set; }
        public int CompanyID { get; set; }
        public int EmpID { get; set; }
        public DateTime dtTo { get; set; }
        public int Count { get; set; }
    }

    public class CustomShiftModel
    {
        public string ShiftID{ get; set; }
        public string ShiftName { get; set; }
    }
    public class OTApprovalList
    {
        public string EmpDate { get; set; }
        public string AttDate { get; set; }
        public int EmpID { get; set; }
        public string EmpNo { get; set; }
        public string Name { get; set; }
        public string DesignationName { get; set; }
        public string SectionName { get; set; }
        public string ActualOTHours { get; set; }
        public string ShiftID { get; set; }
        public string ApprOTHours { get; set; }
    }
}
