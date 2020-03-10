using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WMS.HelperClass;
using WMS.Models;

namespace WMS.Controllers
{
    public class LunchTimingController : Controller
    {
        TAS2013Entities db = new TAS2013Entities();
        ViewToExcel SalRepository = new ViewToExcel();
        // GET: LunchTiming
        public ActionResult Index()
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            if (LoggedInUser.UserID == 63 || LoggedInUser.UserID == 15 || LoggedInUser.UserID == 18 || LoggedInUser.UserID == 24 || LoggedInUser.UserID == 67)
            {
                DateTime? FromDate = null;
                DateTime? ToDate = null;
                if (Request.Form["JobDateFrom"] != null || Request.Form["JobDateFrom"] != null)
                {
                    FromDate = Convert.ToDateTime(Request.Form["JobDateFrom"].ToString());
                    ToDate = Convert.ToDateTime(Request.Form["JobDateTo"].ToString());
                    ViewBag.FromDate = FromDate;
                    ViewBag.ToDate = ToDate;
                }
                if (FromDate == null && ToDate == null)
                {
                    DateTime? dt = DateTime.Today.AddDays(-1);
                    List<ViewDetailAttData> Viewdetailattdata = db.ViewDetailAttDatas.Where(aa => aa.AttDate >= dt && aa.CompanyID == 1 && aa.TypeID == 7 && aa.Tin1 != null).ToList();
                    List<VMAttDataDetails> vmList = new List<VMAttDataDetails>();
                    if (Viewdetailattdata.Count > 0)
                    {
                        DateTime startchecktime = dt.Value + new TimeSpan(12, 30, 0);
                        DateTime endchecktime = dt.Value + new TimeSpan(15, 0, 0);
                        foreach (var item in Viewdetailattdata)
                        {
                            VMAttDataDetails vmattdatadetails = new VMAttDataDetails();
                            bool IsDataExist = false;
                            if (item.Tin0 != null && item.Tin0 >= startchecktime && item.Tin0 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceIn(vmattdatadetails, item.Tin0);
                            }
                            if (item.Tin1 != null && item.Tin1 >= startchecktime && item.Tin1 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceIn(vmattdatadetails, item.Tin1);
                            }
                            if (item.Tin2 != null && item.Tin2 >= startchecktime && item.Tin2 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceIn(vmattdatadetails, item.Tin2);
                            }
                            if (item.Tout0 != null && item.Tout0 >= startchecktime && item.Tout0 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceOut(vmattdatadetails, item.Tout0);
                                if (vmattdatadetails.Tin0 != null && vmattdatadetails.Tout0 != null)
                                    vmattdatadetails.Total0 = (vmattdatadetails.Tin0.Value.TimeOfDay - vmattdatadetails.Tout0.Value.TimeOfDay);
                            }
                            if (item.Tout1 != null && item.Tout1 >= startchecktime && item.Tout1 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceOut(vmattdatadetails, item.Tout1);
                                if (vmattdatadetails.Tin1 != null && vmattdatadetails.Tout1 != null)
                                    vmattdatadetails.Total1 = (vmattdatadetails.Tin1.Value.TimeOfDay - vmattdatadetails.Tout1.Value.TimeOfDay);
                            }
                            if (item.Tout2 != null && item.Tout2 >= startchecktime && item.Tout2 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceOut(vmattdatadetails, item.Tout2);
                                if (vmattdatadetails.Tin2 != null && vmattdatadetails.Tout2 != null)
                                    vmattdatadetails.Total2 = (vmattdatadetails.Tin2.Value.TimeOfDay - vmattdatadetails.Tout2.Value.TimeOfDay);
                            }
                            vmattdatadetails.AllTotal = vmattdatadetails.Total0 + vmattdatadetails.Total1 + vmattdatadetails.Total2;
                            if (IsDataExist == true)
                            {
                                vmattdatadetails.EmpNo = item.EmpNo;
                                vmattdatadetails.EmpName = item.EmpName;
                                vmattdatadetails.AttDate = (DateTime)item.AttDate;
                            }
                            vmList.Add(vmattdatadetails);
                        }
                    }                  
                    return View(vmList);
                }
                else
                {

                    List<ViewDetailAttData> Viewdetailattdata = db.ViewDetailAttDatas.Where(aa => (aa.AttDate >= FromDate && aa.AttDate <= ToDate) && aa.CompanyID == 1 && aa.TypeID == 7 && aa.Tin1 != null).ToList();
                    List<VMAttDataDetails> vmList = new List<VMAttDataDetails>();
                    if (Viewdetailattdata.Count > 0)
                    {
                        DateTime startchecktime = FromDate.Value + new TimeSpan(12, 30, 0);
                        DateTime endchecktime = ToDate.Value + new TimeSpan(15, 0, 0);
                        foreach (var item in Viewdetailattdata)
                        {
                            VMAttDataDetails vmattdatadetails = new VMAttDataDetails();
                            bool IsDataExist = false;
                            if (item.Tin0 != null && item.Tin0 >= startchecktime && item.Tin0 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceIn(vmattdatadetails, item.Tin0);
                            }
                            if (item.Tin1 != null && item.Tin1 >= startchecktime && item.Tin1 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceIn(vmattdatadetails, item.Tin1);
                            }
                            if (item.Tin2 != null && item.Tin2 >= startchecktime && item.Tin2 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceIn(vmattdatadetails, item.Tin2);
                            }
                            if (item.Tout0 != null && item.Tout0 >= startchecktime && item.Tout0 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceOut(vmattdatadetails, item.Tout0);
                                if (vmattdatadetails.Tin0 != null && vmattdatadetails.Tout0 != null)
                                    vmattdatadetails.Total0 = (vmattdatadetails.Tin0.Value.TimeOfDay - vmattdatadetails.Tout0.Value.TimeOfDay);
                            }
                            if (item.Tout1 != null && item.Tout1 >= startchecktime && item.Tout1 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceOut(vmattdatadetails, item.Tout1);
                                if (vmattdatadetails.Tin1 != null && vmattdatadetails.Tout1 != null)
                                    vmattdatadetails.Total1 = (vmattdatadetails.Tin1.Value.TimeOfDay - vmattdatadetails.Tout1.Value.TimeOfDay);
                            }
                            if (item.Tout2 != null && item.Tout2 >= startchecktime && item.Tout2 <= endchecktime)
                            {
                                IsDataExist = true;
                                vmattdatadetails = PlaceOut(vmattdatadetails, item.Tout2);
                                if (vmattdatadetails.Tin2 != null && vmattdatadetails.Tout2 != null)
                                    vmattdatadetails.Total2 = (vmattdatadetails.Tin2.Value.TimeOfDay - vmattdatadetails.Tout2.Value.TimeOfDay);
                            }
                            vmattdatadetails.AllTotal = vmattdatadetails.Total0 + vmattdatadetails.Total1 + vmattdatadetails.Total2;
                            if (IsDataExist == true)
                            {
                                vmattdatadetails.EmpNo = item.EmpNo;
                                vmattdatadetails.EmpName = item.EmpName;
                                vmattdatadetails.AttDate = (DateTime)item.AttDate;
                            }
                            vmList.Add(vmattdatadetails);
                        }
                    }

                    return View(vmList);
                }
                //aa => aa.CompanyID == 1 && aa.Status == true && aa.AttDate == datefrom).ToList();
            }
            else
            {
                return View("Index");
            }

        }
        private VMAttDataDetails PlaceIn(VMAttDataDetails vmattdatadetails, DateTime? tin0)
        {

            if (vmattdatadetails.Tin0 == null)
                vmattdatadetails.Tin0 = tin0;
            else
            {
                if (vmattdatadetails.Tin1 == null)
                    vmattdatadetails.Tin1 = tin0;
                else
                {
                    if (vmattdatadetails.Tin2 == null)
                        vmattdatadetails.Tin2 = tin0;
                }
            }
            return vmattdatadetails;
        }
        private VMAttDataDetails PlaceOut(VMAttDataDetails vmattdatadetails, DateTime? tout0)
        {
            if (vmattdatadetails.Tout0 == null)
                vmattdatadetails.Tout0 = tout0;
            else
            {
                if (vmattdatadetails.Tout1 == null)
                    vmattdatadetails.Tout1 = tout0;
                else
                {
                    if (vmattdatadetails.Tout2 == null)
                        vmattdatadetails.Tout2 = tout0;
                }
            }
            return vmattdatadetails;
        }
        
    }
    public class VMAttDataDetails
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public DateTime AttDate { get; set; }
        public DateTime? Tin0 { get; set; }
        public DateTime? Tout0 { get; set; }
        public TimeSpan Total0 { get; set; }
        public DateTime? Tin1 { get; set; }
        public DateTime? Tout1 { get; set; }
        public TimeSpan Total1 { get; set; }
        public DateTime? Tin2 { get; set; }
        public DateTime? Tout2 { get; set; }
        public TimeSpan Total2 { get; set; }
        public TimeSpan AllTotal { get; set; }

    }
}