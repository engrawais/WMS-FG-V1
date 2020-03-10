using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class EditorController : Controller
    {
        private TAS2013Entities db = new TAS2013Entities();

        // GET: /Editor/
        public ActionResult Index()
        {
             TAS2013Entities db = new TAS2013Entities();
            if (Session["EditAttendanceDate"] == null)
            {
                ViewData["datef"] = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            }
            else
            {
                ViewData["datef"] = Session["EditAttendanceDate"].ToString();
            }
            User LoggedInUser = Session["LoggedUser"] as User;
            ViewData["JobDateFrom"] = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            ViewData["JobDateTo"] = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            ViewBag.JobCardType = new SelectList(db.JobCards, "WorkCardID", "WorkCardName");
            ViewBag.ShiftList = new SelectList(db.Shifts, "ShiftID", "ShiftName");

            // string _EmpNo = Request.Form["EmpNo"].ToString();
            ViewBag.CompanyID = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);
            ViewBag.CompanyIDJobCard = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);
            ViewBag.CrewList = new SelectList(db.Crews, "CrewID", "CrewName");
            ViewBag.SectionList = new SelectList(db.Sections, "SectionID", "SectionName");
            ViewBag.DesignationID = new SelectList(db.Designations.Where(aa => aa.CompanyID == LoggedInUser.CompanyID), "DesignationID", "DesignationName");
            ViewBag.Message = "";
            return View();
        }
        //Load Attendance Details of Selected Employee
        [HttpPost]
        
        public ActionResult EditorOT(FormCollection form)
        {
            TAS2013Entities db = new TAS2013Entities();
            try
            {
                User LoggedInUser = Session["LoggedUser"] as User;
               ViewBag.CompanyID = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);
                ViewData["datef"] = Convert.ToDateTime(Request.Form["DateFrom"].ToString()).ToString("yyyy-MM-dd");
                //ViewData["datef"] = Request.Form["DateFrom"].ToString();
                if (Request.Form["EmpNo"].ToString() != "" && Request.Form["DateFrom"].ToString() != "")
                {
                    string _EmpNo = Request.Form["EmpNo"].ToString();
                    DateTime _AttDataFrom = Convert.ToDateTime(Request.Form["DateFrom"].ToString());
                    Session["EditAttendanceDate"] = Request.Form["DateFrom"].ToString();
                    var _CompId = Request.Form["CompanyID"];
                    int compID = Convert.ToInt32(_CompId);
                    ViewAttDataOT _attData = new ViewAttDataOT();
                    List<EmpView> _Emp = new List<EmpView>();
                    int EmpID = 0;
                    _Emp = db.EmpViews.Where(aa => aa.EmpNo == _EmpNo && aa.CompanyID == compID && aa.Status == true).ToList();
                    if (_Emp.Count > 0)
                    {
                        EmpID = _Emp.FirstOrDefault().EmpID;
                        if (db.AttDataOTs.Where(aa => aa.EmpID == EmpID && aa.AttDate == _AttDataFrom).Count() > 0)
                            _attData = db.ViewAttDataOTs.First(aa => aa.EmpID == EmpID && aa.AttDate == _AttDataFrom);
                    }
                    _attData.EmpID = EmpID;
                    _attData.EmpNo = _Emp.FirstOrDefault().EmpNo;
                    _attData.AttDate = _AttDataFrom;
                    _attData.EmpName = _Emp.FirstOrDefault().EmpName;
                    _attData.FatherName = _Emp.FirstOrDefault().FatherName;
                    _attData.SectionName = _Emp.FirstOrDefault().SectionName;
                    _attData.DeptName = _Emp.FirstOrDefault().DeptName;
                    _attData.DesignationName = _Emp.FirstOrDefault().DesignationName;   
                    if (_attData.EmpNo!=null)
                    {
                        List<PollData> _Polls = new List<PollData>();
                        string _EmpDate = _attData.EmpID.ToString() + _AttDataFrom.Date.ToString("yyMMdd");
                        _Polls = db.PollDatas.Where(aa => aa.EntDate == _AttDataFrom && aa.EmpID == _attData.EmpID).OrderBy(a => a.EntTime).ToList();
                        ViewBag.PollsDataIn = _Polls.Where(aa => aa.Reader.OnlyOT == true && aa.RdrDuty == 1);
                        ViewBag.PollsDataOut = _Polls.Where(aa => aa.Reader.OnlyOT == true && aa.RdrDuty == 1);
                        ViewBag.EmpID = new SelectList(db.Emps.OrderBy(s => s.EmpName), "EmpID", "EmpNo", _attData.EmpID);
                        Session["NEmpNo"] = _attData.EmpID;
                        ViewBag.SucessMessage = "";
                        return View(_attData);
                    }
                    else
                    {
                                                                 
                        return View(_attData);
                    }
                }
                else
                    return View("Index");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Sequence"))
                    ViewBag.Message = "No Entry found on this particular date";
                return View("Index");

            }

        }
        public ActionResult EditorOTNew([Bind(Include = "EmpDate,AttDate,EmpNo,EmpID,Tin0,Tout0,Tin1,Tout1,Tin2,Tout2,Tin3,Tout3")] AttDataOT _attDataOT, FormCollection from)
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            try
            {
                AttDataOT attot = new AttDataOT();
                string empdate = Request.Form["EmpDate"].ToString();
                int empID = Convert.ToInt32(Request.Form["EmpID"].ToString());
                DateTime _AttDataFrom = Convert.ToDateTime(Request.Form["AttDate"].ToString());
                string Tin1 = Request.Form["Tin1"].ToString();
                string Tout1 = Request.Form["Tout1"].ToString();
                string Tin2 = Request.Form["Tin2"].ToString();
                string Tout2 = Request.Form["Tout2"].ToString();
                string Tin3 = Request.Form["Tin3"].ToString();
                string Tout3 = Request.Form["Tout3"].ToString();
                string Remarks = Request.Form["Remarks"].ToString();
                TimeSpan ts = new TimeSpan();
                bool change = false;
                if (db.AttDataOTs.Where(aa => aa.EmpDate == empdate).Count() > 0)
                {
                    attot = db.AttDataOTs.First(aa => aa.EmpDate == empdate);
                }
                if (Tin1 != "0000" && Tout1 != "0000")
                {
                    string Tin1H = Tin1.Substring(0, 2);
                    string Tin1M = Tin1.Substring(2, 2);
                    string Tout1H = Tout1.Substring(0, 2);
                    string Tout1M = Tout1.Substring(2, 2);
                    TimeSpan _TimeIn1 = new TimeSpan(Convert.ToInt16(Tin1H), Convert.ToInt16(Tin1M), 0);
                    TimeSpan _TimeOut1 = new TimeSpan(Convert.ToInt16(Tout1H), Convert.ToInt16(Tout1M), 0);
                    ts = ts + _TimeOut1 - _TimeIn1; 
                    if (_TimeOut1 < _TimeIn1)
                    {
                        attot.ETout1 = _AttDataFrom.AddDays(1) + _TimeOut1;
                        attot.ETin1 = _AttDataFrom + _TimeIn1;
                    }
                    else
                    {
                        attot.ETout1 = _AttDataFrom + _TimeOut1;
                        attot.ETin1 = _AttDataFrom + _TimeIn1;
                    }
                    
                    change = true;
                }
                if (Tin2 != "0000" && Tout2 != "0000")
                {
                    string Tin2H = Tin2.Substring(0, 2);
                    string Tin2M = Tin2.Substring(2, 2);
                    string Tout2H = Tout2.Substring(0, 2);
                    string Tout2M = Tout2.Substring(2, 2);
                    TimeSpan _TimeIn2 = new TimeSpan(Convert.ToInt16(Tin2H), Convert.ToInt16(Tin2M), 0);
                    TimeSpan _TimeOut2 = new TimeSpan(Convert.ToInt16(Tout2H), Convert.ToInt16(Tout2M), 0);
                    ts = ts + _TimeOut2 - _TimeIn2;
                    change = true;
                    if (_TimeOut2 < _TimeIn2)
                    {
                        attot.ETout2 = _AttDataFrom.AddDays(1) + _TimeOut2;
                        attot.ETin2 = _AttDataFrom + _TimeIn2;
                    }
                    else
                    {
                        attot.ETout2 = _AttDataFrom + _TimeOut2;
                        attot.ETin2 = _AttDataFrom + _TimeIn2;
                    }
                }
                if (Tin3 != "0000" && Tout3 != "0000")
                {
                    string Tin3H = Tin3.Substring(0, 2);
                    string Tin3M = Tin3.Substring(2, 2);
                    string Tout3H = Tout3.Substring(0, 2);
                    string Tout3M = Tout3.Substring(2, 2);
                    TimeSpan _TimeIn3 = new TimeSpan(Convert.ToInt16(Tin3H), Convert.ToInt16(Tin3M), 0);
                    TimeSpan _TimeOut3 = new TimeSpan(Convert.ToInt16(Tout3H), Convert.ToInt16(Tout3M), 0);
                    ts = ts + _TimeOut3 - _TimeIn3;
                    change = true;
                    if (_TimeOut3 < _TimeIn3)
                    {
                        attot.ETout3 = _AttDataFrom.AddDays(1) + _TimeOut3;
                        attot.ETin3 = _AttDataFrom + _TimeIn3;
                    }
                    else
                    {
                        attot.ETout3 = _AttDataFrom + _TimeOut3;
                        attot.ETin3= _AttDataFrom + _TimeIn3;
                    }
                }
                if (change==true)
                {
                    attot.AttDate = _AttDataFrom;
                    attot.EmpDate = empdate;
                    attot.EmpID = empID;
                    attot.Mins = (Int16)ts.TotalMinutes;
                    if (db.AttDataOTs.Where(aa => aa.EmpDate == empdate).Count() == 0)
                    {
                        db.AttDataOTs.Add(attot);
                    }
                    attot.Remarks = Remarks;
                    db.SaveChanges();

                    AttData attData = db.AttDatas.First(aa => aa.EmpDate == empdate);
                    if (attData.DutyCode == "G")
                        attData.GZOTMin = attot.Mins;
                    else
                        attData.OTMin = attot.Mins;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                //ViewBag.SucessMessage = "An error occured while saving Entry";
                //_attData = db.AttDatas.First(aa => aa.EmpDate == _EmpDate);
                //List<PollData> _Polls = new List<PollData>();
                //_Polls = db.PollDatas.Where(aa => aa.EmpDate == _EmpDate).OrderBy(a => a.EntTime).ToList();
                //ViewBag.PollsDataIn = _Polls.Where(aa => aa.RdrDuty == 1);
                //ViewBag.PollsDataOut = _Polls.Where(aa => aa.RdrDuty == 5);
                //return View(_attData);
            }
            return View();
        }

        private bool TimeValid(string Tin1, string Tout1)
        {
            if (Tin1.Count() == 4 && Tout1.Count() == 4)
            {
                return true;
            }
            else
                return false;
        }
        //Add New Times and Process Attendance of Particular Employee
        // GET: /Editor/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewAttDataOT viewattdataot = db.ViewAttDataOTs.Find(id);
            if (viewattdataot == null)
            {
                return HttpNotFound();
            }
            return View(viewattdataot);
        }

        // GET: /Editor/Create
        public ActionResult Create()
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            ViewBag.CompName = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);
            return View();
        }

        // POST: /Editor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmpDate,AttDate,Tin1,Tout1,Tin2,Tout2,Tin3,Tout3,Mins,EmpNo,EmpName,Expr1,CompName,EmpID")] ViewAttDataOT viewattdataot)
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            if (viewattdataot.CompName == null)
            {
                ModelState.AddModelError("CompName", "This Company Name is not existing");
            }
            if (ModelState.IsValid)
            {
                db.ViewAttDataOTs.Add(viewattdataot);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompName = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);
             return View(viewattdataot);
        }

        // GET: /Editor/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewAttDataOT viewattdataot = db.ViewAttDataOTs.Find(id);
            if (viewattdataot == null)
            {
                return HttpNotFound();
            }
            return View(viewattdataot);
        }

        // POST: /Editor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="EmpDate,AttDate,Tin1,Tout1,Tin2,Tout2,Tin3,Tout3,Mins,EmpNo,EmpName,Expr1,CompName,EmpID")] ViewAttDataOT viewattdataot)
        {
            if (ModelState.IsValid)
            {
                db.Entry(viewattdataot).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(viewattdataot);
        }

        // GET: /Editor/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewAttDataOT viewattdataot = db.ViewAttDataOTs.Find(id);
            if (viewattdataot == null)
            {
                return HttpNotFound();
            }
            return View(viewattdataot);
        }

        // POST: /Editor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ViewAttDataOT viewattdataot = db.ViewAttDataOTs.Find(id);
            db.ViewAttDataOTs.Remove(viewattdataot);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public class CustomFunction
        {

            internal static List<Models.Company> GetCompanies(List<Models.Company> list, Models.User LoggedInUser)
            {
                switch (LoggedInUser.RoleID)
                {
                    case 1:
                        break;
                    case 2:
                        list = list.Where(aa => aa.CompID == 1 || aa.CompID == 2).ToList();
                        break;
                    case 3:
                        list = list.Where(aa => aa.CompID >= 3).ToList();
                        break;
                    case 4:
                        list = list.Where(aa => aa.CompID == LoggedInUser.CompanyID).ToList();
                        break;
                    case 5:
                        break;
                }
                return list;
            }

            internal static System.Collections.IEnumerable GetLocations(List<Models.Location> locations, List<Models.UserLocation> userLocations)
            {
                List<Models.Location> tempLocs = new List<Models.Location>();
                foreach (var item in userLocations)
                {
                    tempLocs.AddRange(locations.Where(aa => aa.LocID == item.LocationID).ToList());
                }

                return tempLocs;
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
        public ActionResult GetEmpInfo(string empNo)
        {
            List<EmpView> emp = db.EmpViews.Where(aa => aa.EmpNo == empNo).ToList();
            string Name = "";
            string Designation = "";
            string Section = "";
            string Type = "";
            string DOJ = "";
            if (emp.Count > 0)
            {
                Name = "Name: " + emp.FirstOrDefault().EmpName;
                Designation = "Designation: " + emp.FirstOrDefault().DesignationName;
                Section = "Section: " + emp.FirstOrDefault().SectionName;
                Type = "Type: " + emp.FirstOrDefault().TypeName;
                if (emp.FirstOrDefault().BirthDate != null)
                    DOJ = "Join Date: " + emp.FirstOrDefault().BirthDate.Value.ToString("dd-MMM-yyyy");
                else
                    DOJ = "Join Date: Not Added";
                if (HttpContext.Request.IsAjaxRequest())
                    return Json(Name + "@" + Designation + "@" + Section + "@" + Type + "@" + DOJ, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Name = "Name: Not found";
                Designation = "Designation: Not found";
                Section = "Section: Not found";
                Type = "Type: Not found";
                DOJ = "Join Date: Not found";
                if (HttpContext.Request.IsAjaxRequest())
                    return Json(Name + "@" + Designation + "@" + Section + "@" + Type + "@" + DOJ
                       , JsonRequestBehavior.AllowGet);
            }

            return RedirectToAction("Index");
        }
    }
}
