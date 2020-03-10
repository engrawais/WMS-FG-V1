using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class EmpAttendanceController : Controller
    {
        //
        // GET: /EmpAttendance/
        public ActionResult Index()
        {
            VMOTEntry vm = new VMOTEntry();
            User LoggedInUser = Session["LoggedUser"] as User;

            try
            {
                DateTime todate = DateTime.Today;
                DateTime datefrom = DateTime.Today.AddDays(-10);
                List<ViewAttData> attdata = new List<ViewAttData>();
                attdata = db.ViewAttDatas.Where(aa => aa.EmpID == LoggedInUser.EmpID && aa.AttDate >= datefrom && aa.AttDate <= todate).ToList();
                vm.dbViewAttData = attdata;
                vm.Count = attdata.Count;
            }
            catch (Exception)
            {

                throw;
            }
            return View(vm);
        }
        TAS2013Entities db = new TAS2013Entities();
        public ActionResult GetAttendance(VMOTEntry vmdata)
        {
            User LoggedInUser = Session["LoggedUser"] as User;

            try
            {
                if (vmdata.FromDate != null && vmdata.ToDate != null)
                {
                    List<ViewAttData> attdata = new List<ViewAttData>();
                    attdata = db.ViewAttDatas.Where(aa => aa.EmpID == LoggedInUser.EmpID && aa.AttDate >= vmdata.FromDate && aa.AttDate <= vmdata.ToDate).ToList();
                    vmdata.dbViewAttData = attdata;
                    vmdata.Count = attdata.Count;
                }
                return View("Index", vmdata);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
    public class VMOTEntry
    {
        public int EmpID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Count { get; set; }
        public List<ViewAttData> dbViewAttData;
    }
}