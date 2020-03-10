using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WMS.Controllers;
using WMS.CustomClass;
using WMS.HelperClass;
using WMS.Models;
using WMSLibrary;

namespace WMS.Reports
{
    /// <summary>
    /// This is report Container Please Always follow regions and place your code as per regions defined,
    /// Always Solve Unidentified Regions
    /// </summary>
    #region --Main Region (load report, Filter Implemetation, Helping Function, Methods and Datatables)
    public partial class ReportContainer : System.Web.UI.Page
    {
        String title = "";
        string _dateFrom = "";
        List<EmpPhoto> companyimage = new List<EmpPhoto>();
        FiltersModel fm;
        TAS2013Entities db = new TAS2013Entities();

        #region -- Page_Load  (Reports Region)--

        protected void Page_Load(object sender, EventArgs e)
        {
            String reportName = Request.QueryString["reportname"];
            String type = Request.QueryString["type"];
            if (!Page.IsPostBack)
            {
                List<string> list = Session["ReportSession"] as List<string>;
                fm = new FiltersModel();
                fm = Session["FiltersModel"] as FiltersModel;
                CreateDataTable();
                CreateFlexyMonthlyDataTable();
                CreateEmpSumTimeDatatable();
                User LoggedInUser = HttpContext.Current.Session["LoggedUser"] as User;
                QueryBuilder qb = new QueryBuilder();
                string query = qb.MakeCustomizeQuery(LoggedInUser);
                _dateFrom = list[0];
                string _dateTo = list[1];
                companyimage = GetCompanyImages(fm);
                string PathString = "";
                string consolidatedMonth = "";
                switch (reportName)
                {
                    #region -----Employee Job Card---------

                    case "Emp_JobCardsSummary": DataTable EmpJCdt1 = qb.GetValuesfromDB("select * from ViewJobCardAudit " + query + " and (DateCreated >= '" + _dateFrom + "'and DateCreated <= '" + _dateTo + "')");
                        List<ViewJobCardAudit> _ViewListEmpJob1 = EmpJCdt1.ToList<ViewJobCardAudit>();
                        List<ViewJobCardAudit> _TempViewListEmpJob1 = new List<ViewJobCardAudit>();
                        title = "Employee Job Card Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpSummaryJC.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpSummaryJC.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListEmpJob1, _ViewListEmpJob1), _dateFrom + " To " + _dateTo);

                        break;


                    case "Emp_JobCards": DataTable EmpJCdt = qb.GetValuesfromDB("select * from ViewJobCardAudit " + query + " and (DateCreated >= '" + _dateFrom + "')");
                        List<ViewJobCardAudit> _ViewListEmpJob = EmpJCdt.ToList<ViewJobCardAudit>();
                        List<ViewJobCardAudit> _TempViewListEmpJob = new List<ViewJobCardAudit>();
                        title = "Employee Job Card Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpJobCards.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpJobCards.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListEmpJob, _ViewListEmpJob), _dateFrom);

                        break;
                    #endregion

                    #region --- Employee Record ---
                    case "monthly_productivity":
                        monthlyProductivityProcess(_dateFrom, _dateTo, query);

                        break;
                    case "badli_report":

                        DataTable badlidt = qb.GetValuesfromDB("select * from BadliRecordEmp where (Date >= " + "'" + _dateFrom + "'" + " and Date <= " + "'" + _dateTo + "'" + " )");
                        List<BadliRecordEmp> _BadliList = badlidt.ToList<BadliRecordEmp>();

                        title = "Badli Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/BadliReport.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/BadliReport.rdlc";

                        LoadReport(PathString, GetBadliValue(_BadliList), _dateFrom + " TO " + _dateTo);
                        break;
                    case "Employee_Att_Summary_New_report": DataTable dt4 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )");
                        List<ViewAttData> ListOfAttDate = new List<ViewAttData>();
                        List<ViewAttData> TempList = new List<ViewAttData>();
                        title = "Employee Attendace Summary New";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpAttSummaryNew.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpAttSummaryNew.rdlc";
                        ListOfAttDate = dt4.ToList<ViewAttData>();
                        TempList = new List<ViewAttData>();

                        LoadReport(PathString, ReportsFilterImplementation(fm, TempList, ListOfAttDate), _dateFrom + " TO " + _dateTo);


                        break;
                    case "department_attendance_summary": HRReportsMaker hrm = new HRReportsMaker();
                        List<AttDeptSummary> AttDept = hrm.GetListForAttDepartmentsSummary(Session["FiltersModel"] as FiltersModel, _dateFrom, _dateTo);
                        title = "Department Attendace Summary";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/AttDepartmentSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/AttDepartmentSummary.rdlc";

                        LoadReport(PathString, AttDept, _dateFrom + " TO " + _dateTo);

                        break;
                    case "emp_record": DataTable dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        List<EmpView> _ViewList = dt.ToList<EmpView>();
                        List<EmpView> _TempViewList = new List<EmpView>();
                        title = "Employee Record Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/Employee.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/Employee.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);

                        break;
                    case "emp_record_active": dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        title = "Active Employees Record Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/Employee.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/Employee.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);

                        break;
                    case "emp_record_inactive": dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=0 ");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        title = "Inactive Employees Record Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/Employee.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/Employee.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);
                        break;
                    case "emp_detail_excel": DataTable dt1 = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        List<EmpView> _ViewList1 = dt1.ToList<EmpView>();
                        List<EmpView> _TempViewList1 = new List<EmpView>();
                        title = "Employee Detail Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmployeeDetail.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmployeeDetail.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), _dateFrom + " TO " + _dateTo);

                        break;
                    #endregion

                    #region -- Daily Report --


                    case "leave_application": dt1 = qb.GetValuesfromDB("select * from ViewLvApplication " + query + " and (FromDate >= '" + _dateFrom + "' and ToDate <= '" + _dateTo + "' )");
                        List<ViewLvApplication> _ViewListLvApp = dt1.ToList<ViewLvApplication>();
                        List<ViewLvApplication> _TempViewListLvApp = new List<ViewLvApplication>();
                        title = "Leave Application Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLeave.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListLvApp, _ViewListLvApp), _dateFrom + " TO " + _dateTo);

                        break;
                    case "lv_applicationCPL": dt1 = qb.GetValuesfromDB("select * from ViewLvApplication " + query + " and ( LvType='E' and FromDate >= '" + _dateFrom + "' and ToDate <= '" + _dateTo + "' )");
                        _ViewListLvApp = dt1.ToList<ViewLvApplication>();
                        _TempViewListLvApp = new List<ViewLvApplication>();
                        title = "Leave Application Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRCPLLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRCPLLeave.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListLvApp, _ViewListLvApp), _dateFrom + " TO " + _dateTo);
                        break;
                    case "lv_applicationLWOP": dt1 = qb.GetValuesfromDB("select * from ViewLvApplication " + query + " and ( LvType='F' and FromDate >= '" + _dateFrom + "' and ToDate <= '" + _dateTo + "' )");
                        _ViewListLvApp = dt1.ToList<ViewLvApplication>();
                        _TempViewListLvApp = new List<ViewLvApplication>();
                        title = "Leave Application Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLWOPLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLWOPLeave.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListLvApp, _ViewListLvApp), _dateFrom + " TO " + _dateTo);

                        break;
                    case "detailed_att": DataTable dt2 = qb.GetValuesfromDB("select * from ViewDetailAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )");
                        List<ViewDetailAttData> _ViewList2 = dt2.ToList<ViewDetailAttData>();
                        List<ViewDetailAttData> _TempViewList2 = new List<ViewDetailAttData>();
                        title = "Detailed Attendence";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRDetailed.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRDetailed.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList2, _ViewList2), _dateFrom + " TO " + _dateTo);

                        break;
                    case "da_sheet": DataTable dt12 = qb.GetValuesfromDB("select * from ViewDetailAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                + _dateTo + "'" + " )");
                        List<ViewDetailAttData> _ViewList12 = dt12.ToList<ViewDetailAttData>();
                        List<ViewDetailAttData> _TempViewList12 = new List<ViewDetailAttData>();
                        title = "Daily Attendance Sheet";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DASheet.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DASheet.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList12, _ViewList12), _dateFrom + " TO " + _dateTo);

                        break;
                    case "consolidated_att": DataTable dt3 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )");
                        List<ViewAttData> _ViewList3 = dt3.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList3 = new List<ViewAttData>();
                        title = "Consolidated Attendence Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRAttendance.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRAttendance.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList3, _ViewList3), _dateFrom + " TO " + _dateTo);

                        break;
                    case "NegativeWorkMin": dt3 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                 + _dateTo + "'" + " ) and WorkMin<0");
                        _ViewList3 = dt3.ToList<ViewAttData>();
                        _TempViewList3 = new List<ViewAttData>();
                        title = "Negative Work Minutes Attendence Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRPresent.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRPresent.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList3, _ViewList3), _dateFrom + " TO " + _dateTo);

                        break;
                    case "present": DataTable datatable = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusP = 1 ");
                        List<ViewAttData> _ViewList4 = datatable.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList4 = new List<ViewAttData>();
                        title = "Present Employee Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRPresent.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRPresent.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList4, _ViewList4), _dateFrom + " TO " + _dateTo);

                        break;
                    case "presentFather": datatable = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusP = 1 ");
                        _ViewList4 = datatable.ToList<ViewAttData>();
                        _TempViewList4 = new List<ViewAttData>();
                        title = "Present Employee Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRPresentFather.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRPresentFather.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList4, _ViewList4), _dateFrom + " TO " + _dateTo);

                        break;
                    case "absent": DataTable dt5 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusAB = 1 ");
                        List<ViewAttData> _ViewList5 = dt5.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList5 = new List<ViewAttData>();
                        title = "Absent Employee Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRAbsent.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRAbsent.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList5, _ViewList5), _dateFrom + " TO " + _dateTo);

                        break;
                    case "lv_application": DataTable dt6 = qb.GetValuesfromDB("select * from ViewLvApplication " + query + " and Status=1 " + " and (FromDate >= " + "'" + _dateFrom + "'" + " and FromDate <= " + "'"
                                                + _dateTo + "'" + " )");
                        List<ViewLvApplication> _ViewList6 = dt6.ToList<ViewLvApplication>();
                        List<ViewLvApplication> _TempViewList6 = new List<ViewLvApplication>();
                        title = "Leave Attendence Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLeave.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList6, _ViewList6), _dateFrom + " TO " + _dateTo);
                        //To-do Develop Leave Attendance Report
                        break;
                    case "short_lv": DataTable dt7 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusSL=1");
                        List<ViewAttData> _ViewList7 = dt7.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList7 = new List<ViewAttData>();
                        title = "Short Leave Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRShortLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRShortLeave.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList7, _ViewList7), _dateFrom + " TO " + _dateTo);

                        break;
                    case "late_in": DataTable dt8 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                    + _dateTo + "'" + " )" + " and StatusLI=1 ");
                        List<ViewAttData> _ViewList8 = dt8.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList8 = new List<ViewAttData>();
                        title = "Late In Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLateIn.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLateIn.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;

                    case "late_out": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                            + _dateTo + "'" + " )" + " and StatusLO=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        title = "Late Out Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLateOut.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLateOut.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;

                    case "early_in": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                           + _dateTo + "'" + " )" + " and StatusEI=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        title = "Early In Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DREarlyIn.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DREarlyIn.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "early_out": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                             + _dateTo + "'" + " )" + " and StatusEO=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        title = "Early Out Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DREarlyOut.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DREarlyOut.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "overtime": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                         + _dateTo + "'" + " )" + " and StatusOT=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        title = "OverTime Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DROverTime.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DROverTime.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "missing_attendance": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                    + _dateTo + "'" + " )" + " and ((TimeIn is null and TimeOut is not null) or (TimeIn is not null and TimeOut is null)) ");

                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        title = "Missing Attendence Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRMissingAtt.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRMissingAtt.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "multiple_in_out": dt = qb.GetValuesfromDB("select * from ViewMultipleInOut " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and (Tin1 is not null or TOut1 is not null)");
                        //change query for multiple_in_out
                        List<ViewMultipleInOut> _ViewList9 = dt.ToList<ViewMultipleInOut>();
                        List<ViewMultipleInOut> _TempViewList9 = new List<ViewMultipleInOut>();
                        title = "Multiple In/Out Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRMultipleInOut.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRMultipleInOut.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList9, _ViewList9), _dateFrom + " TO " + _dateTo);

                        break;
                    case "polldata_in": dt = qb.GetValuesfromDB("select * from ViewPollData " + query + " and Status=1 " + " and (EntDate >= " + "'" + _dateFrom + "'" + " and EntDate <= " + "'"
                                     + _dateTo + "'" + " and RdrDutyName='IN'" + " )");
                        List<ViewPollData> _ViewList88 = dt.ToList<ViewPollData>();
                        List<ViewPollData> _TempViewList88 = new List<ViewPollData>();
                        title = "Device Data IN Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRDeviceData.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRDeviceData.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList88, _ViewList88), _dateFrom + " TO " + _dateTo);

                        break;
                    case "polldata_out": dt = qb.GetValuesfromDB("select * from ViewPollData " + query + " and Status=1 " + " and (EntDate >= " + "'" + _dateFrom + "'" + " and EntDate <= " + "'"
                                    + _dateTo + "'" + " and RdrDutyName='OUT' " + " )");
                        _ViewList88 = dt.ToList<ViewPollData>();
                        _TempViewList88 = new List<ViewPollData>();
                        title = "Device Data OUT Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRDeviceData.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRDeviceData.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList88, _ViewList88), _dateFrom + " TO " + _dateTo);

                        break;

                    case "Approved_Overtime": dt = qb.GetValuesfromDB("select * from ViewAttDataAppOT " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                      + _dateTo + "'" + " )" + " and StatusOT=1 ");
                        List<ViewAttDataAppOT> _ViewListApprovedOT = dt.ToList<ViewAttDataAppOT>();
                        List<ViewAttDataAppOT> _TempViewListApprovedOT = new List<ViewAttDataAppOT>();
                        title = "Approved OverTime Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRApprovedOT.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRApprovedOT.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListApprovedOT, _ViewListApprovedOT), _dateFrom + " TO " + _dateTo);

                        break;



                    #endregion

                    #region -- Monthly Leave Report
                    case "monthly_leave_sheet": string _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        _ViewList1 = dt.ToList<EmpView>();
                        _TempViewList1 = new List<EmpView>();
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MLvConsumed.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MLvConsumed.rdlc";
                        int monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        int monthTo = Convert.ToDateTime(_dateTo).Month;
                        //int totalMonths = monthfrom < monthTo ? monthTo : monthfrom;
                        for (int ul = monthfrom > monthTo ? monthTo : monthfrom; ul <= (monthfrom < monthTo ? monthTo : monthfrom); ul++)
                        {
                            LoadReport(PathString, GetLV(ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), monthfrom, Convert.ToDateTime(_dateFrom)), ul);

                        }

                        // LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), _dateFrom);
                        break;
                    case "monthly_leave_sheetCPL": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        _ViewList1 = dt.ToList<EmpView>();
                        _TempViewList1 = new List<EmpView>();
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MLvConsumedCPL.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MLvConsumedCPL.rdlc";
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        //int totalMonths = monthfrom < monthTo ? monthTo : monthfrom;
                        for (int ul = monthfrom > monthTo ? monthTo : monthfrom; ul <= (monthfrom < monthTo ? monthTo : monthfrom); ul++)
                        {
                            LoadReport(PathString, GetLVCPL(ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), monthfrom, Convert.ToDateTime(_dateFrom)), ul);

                        }

                        // LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), _dateFrom);
                        break;
                    #endregion
                    #region Permanennt Monthly--
                    case "monthly_21-20": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '1" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Status=1 " + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Period = " + _period);
                        title = "Monthly Sheet (21st to 20th)";
                        List<ViewMonthlyDataPer> _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        List<ViewMonthlyDataPer> _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRSheetP.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRSheetP.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom);
                        break;
                    case "monthlysummary_21-20": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '1" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Status=1" + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Period = " + _period);
                        title = "Monthly Summary Report (21st to 20th)";
                        _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom);
                        break;
                    case "monthly_21-20_excel": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '1" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Status=1" + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Period = " + _period);
                        title = "Monthly Sheet (21st to 20th)";
                        _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDetailExcelP.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDetailExcelP.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom);
                        break;
                    case "monthly_26-25_consolidated": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        consolidatedMonth = "";
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '1" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and Status=1" + " and" + consolidatedMonth);
                        title = "Monthly Consolidated (26th to 25th)(Excel)";
                        _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDetailExcelP26-25.rdlc";
                        else
                            PathString = "/Reports/RDLC/MRDetailExcelP26-25.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom + " to " + _dateTo);
                        break;
                    case "monthly_21-20_consolidated": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        consolidatedMonth = "";
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '1" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Status=1" + " and" + consolidatedMonth);
                        title = "Monthly Consolidated (21th to 20th)(Excel)";
                        _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDetailExcelP.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDetailExcelP.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom + " to " + _dateTo);
                        break;

                    case "monthly_21-20_flexy":
                        dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        title = "Employee Record Report";

                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRFlexyDetailExcelP.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRFlexyDetailExcelP.rdlc";
                        LoadReport(LoadPermanentMonthlyDT(ReportsFilterImplementation(fm, _TempViewList, _ViewList), Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo)), PathString, _dateFrom + " TO " + _dateTo);
                        break;
                    #endregion
                    #region -- Contractuals AttendanceReport --


                    #region --- Monthly consolidated with Badli Days (RWML units) Active /Inactive Employees

                    #region-- RWML badli Days report Active Report

                    case "monthly_1-31_conBadli": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;

                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewAttMonthBadli " + query + " and" + consolidatedMonth + " and Status = 1");
                        title = "Monthly Consolidated Attendance with Badli Days (1st to 31th)";
                        List<ViewAttMonthBadli> _ViewListMonthlyDataB = dt.ToList<ViewAttMonthBadli>();
                        List<ViewAttMonthBadli> _TempViewListMonthlyDataB = new List<ViewAttMonthBadli>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDetailExcelCBadli.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDetailExcelCBadli.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataB, _ViewListMonthlyDataB), _dateFrom + " to " + _dateTo);
                        break;
                    #endregion

                    #region-- RWML badli Days report In-Active Report

                    case "monthly_1-31_conBadliIE": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;

                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewAttMonthBadli " + query + " and" + consolidatedMonth + " and Status = 0");
                        title = "Monthly Consolidated Attendance with Badli Days (1st to 31th)";
                        List<ViewAttMonthBadli> _ViewListMonthlyDataBIE = dt.ToList<ViewAttMonthBadli>();
                        List<ViewAttMonthBadli> _TempViewListMonthlyDataBIE = new List<ViewAttMonthBadli>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDetailExcelCBadliIE.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDetailExcelCBadliIE.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataBIE, _ViewListMonthlyDataBIE), _dateFrom + " to " + _dateTo);
                        break;
                    #endregion
                    #endregion
                    #region-- Monthly Approve OT By Emp Active and Non Active --
                    case "monthly_1-31_consolidated_AppOT": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;

                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewEmpAttMnAppOT " + query + " and " + consolidatedMonth);
                        title = "Monthly Consolidated Attendance Sheet With Approve OT (1st to 31th)";
                        List<ViewEmpAttMnAppOT> _ViewListMonthlyDataApprovedOT = dt.ToList<ViewEmpAttMnAppOT>();
                        List<ViewEmpAttMnAppOT> _TempViewListMonthlyApprovedOT = new List<ViewEmpAttMnAppOT>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MREmpAttMnAppOT.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MREmpAttMnAppOT.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyApprovedOT, _ViewListMonthlyDataApprovedOT), _dateFrom + " to " + _dateTo);
                        break;
                    #region-- Monthly Approve OT By Emp Non Active --
                    case "monthly_1-31_consolidated_AppOTIE": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;

                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewEmpAttMnAppOT " + query + " and  " + consolidatedMonth);
                        title = "Monthly Consolidated Attendance Sheet With Approve OT (1st to 31th)";
                        List<ViewEmpAttMnAppOT> _ViewListMonthlyDataApprovedOTIE = dt.ToList<ViewEmpAttMnAppOT>();
                        List<ViewEmpAttMnAppOT> _TempViewListMonthlyApprovedOTIE = new List<ViewEmpAttMnAppOT>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MREmpAttMnAppOT.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MREmpAttMnAppOT.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyApprovedOTIE, _ViewListMonthlyDataApprovedOTIE), _dateFrom + " to " + _dateTo);
                        break;
                    #endregion
                    #endregion
                    #region-- Monthly data report Active and NonActive Employees
                    case "monthly_1-31_consolidated": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;

                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                        {
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        }
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and" + consolidatedMonth + " and Status = 1");
                        title = "Monthly Consolidated Attendance Sheet (1st to 31th)";
                        List<ViewMonthlyData> _ViewListMonthlyData = dt.ToList<ViewMonthlyData>();
                        List<ViewMonthlyData> _TempViewListMonthlyData = new List<ViewMonthlyData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDetailExcelC.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDetailExcelC.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyData, _ViewListMonthlyData), _dateFrom + " to " + _dateTo);
                        break;
                    case "monthly_1-31_consolidatedIE": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;

                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and" + consolidatedMonth + " and Status = 0");
                        title = "Monthly Consolidated Attendance Sheet (1st to 31th)";
                        List<ViewMonthlyData> _ViewListMonthlyDataIE = dt.ToList<ViewMonthlyData>();
                        List<ViewMonthlyData> _TempViewListMonthlyDataIE = new List<ViewMonthlyData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == true)
                            PathString = "/Reports/RDLC/MRDetailExcelCIE.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDetailExcelCIE.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataIE, _ViewListMonthlyDataIE), _dateFrom + " to " + _dateTo);
                        break;

                    #endregion

                    #region old
                    case "monthly_1-31": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and Period = " + _period);
                        title = "Monthly Sheet (1st to 31st)";
                        _ViewListMonthlyData = dt.ToList<ViewMonthlyData>();
                        _TempViewListMonthlyData = new List<ViewMonthlyData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRSheetC.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRSheetC.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyData, _ViewListMonthlyData), _dateFrom);
                        break;


                    case "monthlysummary_1-31": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Period = " + _period);
                        title = "Monthly Summary (1st to 31st)";
                        _ViewListMonthlyData = dt.ToList<ViewMonthlyData>();
                        _TempViewListMonthlyData = new List<ViewMonthlyData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRSummaryC.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRSummaryC.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyData, _ViewListMonthlyData), _dateFrom);
                        break;





                    #region Approved OT
                    //Load Report








                    //Download Link Approve OT

                    case "monthlyAppOTConDownload": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;

                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewEmpAttMnAppOT " + query + " and" + consolidatedMonth);
                        title = "Monthly with Badli Consolidated Attendance Sheet (1st to 31th)";
                        List<ViewEmpAttMnAppOT> _ViewListMonthlyDataApOT = dt.ToList<ViewEmpAttMnAppOT>();
                        List<ViewEmpAttMnAppOT> _TempViewListMonthlyDataApOT = new List<ViewEmpAttMnAppOT>();


                        DownloadAppOTReport(ReportsFilterImplementation(fm, _TempViewListMonthlyDataApOT, _ViewListMonthlyDataApOT), _dateFrom + " to " + _dateTo, LoggedInUser.UserID);

                        break;

                    #endregion







                    case "monthlyConDownload": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;

                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and" + consolidatedMonth);
                        title = "Monthly Consolidated Attendance Sheet (1st to 31th)";
                        _ViewListMonthlyData = dt.ToList<ViewMonthlyData>();
                        _TempViewListMonthlyData = new List<ViewMonthlyData>();
                        DownloadReport(ReportsFilterImplementation(fm, _TempViewListMonthlyData, _ViewListMonthlyData), _dateFrom + " to " + _dateTo, LoggedInUser.UserID);
                        break;
                    case "monthlyBadliConDownload": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;

                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewAttMonthBadli " + query + " and" + consolidatedMonth);
                        title = "Monthly with Badli Consolidated Attendance Sheet (1st to 31th)";
                        _ViewListMonthlyDataB = dt.ToList<ViewAttMonthBadli>();
                        _TempViewListMonthlyDataB = new List<ViewAttMonthBadli>();
                        DownloadBadliReport(ReportsFilterImplementation(fm, _TempViewListMonthlyDataB, _ViewListMonthlyDataB), _dateFrom + " to " + _dateTo, LoggedInUser.UserID);
                        break;
                    case "monthly_1-31_overtime": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;

                        if (monthfrom != 12)
                        {
                            for (int i = monthTo; i <= monthTo; i++)
                            {
                                consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                            }
                            if (consolidatedMonth.Length > 4)
                                consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        }
                        else
                            consolidatedMonth = " Period = '12" + Convert.ToDateTime(_dateTo).Year.ToString() + "'";
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and" + consolidatedMonth);
                        title = "Monthly Consolidated Attendance Sheet (1st to 31th)";
                        _ViewListMonthlyData = dt.ToList<ViewMonthlyData>();
                        _TempViewListMonthlyData = new List<ViewMonthlyData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDetailExcelCOT.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDetailExcelCOT.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyData, _ViewListMonthlyData), _dateFrom + " to " + _dateTo);
                        break;

                    case "badli_month": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        dt = qb.GetValuesfromDB("select * from ViewAttBadliMonth " + query + " and EmpPeriod=" + _period);
                        List<ViewAttBadliMonth> _ViewListB = dt.ToList<ViewAttBadliMonth>();
                        List<ViewAttBadliMonth> _TempViewListB = new List<ViewAttBadliMonth>();
                        title = "Employee Monthly Badli Report";

                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/BadliMonthlyReport.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/BadliMonthlyReport.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListB, _ViewListB), _dateFrom + " to " + _dateTo);
                        break;
                    case "editor_ot": DataTable dt11 = qb.GetValuesfromDB("select * from ViewAttDataOT ");
                        List<ViewAttDataOT> _ViewListeditor = dt11.ToList<ViewAttDataOT>();
                        List<ViewAttDataOT> _TempViewListeditor = new List<ViewAttDataOT>();
                        title = "Editor OverTime Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EditorOT.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EditorOT.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListeditor, _ViewListeditor), _dateFrom + " TO " + _dateTo);

                        break;
                    #endregion


                    #endregion
                    #region Monthly Sheet ab nd lv


                    case "MonthlySheetAbandLv": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        for (int i = monthfrom; i <= monthTo; i++)
                        {
                            consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                        }
                        if (consolidatedMonth.Length > 4)
                            consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and (AbDays>0 or LeaveDays>0) and Status=1" + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and Period = " + _period);
                        title = "Monthly Sheet (Absent and Leaves Only) (1st to 31st)";
                        _ViewListMonthlyData = dt.ToList<ViewMonthlyData>();
                        _TempViewListMonthlyData = new List<ViewMonthlyData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MonthlySheetAbandLv.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MonthlySheetAbandLv.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyData, _ViewListMonthlyData), _dateFrom);
                        break;

                    case "MonthlySheetIncompleteOnly": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        for (int i = monthfrom; i <= monthTo; i++)
                        {
                            consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                        }
                        if (consolidatedMonth.Length > 4)
                            consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData" + query + " and (((WorkDays + AbDays )!= TotalDays) and PreDays>0) and Status=1" + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and Period = " + _period);
                        title = "Monthly Sheet Missing Only (1st to 31st)";
                        _ViewListMonthlyData = dt.ToList<ViewMonthlyData>();
                        _TempViewListMonthlyData = new List<ViewMonthlyData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MonthlySheetIncompleteOnly.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MonthlySheetIncompleteOnly.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyData, _ViewListMonthlyData), _dateFrom);
                        break;


                    #endregion

                    #region -- Detailed Report --
                    case "emp_att": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )");
                        title = "Employee Attendance";
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpAttSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpAttSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);
                        break;
                    case "emp_att_old": dt = qb.GetValuesfromDB("select * from ViewAttDataOld1 " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )");
                        title = "Employee Attendance";
                        List<ViewAttDataOld1> _ViewList68 = dt.ToList<ViewAttDataOld1>();
                        List<ViewAttDataOld1> _TempViewList68 = new List<ViewAttDataOld1>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpAttSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpAttSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList68, _ViewList68), _dateFrom + " TO " + _dateTo);
                        break;
                    case "emp_attTime": dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        title = "Employee Attendance";
                        dt1 = qb.GetValuesfromDB("select * from ViewDetailAttData " + query + " and Status=1 and CompanyID=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )");
                        _ViewList2 = dt1.ToList<ViewDetailAttData>();
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpSumTime.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpSumTime.rdlc";
                        LoadReports(PathString, AddEmpSumTime(_ViewList2, ReportsFilterImplementation(fm, _TempViewList, _ViewList)), Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo));
                        break;
                    case "emp_absent":
                        _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )" + " and StatusAB = 1 ");
                        title = "Employee Absent";
                        _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths

                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom);
                        break;
                    case "lv_quota": dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1");
                        _ViewList1 = dt.ToList<EmpView>();
                        _TempViewList1 = new List<EmpView>();
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MYLeaveSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MYLeaveSummary.rdlc";
                        LoadReport(PathString, GYL(ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), Convert.ToDateTime(_dateFrom)));
                        // LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom);
                        break;
                    #endregion

                    #region ----Summary Report----
                    /////////////////////////////////////////////////////////////   
                    /////////////////Summary Reports////////////////////////////
                    ///////////////////////////////////////////////////////////
                    case "company_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "C"), _dateFrom + " TO " + _dateTo, "Company Consolidated Summary");
                        break;
                    case "company_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "C"), _dateFrom + " TO " + _dateTo, "Company Strength Summary");
                        break;
                    case "company_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "C"), _dateFrom + " TO " + _dateTo, "Company Work Times Summary");
                        break;
                    case "location_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        ReportsFilterImplementation(fm, _dateFrom, _dateTo, "L");
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "L"), _dateFrom + " TO " + _dateTo, "Location Consolidated Summary");
                        break;
                    case "location_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "L"), _dateFrom + " TO " + _dateTo, "Location Strength Summary");
                        break;
                    case "location_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "L"), _dateFrom + " TO " + _dateTo, "Location Work Times Summary");
                        break;
                    case "shift_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "S"), _dateFrom + " TO " + _dateTo, "Shift Consolidated Summary");
                        break;
                    case "shift_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "S"), _dateFrom + " TO " + _dateTo, "Shift Strength Summary");
                        break;
                    case "shift_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "S"), _dateFrom + " TO " + _dateTo, "Shift Work Times Summary");
                        break;
                    case "category_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "A"), _dateFrom + " TO " + _dateTo, "Category Consolidated Summary");
                        break;
                    case "category_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "A"), _dateFrom + " TO " + _dateTo, "Category Strength Summary");
                        break;
                    case "category_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "A"), _dateFrom + " TO " + _dateTo, "Category Work Times Summary");
                        break;
                    case "type_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "T"), _dateFrom + " TO " + _dateTo, "Employee Type Consolidated Summary");
                        break;
                    case "type_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "T"), _dateFrom + " TO " + _dateTo, "Employee Type Strength Summary");
                        break;
                    case "type_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "T"), _dateFrom + " TO " + _dateTo, "Employee Type Work Times Summary");
                        break;
                    case "dept_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "D"), _dateFrom + " TO " + _dateTo, "Department Consolidated Summary");
                        break;
                    case "dept_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "D"), _dateFrom + " TO " + _dateTo, "Department Strength Summary");
                        break;
                    case "dept_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "D"), _dateFrom + " TO " + _dateTo, "Department Work Times Summary");
                        break;
                    case "section_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "E"), _dateFrom + " TO " + _dateTo, "Section Consolidated Summary");
                        break;
                    case "section_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "E"), _dateFrom + " TO " + _dateTo, "Section Strength Summary");
                        break;
                    case "section_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "E"), _dateFrom + " TO " + _dateTo, "Section Work Times Summary");
                        break;
                    #endregion

                    #region ////////////Edit Attendance/////////

                    case "edit_Attendance":
                        string dtF = _dateFrom + " 00:00:01";
                        string dtT = _dateTo + " 23:59:59";
                        datatable = qb.GetValuesfromDB("select * from ViewEditAttendance " + query + "" + " and (NewTimeIn >= " + "'" + _dateFrom + "'" + " and NewTimeIn <= " + "'"
                                                 + _dateTo + "'" + " )");
                        List<ViewEditAttendance> _ViewList15 = datatable.ToList<ViewEditAttendance>();
                        List<ViewEditAttendance> _TempViewList15 = new List<ViewEditAttendance>();
                        title = "Edit Attendance Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/REditAttendance.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/REditAttendance.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList15, _ViewList15), _dateFrom + " TO " + _dateTo);

                        break;
                    case "emp_audit": dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        DataTable dtEdit = qb.GetValuesfromDB("select * from ViewEditAttendance " + query + " and Status=1 and NewTimeIn>='" + _dateFrom + "' and NewTimeIn<='" + _dateTo + "'");
                        _ViewList = dt.ToList<EmpView>();
                        List<ViewEditAttendance> _editList = dtEdit.ToList<ViewEditAttendance>();
                        _TempViewList = new List<EmpView>();
                        title = "Employee Audit Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/AuditReport.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/AuditReport.rdlc";
                        List<EmpView> emps = ReportsFilterImplementation(fm, _TempViewList, _ViewList);
                        List<ModelEditAttAudit> mdea = GetTotalCount(emps, _editList);
                        LoadReport(PathString, mdea.Where(aa => aa.TotalRecords > 0).ToList(), _dateFrom + " TO " + _dateTo);

                        break;
                    #endregion

                    #region ---------Fatima Payroll Report-----
                    case "FatimaPayroll_Report":
                        DateTime Mydt = Convert.ToDateTime(_dateFrom);
                        string period = Mydt.Month.ToString("00") + Mydt.Year.ToString("00");
                        DataTable dtPayroll = qb.GetValuesfromDB("select * from ViewFatimaPayroll " + query + " and MonthYear = " + period);
                        List<ViewFatimaPayroll> _ViewListPayroll = dtPayroll.ToList<ViewFatimaPayroll>();
                        List<ViewFatimaPayroll> _TempViewPayroll = new List<ViewFatimaPayroll>();
                        title = "Payroll Monthly Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRFatimaPayroll.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRFatimaPayroll.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewPayroll, _ViewListPayroll), _dateFrom + " TO " + _dateTo);

                        break;


                    #endregion
                    #region ---------HSE Hour Report-----


                    case "HSEHourEmp":
                        string _periodFromEmp = "";
                        string _periodToEmp = "";
                        _periodFromEmp = Convert.ToDateTime(_dateFrom).ToString();
                        _periodToEmp = Convert.ToDateTime(_dateTo).ToString();
                        dt = qb.GetValuesfromDB("select * from AttData  where AttDate >='" + _periodFromEmp + "'and AttDate <= '" + _periodToEmp + "'");
                        List<AttData> _ViewAttList = dt.ToList<AttData>();


                        dt1 = qb.GetValuesfromDB("select * from EmpView " + query);
                        _ViewList1 = dt1.ToList<EmpView>();
                        _TempViewList1 = new List<EmpView>();


                        _TempViewList = ReportsFilterImplementation(fm, _TempViewList1, _ViewList1);
                        title = "HSE Hours Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRHSEbyEmp.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRHSEbyEmp.rdlc";

                        List<ModelHSEDataEmp> hseData = GetHSEData(_ViewAttList, _TempViewList);
                        LoadReport(PathString, hseData, _dateFrom + " TO " + _dateTo);



                        break;

                    case "HSEHour":
                        string _periodFrom = "";
                        string _periodTo = "";
                        _periodFrom = Convert.ToDateTime(_dateFrom).ToString("yyyy-MMM-dd");
                        _periodTo = Convert.ToDateTime(_dateTo).ToString("yyyy-MMM-dd");

                        dt = qb.GetValuesfromDB("select * from ViewHSEHour  where HSEDate >='" + _periodFrom + "'and HSEDate <= '" + _periodTo + "' and Total>0 and Type='P'");
                        //dt = qb.GetValuesfromDB("select * from ViewHSEHour  where HSEDate ='" + _periodFrom + "'and Total>0");
                        List<ViewHSEHour> _ViewListHSEHour = dt.ToList<ViewHSEHour>();
                        title = "HSE Hours Report (Plant)";

                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRHSEHourS.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRHSEHourS.rdlc";
                        LoadReport(PathString, GetHSEData(_ViewListHSEHour, Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo), db.Sections.Where(aa => aa.CompanyID == 1).ToList()), _dateFrom);
                        break;
                    case "HSEHour_con":
                        _periodFrom = Convert.ToDateTime(_dateFrom).ToString("yyyy-MMM-dd");
                        _periodTo = Convert.ToDateTime(_dateTo).ToString("yyyy-MMM-dd");
                        dt = qb.GetValuesfromDB("select * from ViewHSEHour  where HSEDate >='" + _periodFrom + "'and HSEDate <= '" + _periodTo + "' and Total>0 and Type='P'");
                        _ViewListHSEHour = dt.ToList<ViewHSEHour>();
                        title = "HSE Hours Report (Plant)";

                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRHSEHourC.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRHSEHourC.rdlc";
                        LoadReport(PathString, GetHSEData(_ViewListHSEHour, Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo), db.Sections.Where(aa => aa.CompanyID == 1).ToList()), _dateFrom + " to " + _dateTo);
                        break;
                    case "THSEHour":
                        _periodFrom = Convert.ToDateTime(_dateFrom).ToString("yyyy-MMM-dd");
                        _periodTo = Convert.ToDateTime(_dateTo).ToString("yyyy-MMM-dd");

                        dt = qb.GetValuesfromDB("select * from ViewHSEHour  where HSEDate >='" + _periodFrom + "'and HSEDate <= '" + _periodTo + "' and Total>0 and Type='T'");
                        //dt = qb.GetValuesfromDB("select * from ViewHSEHour  where HSEDate ='" + _periodFrom + "'and Total>0");
                        _ViewListHSEHour = dt.ToList<ViewHSEHour>();
                        title = "HSE Hours Report (Township)";

                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRHSEHourS.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRHSEHourS.rdlc";
                        LoadReport(PathString, GetHSEData(_ViewListHSEHour, Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo), db.Sections.Where(aa => aa.CompanyID == 1).ToList()), _dateFrom);
                        break;
                    case "THSEHour_con":
                        _periodFrom = Convert.ToDateTime(_dateFrom).ToString("yyyy-MMM-dd");
                        _periodTo = Convert.ToDateTime(_dateTo).ToString("yyyy-MMM-dd");
                        dt = qb.GetValuesfromDB("select * from ViewHSEHour  where HSEDate >='" + _periodFrom + "'and HSEDate <= '" + _periodTo + "' and Total>0 and Type='T'");
                        _ViewListHSEHour = dt.ToList<ViewHSEHour>();
                        title = "HSE Hours Report (Township)";

                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRHSEHourC.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRHSEHourC.rdlc";
                        LoadReport(PathString, GetHSEData(_ViewListHSEHour, Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo), db.Sections.Where(aa => aa.CompanyID == 1).ToList()), _dateFrom + " to " + _dateTo);
                        break;
                    #endregion
                    #region -- DOJ/ DOR --
                    case "mrSum_DOR":
                        DateTime dtS = Convert.ToDateTime(_dateFrom);
                        DateTime dtE = Convert.ToDateTime(_dateTo);
                        dt = qb.GetValuesfromDB("select * from EmpView " + query + " and ResignDate >='" + _dateFrom + "' and ResignDate<='" + _dateTo + "'");
                        _ViewList = dt.ToList<EmpView>();

                        _TempViewList = new List<EmpView>();
                        title = "List of Resigned Employees Report ";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDOJDOR.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDOJDOR.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);

                        break;
                    case "mrSum_DOJ":
                        dtS = Convert.ToDateTime(_dateFrom);
                        dtE = Convert.ToDateTime(_dateTo);
                        dt = qb.GetValuesfromDB("select * from EmpView " + query + " and JoinDate >='" + _dateFrom + "' and JoinDate<='" + _dateTo + "'");
                        _ViewList = dt.ToList<EmpView>();

                        _TempViewList = new List<EmpView>();
                        title = "List of Active Employees Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDOJDOR.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDOJDOR.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);

                        break;
                    #endregion
                    case "device_in_out": dt = qb.GetValuesfromDB("select * from ViewPollData" + " where (EntDate >= " + "'" + _dateFrom + "'" + " and EntDate <= " + "'" + _dateTo + "'" + " )");
                        title = "Device Wise In/Out Report";
                        List<ViewPollData> _ViewList33 = dt.ToList<ViewPollData>();
                        List<ViewPollData> _TempViewList33 = new List<ViewPollData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRPollData.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRPollData.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList33, _ViewList33), _dateFrom + " TO " + _dateTo);
                        break;
                    case "mrJCSummary":
                        dt = qb.GetValuesfromDB("select * from EmpView " + query + "and Status=1");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        List<JobCardEmp> jcEmps = new List<JobCardEmp>();
                        dt1 = qb.GetValuesfromDB("select * from JobCardEmp where Dated>='" + _dateFrom + "' and Dated<='" + _dateTo + "'");
                        jcEmps = dt1.ToList<JobCardEmp>();
                        dt2 = qb.GetValuesfromDB("select * from AttDataManEdit where NewTimeIn>='" + _dateFrom + "' and NewTimeIn<='" + _dateTo + " 23:59:00'");
                        title = "Job Card Summary";
                        List<AttDataManEdit> attedit = dt2.ToList<AttDataManEdit>();
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRJCSummData.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRJCSummData.rdlc";
                        _TempViewList = ReportsFilterImplementation(fm, _TempViewList, _ViewList);
                        List<ModelJCSummaryData> vms = GetJCSummaryData(attedit, jcEmps, _TempViewList, Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo));
                        LoadReport(PathString, vms, _dateFrom + " TO " + _dateTo);

                        break;
                }

            }
        }

        #endregion

        #region -- Load report Functions---

        #region load report function but not to be identified (?)
        private void monthlyProductivityProcess(String _dateFrom, String _dateTo, String query)
        {
            QueryBuilder qb = new QueryBuilder();
            string PathString = "";
            DataTable dt4 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )");
            List<ViewAttData> ListOfAttDate = new List<ViewAttData>();
            List<ViewAttData> TempList = new List<ViewAttData>();
            List<ViewAttData> finalOutput = new List<ViewAttData>();
            title = "Employee Attendace Summary New";
            if (GlobalVariables.DeploymentType == false)
                PathString = "/Reports/RDLC/MonthlyProductivityEmployees.rdlc";
            else
                PathString = "/WMS/Reports/RDLC/MonthlyProductivityEmployees.rdlc";
            ListOfAttDate = dt4.ToList<ViewAttData>();
            TempList = new List<ViewAttData>();

            finalOutput = ReportsFilterImplementation(fm, TempList, ListOfAttDate);
            List<EmpMonthlyProductivityEntity> empe = new List<EmpMonthlyProductivityEntity>();
            empe = EmpMonthlyProductivityEntity.ProcessAttendence(finalOutput, _dateFrom, _dateTo);
            LoadReport(empe, PathString, _dateFrom, _dateTo);



        }
        #endregion

        #region -- Monthly Consolidated report With  Badli Days Active and Non-Active Employees--Load Report
        private void LoadReport(string path, List<ViewAttMonthBadli> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewAttMonthBadli> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }


        #endregion

        #region -- Not Good Placement of Load Reports --

        private void LoadReport(string PathString, List<ViewHSEHour> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewHSEHour> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<ModelHSEDataEmp> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ModelHSEDataEmp> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, List<ModelJCSummaryData> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ModelJCSummaryData> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, List<ModelEditAttAudit> mdea, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ModelEditAttAudit> ie;
            ie = mdea.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, List<ViewEmpAttMnAppOT> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewEmpAttMnAppOT> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, List<ViewAttDataOT> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewAttDataOT> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, List<ViewFatimaPayroll> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewFatimaPayroll> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, List<ViewAttBadliMonth> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewAttBadliMonth> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, FiltersModel fm, List<ViewFatimaPayroll> _TempViewPayroll, List<ViewFatimaPayroll> _ViewListPayroll)
        {
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewFatimaPayroll> ie;
            ie = _ViewListPayroll.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, List<ViewAttDataAppOT> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewAttDataAppOT> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.HyperlinkTarget = "_blank";


            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(List<EmpMonthlyProductivityEntity> empe, string PathString, string _dateFrom, string _dateTo)
        {
            string Header = "Monthly Productivity Sheet";
            this.ReportViewer1.LocalReport.DisplayName = "Monthly Productivity Sheet";
            this.ReportViewer1.ProcessingMode = ProcessingMode.Local;
            string Month = Convert.ToDateTime(_dateFrom).Date.ToString("MMMM-yyyy");
            Month = "For the Month of " + Month;
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            this.ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<EmpMonthlyProductivityEntity> ie;
            ie = empe.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();

            ReportDataSource datasource1 = new ReportDataSource("DataSet2", companyImage);
            ReportDataSource datasource2 = new ReportDataSource("DataSet1", ie);

            this.ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            this.ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", Month, false);
            ReportParameter rp1 = new ReportParameter("Header", Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp1, rp });
            this.ReportViewer1.LocalReport.Refresh();


            //this.ReportViewer1.LocalReport.DisplayName = title;
            //ReportViewer1.ProcessingMode = ProcessingMode.Local;
            //ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            //System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            //ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            //IEnumerable<DailySummary> ie;
            //ie = list.AsQueryable();
            //IEnumerable<EmpPhoto> companyImage;
            //companyImage = companyimage.AsQueryable();
            //ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            //ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            //ReportViewer1.LocalReport.DataSources.Clear();
            //ReportViewer1.LocalReport.EnableExternalImages = true;
            //ReportViewer1.LocalReport.DataSources.Add(datasource1);
            //ReportViewer1.LocalReport.DataSources.Add(datasource2);
            //ReportParameter rp = new ReportParameter("Date", p, false);
            //ReportParameter rp1 = new ReportParameter("Header", Header, false);
            //this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp1, rp });
            //ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, List<ViewPollData> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewPollData> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }


        private void LoadReports(string path, DataTable _Summary, DateTime? dtFrom, DateTime? dtTo)
        {
            string _Header = "Employee Attendance Summary";
            this.ReportViewer1.LocalReport.DisplayName = "Employee Attendance Summary";
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", _Summary);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", dtFrom.Value.ToString("dd-MMM-yyyy") + " TO" + dtTo.Value.ToString("dd-MMM-yyyy"), false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(DataTable dataTable, string PathString, string p)
        {
            string _Header = "Flexy Monthly Sheet";
            //string Date = Convert.ToDateTime(_dateFrom).Date.ToString("dd-MMM-yyyy");
            this.ReportViewer1.LocalReport.DisplayName = "Flexy Monthly Sheet";
            string Date = p;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", dataTable);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.HyperlinkTarget = "_blank";
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", Date, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, List<DailySummary> list, string p, string Header)
        {
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<DailySummary> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", p, false);
            ReportParameter rp1 = new ReportParameter("Header", Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp1, rp });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<ViewBadli> list, string p)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewBadli> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", p, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp1, rp });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<TASReportDataSet.SummarizedMonthlyReportDataTable> VMLD, string p)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<TASReportDataSet.SummarizedMonthlyReportDataTable> ie;
            ie = VMLD.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<AttDeptSummary> AttDept, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<AttDeptSummary> ie;
            ie = AttDept.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Title", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<ViewMultipleInOut> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewMultipleInOut> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, List<ViewDetailAttData> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewDetailAttData> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);

            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, List<ViewMonthlyData> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewMonthlyData> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            date = "";
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, List<ViewMonthlyDataPer> _Employee, String date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;

            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewMonthlyDataPer> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", date, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string path, DataTable _LvSummary)
        {
            string _Header = "Year wise Leaves Summary";
            this.ReportViewer1.LocalReport.DisplayName = "Leave Summary Report";
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", _LvSummary);
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, List<EmpView> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<EmpView> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.HyperlinkTarget = "_blank";


            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string path, List<ViewAttData> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewAttData> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.HyperlinkTarget = "_blank";


            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string path, List<ViewAttDataOld1> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewAttDataOld1> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.HyperlinkTarget = "_blank";


            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, DataTable _LvSummary, int i)
        {
            string _Header = "Monthly Leaves Sheet";
            //string Date = Convert.ToDateTime(_dateFrom).Date.ToString("dd-MMM-yyyy");
            this.ReportViewer1.LocalReport.DisplayName = "Leave Balance Report";
            string Date = "Month: " + Convert.ToDateTime(_dateFrom).Date.ToString("MMMM");
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", _LvSummary);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.HyperlinkTarget = "_blank";
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", Date, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string path, List<ViewLvApplication> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewLvApplication> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, List<ModelAppMonthlyOT> md, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ModelAppMonthlyOT> ie;
            ie = md.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<ViewEditAttendance> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewEditAttendance> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);

            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<VMBadliRecord> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<VMBadliRecord> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);

            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<ViewJobCardAudit> list, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewJobCardAudit> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        #endregion

        #endregion

        #region -- Helping Functions With sub heading Reports region

        #region Excel Report Download

        private void DownloadReport(List<ViewMonthlyData> list, string p, int UserID)
        {
            string val = "";
            list = list.OrderBy(aa => aa.DeptID).ThenBy(aa => aa.SecID).ThenBy(aa => aa.EmpID).ToList();
            val = DownloadData(list, p, UserID);
            if (val != "")
            {
                Response.ContentType = ContentType;
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + val);
                Response.WriteFile(val);
                Response.End();
            }
        }
        private void DownloadBadliReport(List<ViewAttMonthBadli> list, string p, int UserID)
        {
            string val = "";
            list = list.OrderBy(aa => aa.DeptID).ThenBy(aa => aa.SecID).ThenBy(aa => aa.EmpID).ToList();
            val = DownloadBadliData(list, p, UserID);
            if (val != "")
            {
                Response.ContentType = ContentType;
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + val);
                Response.WriteFile(val);
                Response.End();
            }
        }
        private string DownloadBadliData(List<ViewAttMonthBadli> MonthlyDatas, string Date, int UserID)
        {
            string retVal = "";
            //lvPDF is nothing but the listview control name
            string[] st = new string[5];
            DirectoryInfo di = new DirectoryInfo(@"D:\Reports\");
            if (di.Exists == false)
                di.Create();
            StreamWriter sw = new StreamWriter(@"D:\Reports\MonthlyReportB" + UserID + MonthlyDatas.FirstOrDefault().StartDate.Value.Month.ToString("00") + ".xls", false);
            sw.AutoFlush = true;
            List<string> ColumnsName = new List<string>();
            ColumnsName.Add("EmpNo");
            ColumnsName.Add("Name");
            ColumnsName.Add("Designation");
            ColumnsName.Add("Section");
            ColumnsName.Add("Department");
            ColumnsName.Add("Type");
            ColumnsName.Add("1");
            ColumnsName.Add("2");
            ColumnsName.Add("3");
            ColumnsName.Add("4");
            ColumnsName.Add("5");
            ColumnsName.Add("6");
            ColumnsName.Add("7");
            ColumnsName.Add("8");
            ColumnsName.Add("9");
            ColumnsName.Add("10");
            ColumnsName.Add("11");
            ColumnsName.Add("12");
            ColumnsName.Add("13");
            ColumnsName.Add("14");
            ColumnsName.Add("15");
            ColumnsName.Add("16");
            ColumnsName.Add("17");
            ColumnsName.Add("18");
            ColumnsName.Add("19");
            ColumnsName.Add("20");
            ColumnsName.Add("21");
            ColumnsName.Add("22");
            ColumnsName.Add("23");
            ColumnsName.Add("24");
            ColumnsName.Add("25");
            ColumnsName.Add("26");
            ColumnsName.Add("27");
            ColumnsName.Add("28");
            ColumnsName.Add("29");
            ColumnsName.Add("30");
            ColumnsName.Add("31");
            ColumnsName.Add("PR");
            ColumnsName.Add("AB");
            ColumnsName.Add("LV");
            ColumnsName.Add("HLV");
            ColumnsName.Add("DO");
            ColumnsName.Add("GZ");
            ColumnsName.Add("PD");
            ColumnsName.Add("BadliDays");
            ColumnsName.Add("Total");
            ColumnsName.Add("N-OT");
            ColumnsName.Add("G-OT");
            ColumnsName.Add("E-In");
            ColumnsName.Add("E-Out");
            ColumnsName.Add("L-In");
            ColumnsName.Add("A-NOT");
            ColumnsName.Add("A-GOT");
            string ColumnString = "";
            for (int i = 0; i < ColumnsName.Count; i++)
            {
                ColumnString = ColumnString + ColumnsName[i] + "\t";
            }
            sw.Write(Date + "\n");
            sw.Write(ColumnString + "\n");
            foreach (var item in MonthlyDatas)
            {
                string Personal = item.EmpNo + "\t" + item.EmpName + "\t" + item.DesignationName + "\t" + item.SectionName + "\t" + item.DeptName + "\t" + item.TypeName;
                string DaysInfo = item.D1 + "\t" + item.D2 + "\t" + item.D3 + "\t" + item.D4 + "\t" + item.D5 + "\t" + item.D6 + "\t" + item.D7 + "\t" + item.D8 + "\t" + item.D9 + "\t" + item.D10
                    + "\t" + item.D11 + "\t" + item.D12 + "\t" + item.D13 + "\t" + item.D14 + "\t" + item.D15 + "\t" + item.D16 + "\t" + item.D17 + "\t" + item.D18 + "\t" + item.D19 + "\t" + item.D20
                    + "\t" + item.D21 + "\t" + item.D22 + "\t" + item.D23 + "\t" + item.D24 + "\t" + item.D25 + "\t" + item.D26 + "\t" + item.D27 + "\t" + item.D28 + "\t" + item.D29 + "\t" + item.D30 + "\t" + item.D31;
                string TotalsDays = GetDaysTotal(item);
                string OTLine = GetOTLine(item);
                sw.WriteLine(Personal + "\t" + DaysInfo + "\t" + TotalsDays);
                sw.WriteLine("\t\t\t\t\t\t" + OTLine);
            }
            sw.Close();
            FileInfo fil = new FileInfo(@"D:\Reports\MonthlyReportB" + UserID + MonthlyDatas.FirstOrDefault().StartDate.Value.Month.ToString("00") + ".xls");
            if (fil.Exists == true)
            {
                retVal = "D:\\Reports\\MonthlyReportB" + UserID + MonthlyDatas.FirstOrDefault().StartDate.Value.Month.ToString("00") + ".xls";
            }
            return retVal;
        }

        private string GetOTLine(ViewAttMonthBadli item)
        {
            string val = "";

            TimeSpan ts1 = new TimeSpan();
            string ot1 = "";
            if (item.OT1 > 0)
            {
                ts1 = new TimeSpan(0, (int)item.OT1, 0);
                ot1 = ts1.Hours.ToString("00") + ":" + ts1.Minutes.ToString("00");
            }
            TimeSpan ts2 = new TimeSpan();
            string ot2 = "";
            if (item.OT2 > 0)
            {
                ts2 = new TimeSpan(0, (int)item.OT2, 0);
                ot2 = ts2.Hours.ToString("00") + ":" + ts2.Minutes.ToString("00");
            }

            TimeSpan ts3 = new TimeSpan();
            string ot3 = "";
            if (item.OT3 > 0)
            {
                ts3 = new TimeSpan(0, (int)item.OT3, 0);
                ot3 = ts3.Hours.ToString("00") + ":" + ts3.Minutes.ToString("00");
            }
            TimeSpan ts4 = new TimeSpan();
            string ot4 = "";
            if (item.OT4 > 0)
            {
                ts4 = new TimeSpan(0, (int)item.OT4, 0);
                ot4 = ts4.Hours.ToString("00") + ":" + ts4.Minutes.ToString("00");
            }
            TimeSpan ts5 = new TimeSpan();
            string ot5 = "";
            if (item.OT5 > 0)
            {
                ts5 = new TimeSpan(0, (int)item.OT5, 0);
                ot5 = ts5.Hours.ToString("00") + ":" + ts5.Minutes.ToString("00");
            }
            TimeSpan ts6 = new TimeSpan();
            string ot6 = "";
            if (item.OT6 > 0)
            {
                ts6 = new TimeSpan(0, (int)item.OT6, 0);
                ot6 = ts6.Hours.ToString("00") + ":" + ts6.Minutes.ToString("00");
            }
            TimeSpan ts7 = new TimeSpan();
            string ot7 = "";
            if (item.OT7 > 0)
            {
                ts7 = new TimeSpan(0, (int)item.OT7, 0);
                ot7 = ts7.Hours.ToString("00") + ":" + ts7.Minutes.ToString("00");
            }
            TimeSpan ts8 = new TimeSpan();
            string ot8 = "";
            if (item.OT8 > 0)
            {
                ts8 = new TimeSpan(0, (int)item.OT8, 0);
                ot8 = ts8.Hours.ToString("00") + ":" + ts8.Minutes.ToString("00");
            }
            TimeSpan ts9 = new TimeSpan();
            string ot9 = "";
            if (item.OT9 > 0)
            {
                ts9 = new TimeSpan(0, (int)item.OT9, 0);
                ot9 = ts9.Hours.ToString("00") + ":" + ts9.Minutes.ToString("00");
            } TimeSpan ts10 = new TimeSpan();
            string ot10 = "";
            if (item.OT10 > 0)
            {
                ts10 = new TimeSpan(0, (int)item.OT10, 0);
                ot10 = ts10.Hours.ToString("00") + ":" + ts10.Minutes.ToString("00");
            } TimeSpan ts11 = new TimeSpan();
            string ot11 = "";
            if (item.OT11 > 0)
            {
                ts11 = new TimeSpan(0, (int)item.OT11, 0);
                ot11 = ts11.Hours.ToString("00") + ":" + ts11.Minutes.ToString("00");
            } TimeSpan ts12 = new TimeSpan();
            string ot12 = "";
            if (item.OT12 > 0)
            {
                ts12 = new TimeSpan(0, (int)item.OT12, 0);
                ot12 = ts12.Hours.ToString("00") + ":" + ts12.Minutes.ToString("00");
            }
            TimeSpan ts13 = new TimeSpan();
            string ot13 = "";
            if (item.OT13 > 0)
            {
                ts13 = new TimeSpan(0, (int)item.OT13, 0);
                ot13 = ts13.Hours.ToString("00") + ":" + ts13.Minutes.ToString("00");
            }
            TimeSpan ts14 = new TimeSpan();
            string ot14 = "";
            if (item.OT14 > 0)
            {
                ts14 = new TimeSpan(0, (int)item.OT14, 0);
                ot14 = ts14.Hours.ToString("00") + ":" + ts14.Minutes.ToString("00");
            }
            TimeSpan ts15 = new TimeSpan();
            string ot15 = "";
            if (item.OT15 > 0)
            {
                ts15 = new TimeSpan(0, (int)item.OT15, 0);
                ot15 = ts15.Hours.ToString("00") + ":" + ts15.Minutes.ToString("00");
            }
            TimeSpan ts16 = new TimeSpan();
            string ot16 = "";
            if (item.OT16 > 0)
            {
                ts16 = new TimeSpan(0, (int)item.OT16, 0);
                ot16 = ts16.Hours.ToString("00") + ":" + ts16.Minutes.ToString("00");
            }
            TimeSpan ts17 = new TimeSpan();
            string ot17 = "";
            if (item.OT17 > 0)
            {
                ts17 = new TimeSpan(0, (int)item.OT17, 0);
                ot17 = ts17.Hours.ToString("00") + ":" + ts17.Minutes.ToString("00");
            }
            TimeSpan ts18 = new TimeSpan();
            string ot18 = "";
            if (item.OT18 > 0)
            {
                ts18 = new TimeSpan(0, (int)item.OT18, 0);
                ot18 = ts18.Hours.ToString("00") + ":" + ts18.Minutes.ToString("00");
            }
            TimeSpan ts19 = new TimeSpan();
            string ot19 = "";
            if (item.OT19 > 0)
            {
                ts19 = new TimeSpan(0, (int)item.OT19, 0);
                ot19 = ts19.Hours.ToString("00") + ":" + ts19.Minutes.ToString("00");
            }
            TimeSpan ts20 = new TimeSpan();
            string ot20 = "";
            if (item.OT20 > 0)
            {
                ts20 = new TimeSpan(0, (int)item.OT20, 0);
                ot20 = ts20.Hours.ToString("00") + ":" + ts20.Minutes.ToString("00");
            }
            TimeSpan ts21 = new TimeSpan();
            string ot21 = "";
            if (item.OT21 > 0)
            {
                ts21 = new TimeSpan(0, (int)item.OT21, 0);
                ot21 = ts21.Hours.ToString("00") + ":" + ts21.Minutes.ToString("00");
            }
            TimeSpan ts22 = new TimeSpan();
            string ot22 = "";
            if (item.OT22 > 0)
            {
                ts22 = new TimeSpan(0, (int)item.OT22, 0);
                ot22 = ts22.Hours.ToString("00") + ":" + ts22.Minutes.ToString("00");
            }
            TimeSpan ts23 = new TimeSpan();
            string ot23 = "";
            if (item.OT23 > 0)
            {
                ts23 = new TimeSpan(0, (int)item.OT23, 0);
                ot23 = ts23.Hours.ToString("00") + ":" + ts23.Minutes.ToString("00");
            }
            TimeSpan ts24 = new TimeSpan();
            string ot24 = "";
            if (item.OT24 > 0)
            {
                ts24 = new TimeSpan(0, (int)item.OT24, 0);
                ot24 = ts24.Hours.ToString("00") + ":" + ts24.Minutes.ToString("00");
            }
            TimeSpan ts25 = new TimeSpan();
            string ot25 = "";
            if (item.OT25 > 0)
            {
                ts25 = new TimeSpan(0, (int)item.OT25, 0);
                ot25 = ts25.Hours.ToString("00") + ":" + ts25.Minutes.ToString("00");
            }
            TimeSpan ts26 = new TimeSpan();
            string ot26 = "";
            if (item.OT26 > 0)
            {
                ts26 = new TimeSpan(0, (int)item.OT26, 0);
                ot26 = ts26.Hours.ToString("00") + ":" + ts26.Minutes.ToString("00");
            }
            TimeSpan ts27 = new TimeSpan();
            string ot27 = "";
            if (item.OT27 > 0)
            {
                ts27 = new TimeSpan(0, (int)item.OT27, 0);
                ot27 = ts27.Hours.ToString("00") + ":" + ts27.Minutes.ToString("00");
            }
            TimeSpan ts28 = new TimeSpan();
            string ot28 = "";
            if (item.OT28 > 0)
            {
                ts28 = new TimeSpan(0, (int)item.OT28, 0);
                ot28 = ts28.Hours.ToString("00") + ":" + ts28.Minutes.ToString("00");
            }
            TimeSpan ts29 = new TimeSpan();
            string ot29 = "";
            if (item.OT29 > 0)
            {
                ts29 = new TimeSpan(0, (int)item.OT29, 0);
                ot29 = ts29.Hours.ToString("00") + ":" + ts29.Minutes.ToString("00");
            }
            TimeSpan ts30 = new TimeSpan();
            string ot30 = "";
            if (item.OT30 > 0)
            {
                ts30 = new TimeSpan(0, (int)item.OT30, 0);
                ot30 = ts30.Hours.ToString("00") + ":" + ts30.Minutes.ToString("00");
            }
            TimeSpan ts31 = new TimeSpan();
            string ot31 = "";
            if (item.OT31 > 0)
            {
                ts31 = new TimeSpan(0, (int)item.OT31, 0);
                ot31 = ts31.Hours.ToString("00") + ":" + ts31.Minutes.ToString("00");
            }
            string Lv1 = "";
            if (item.L1 != null)
                Lv1 = item.L1;
            string Lv2 = "";
            if (item.L2 != null)
                Lv2 = item.L2;
            string Lv3 = "";
            if (item.L3 != null)
                Lv3 = item.L3;
            string Lv4 = "";
            if (item.L4 != null)
                Lv4 = item.L4;
            string Lv5 = "";
            if (item.L5 != null)
                Lv5 = item.L5;
            string Lv6 = "";
            if (item.L6 != null)
                Lv6 = item.L6;
            string Lv7 = "";
            if (item.L7 != null)
                Lv7 = item.L7;
            string Lv8 = "";
            if (item.L8 != null)
                Lv8 = item.L8;
            string Lv9 = "";
            if (item.L9 != null)
                Lv9 = item.L9;
            string Lv10 = "";
            if (item.L10 != null)
                Lv10 = item.L10;
            string Lv11 = "";
            if (item.L11 != null)
                Lv11 = item.L11;
            string Lv12 = "";
            if (item.L12 != null)
                Lv12 = item.L12;
            string Lv13 = "";
            if (item.L13 != null)
                Lv13 = item.L13;
            string Lv14 = "";
            if (item.L14 != null)
                Lv14 = item.L14;
            string Lv15 = "";
            if (item.L15 != null)
                Lv15 = item.L15;

            string Lv16 = "";
            if (item.L16 != null)
                Lv16 = item.L16;
            string Lv17 = "";
            if (item.L17 != null)
                Lv17 = item.L17;
            string Lv18 = "";
            if (item.L18 != null)
                Lv18 = item.L18;
            string Lv19 = "";
            if (item.L19 != null)
                Lv19 = item.L19;
            string Lv20 = "";
            if (item.L20 != null)
                Lv20 = item.L20;
            string Lv21 = "";
            if (item.L21 != null)
                Lv21 = item.L21;
            string Lv22 = "";
            if (item.L22 != null)
                Lv22 = item.L22;
            string Lv23 = "";
            if (item.L23 != null)
                Lv23 = item.L23;
            string Lv24 = "";
            if (item.L24 != null)
                Lv24 = item.L24;
            string Lv25 = "";
            if (item.L25 != null)
                Lv25 = item.L25;
            string Lv26 = "";
            if (item.L26 != null)
                Lv26 = item.L26;
            string Lv27 = "";
            if (item.L27 != null)
                Lv27 = item.L27;
            string Lv28 = "";
            if (item.L28 != null)
                Lv28 = item.L28;
            string Lv29 = "";
            if (item.L29 != null)
                Lv29 = item.L29;
            string Lv30 = "";
            if (item.L30 != null)
                Lv30 = item.L30;
            string Lv31 = "";
            if (item.L31 != null)
                Lv31 = item.L31;
            val = ot1 + Lv1 + "\t" + ot2 + Lv2 + "\t" + ot3 + Lv3 + "\t" + ot4 + Lv4 + "\t" + ot5 + Lv5 + "\t" + ot6 + Lv6 + "\t" + ot7 + Lv7 + "\t" + ot8 + Lv8 + "\t" + ot9 + Lv9 + "\t" +
                ot10 + Lv10 + "\t" + ot11 + Lv11 + "\t" + ot12 + Lv12 + "\t" + ot13 + Lv13 + "\t" + ot14 + Lv14 + "\t" + ot15 + Lv15 + "\t" + ot16 + Lv16 + "\t" + ot17 + Lv17 + "\t" +
                ot18 + Lv18 + "\t" + ot19 + Lv19 + "\t" + ot20 + Lv20 + "\t" + ot21 + Lv21 + "\t" + ot22 + Lv22 + "\t" + ot23 + Lv23 + "\t" + ot24 + Lv24 + "\t" + ot25 + Lv25 + "\t" +
                ot26 + Lv26 + "\t" + ot27 + Lv27 + "\t" + ot28 + Lv28 + "\t" + ot29 + Lv29 + "\t" + ot30 + Lv30 + "\t" + ot31 + Lv31 + "\t";

            return val;
        }

        private string getTotalHours(ViewAttMonthBadli item)
        {
            TimeSpan tsNOT = new TimeSpan();
            TimeSpan tsGOT = new TimeSpan();
            TimeSpan tsEI = new TimeSpan();
            TimeSpan tsEO = new TimeSpan();
            TimeSpan tsLI = new TimeSpan();
            TimeSpan tsANOT = new TimeSpan();
            TimeSpan tsAGOT = new TimeSpan();

            string anot = "";
            string agot = "";
            string not = "";
            string got = "";
            string ei = "";
            string eo = "";
            string li = "";

            if (item.TNOT > 0)
            {
                tsNOT = new TimeSpan(0, (int)item.TNOT, 0);
                int hours = (int)tsNOT.TotalHours;
                int min = (int)(item.TNOT - (hours * 60));
                not = hours.ToString() + ":" + min.ToString();
            }
            if (item.TGZOT > 0)
            {
                tsGOT = new TimeSpan(0, (int)item.TGZOT, 0);
                int hours = (int)tsGOT.TotalHours;
                int min = (int)(item.TGZOT - (hours * 60));
                got = hours.ToString() + ":" + min.ToString();
            }
            if (item.TEarlyIn > 0)
            {
                tsEI = new TimeSpan(0, (int)item.TEarlyIn, 0);
                int hours = (int)tsEI.TotalHours;
                int min = (int)(item.TEarlyIn - (hours * 60));
                ei = hours.ToString() + ":" + min.ToString();
            }
            if (item.TEarlyOut > 0)
            {
                tsEO = new TimeSpan(0, (int)item.TEarlyOut, 0);
                int hours = (int)tsEO.TotalHours;
                int min = (int)(item.TEarlyOut - (hours * 60));
                eo = hours.ToString() + ":" + min.ToString();
            }
            if (item.TLateIn > 0)
            {
                tsLI = new TimeSpan(0, (int)item.TLateIn, 0);
                int hours = (int)tsLI.TotalHours;
                int min = (int)(item.TLateIn - (hours * 60));
                li = hours.ToString() + ":" + min.ToString();
            }
            if (item.TAppNOT > 0)
            {
                tsANOT = new TimeSpan(0, (int)item.TAppNOT, 0);
                int hours = (int)tsANOT.TotalHours;
                int min = (int)(item.TAppNOT - (hours * 60));
                anot = hours.ToString() + ":" + min.ToString();
            }
            if (item.TAppGOT > 0)
            {
                tsAGOT = new TimeSpan(0, (int)item.TAppGOT, 0);
                int hours = (int)tsAGOT.TotalHours;
                int min = (int)(item.TAppGOT - (hours * 60));
                agot = hours.ToString() + ":" + min.ToString();
            }
            string val = "";
            val = not + "\t"
            + got + "\t"
            + ei + "\t"
            + eo + "\t"
            + li + "\t"
            + anot + "\t"
            + agot;
            return val;
        }

        private string GetDaysTotal(ViewAttMonthBadli item)
        {
            string val = "";
            val = item.PreDays + "\t" + item.AbDays + "\t" + item.LeaveDays + "\t" + item.HalfLeavesDay + "\t" + item.RestDays + "\t" + item.GZDays + "\t" + item.WorkDays + "\t" + item.BadliDays + "\t" + item.TotalDays;
            return val;
        }
        public string DownloadData(List<ViewMonthlyData> MonthlyDatas, string Date, int UserID)
        {
            string retVal = "";
            //lvPDF is nothing but the listview control name
            string[] st = new string[5];
            DirectoryInfo di = new DirectoryInfo(@"D:\Reports\");
            if (di.Exists == false)
                di.Create();
            StreamWriter sw = new StreamWriter(@"D:\Reports\MonthlyReport" + UserID + MonthlyDatas.FirstOrDefault().StartDate.Value.Month.ToString("00") + ".xls", false);
            sw.AutoFlush = true;
            List<string> ColumnsName = new List<string>();
            ColumnsName.Add("EmpNo");
            ColumnsName.Add("Name");
            ColumnsName.Add("Designation");
            ColumnsName.Add("Section");
            ColumnsName.Add("Department");
            ColumnsName.Add("Type");
            ColumnsName.Add("1");
            ColumnsName.Add("2");
            ColumnsName.Add("3");
            ColumnsName.Add("4");
            ColumnsName.Add("5");
            ColumnsName.Add("6");
            ColumnsName.Add("7");
            ColumnsName.Add("8");
            ColumnsName.Add("9");
            ColumnsName.Add("10");
            ColumnsName.Add("11");
            ColumnsName.Add("12");
            ColumnsName.Add("13");
            ColumnsName.Add("14");
            ColumnsName.Add("15");
            ColumnsName.Add("16");
            ColumnsName.Add("17");
            ColumnsName.Add("18");
            ColumnsName.Add("19");
            ColumnsName.Add("20");
            ColumnsName.Add("21");
            ColumnsName.Add("22");
            ColumnsName.Add("23");
            ColumnsName.Add("24");
            ColumnsName.Add("25");
            ColumnsName.Add("26");
            ColumnsName.Add("27");
            ColumnsName.Add("28");
            ColumnsName.Add("29");
            ColumnsName.Add("30");
            ColumnsName.Add("31");
            ColumnsName.Add("PR");
            ColumnsName.Add("AB");
            ColumnsName.Add("LV");
            ColumnsName.Add("HLV");
            ColumnsName.Add("DO");
            ColumnsName.Add("GZ");
            ColumnsName.Add("PD");
            ColumnsName.Add("Total");
            ColumnsName.Add("N-OT");
            ColumnsName.Add("G-OT");
            ColumnsName.Add("E-In");
            ColumnsName.Add("E-Out");
            ColumnsName.Add("L-In");
            string ColumnString = "";
            for (int i = 0; i < ColumnsName.Count; i++)
            {
                ColumnString = ColumnString + ColumnsName[i] + "\t";
            }
            sw.Write(Date + "\n");
            sw.Write(ColumnString + "\n");
            foreach (var item in MonthlyDatas)
            {
                string Personal = item.EmpNo + "\t" + item.EmpName + "\t" + item.DesignationName + "\t" + item.SectionName + "\t" + item.DeptName + "\t" + item.TypeName;
                string DaysInfo = item.D1 + "\t" + item.D2 + "\t" + item.D3 + "\t" + item.D4 + "\t" + item.D5 + "\t" + item.D6 + "\t" + item.D7 + "\t" + item.D8 + "\t" + item.D9 + "\t" + item.D10
                    + "\t" + item.D11 + "\t" + item.D12 + "\t" + item.D13 + "\t" + item.D14 + "\t" + item.D15 + "\t" + item.D16 + "\t" + item.D17 + "\t" + item.D18 + "\t" + item.D19 + "\t" + item.D20
                    + "\t" + item.D21 + "\t" + item.D22 + "\t" + item.D23 + "\t" + item.D24 + "\t" + item.D25 + "\t" + item.D26 + "\t" + item.D27 + "\t" + item.D28 + "\t" + item.D29 + "\t" + item.D30 + "\t" + item.D31;
                string TotalsDays = GetDaysTotal(item);
                string OTLine = GetOTLine(item);
                sw.WriteLine(Personal + "\t" + DaysInfo + "\t" + TotalsDays);
                sw.WriteLine("\t\t\t\t\t\t" + OTLine);
            }
            sw.Close();
            FileInfo fil = new FileInfo(@"D:\Reports\MonthlyReport" + UserID + MonthlyDatas.FirstOrDefault().StartDate.Value.Month.ToString("00") + ".xls");
            if (fil.Exists == true)
            {
                retVal = "D:\\Reports\\MonthlyReport" + UserID + MonthlyDatas.FirstOrDefault().StartDate.Value.Month.ToString("00") + ".xls";
            }
            return retVal;
        }

        private string GetOTLine(ViewMonthlyData item)
        {
            string val = "";

            TimeSpan ts1 = new TimeSpan();
            string ot1 = "";
            if (item.OT1 > 0)
            {
                ts1 = new TimeSpan(0, (int)item.OT1, 0);
                ot1 = ts1.Hours.ToString("00") + ":" + ts1.Minutes.ToString("00");
            }
            TimeSpan ts2 = new TimeSpan();
            string ot2 = "";
            if (item.OT2 > 0)
            {
                ts2 = new TimeSpan(0, (int)item.OT2, 0);
                ot2 = ts2.Hours.ToString("00") + ":" + ts2.Minutes.ToString("00");
            }

            TimeSpan ts3 = new TimeSpan();
            string ot3 = "";
            if (item.OT3 > 0)
            {
                ts3 = new TimeSpan(0, (int)item.OT3, 0);
                ot3 = ts3.Hours.ToString("00") + ":" + ts3.Minutes.ToString("00");
            }
            TimeSpan ts4 = new TimeSpan();
            string ot4 = "";
            if (item.OT4 > 0)
            {
                ts4 = new TimeSpan(0, (int)item.OT4, 0);
                ot4 = ts4.Hours.ToString("00") + ":" + ts4.Minutes.ToString("00");
            }
            TimeSpan ts5 = new TimeSpan();
            string ot5 = "";
            if (item.OT5 > 0)
            {
                ts5 = new TimeSpan(0, (int)item.OT5, 0);
                ot5 = ts5.Hours.ToString("00") + ":" + ts5.Minutes.ToString("00");
            }
            TimeSpan ts6 = new TimeSpan();
            string ot6 = "";
            if (item.OT6 > 0)
            {
                ts6 = new TimeSpan(0, (int)item.OT6, 0);
                ot6 = ts6.Hours.ToString("00") + ":" + ts6.Minutes.ToString("00");
            }
            TimeSpan ts7 = new TimeSpan();
            string ot7 = "";
            if (item.OT7 > 0)
            {
                ts7 = new TimeSpan(0, (int)item.OT7, 0);
                ot7 = ts7.Hours.ToString("00") + ":" + ts7.Minutes.ToString("00");
            }
            TimeSpan ts8 = new TimeSpan();
            string ot8 = "";
            if (item.OT8 > 0)
            {
                ts8 = new TimeSpan(0, (int)item.OT8, 0);
                ot8 = ts8.Hours.ToString("00") + ":" + ts8.Minutes.ToString("00");
            }
            TimeSpan ts9 = new TimeSpan();
            string ot9 = "";
            if (item.OT9 > 0)
            {
                ts9 = new TimeSpan(0, (int)item.OT9, 0);
                ot9 = ts9.Hours.ToString("00") + ":" + ts9.Minutes.ToString("00");
            } TimeSpan ts10 = new TimeSpan();
            string ot10 = "";
            if (item.OT10 > 0)
            {
                ts10 = new TimeSpan(0, (int)item.OT10, 0);
                ot10 = ts10.Hours.ToString("00") + ":" + ts10.Minutes.ToString("00");
            } TimeSpan ts11 = new TimeSpan();
            string ot11 = "";
            if (item.OT11 > 0)
            {
                ts11 = new TimeSpan(0, (int)item.OT11, 0);
                ot11 = ts11.Hours.ToString("00") + ":" + ts11.Minutes.ToString("00");
            } TimeSpan ts12 = new TimeSpan();
            string ot12 = "";
            if (item.OT12 > 0)
            {
                ts12 = new TimeSpan(0, (int)item.OT12, 0);
                ot12 = ts12.Hours.ToString("00") + ":" + ts12.Minutes.ToString("00");
            }
            TimeSpan ts13 = new TimeSpan();
            string ot13 = "";
            if (item.OT13 > 0)
            {
                ts13 = new TimeSpan(0, (int)item.OT13, 0);
                ot13 = ts13.Hours.ToString("00") + ":" + ts13.Minutes.ToString("00");
            }
            TimeSpan ts14 = new TimeSpan();
            string ot14 = "";
            if (item.OT14 > 0)
            {
                ts14 = new TimeSpan(0, (int)item.OT14, 0);
                ot14 = ts14.Hours.ToString("00") + ":" + ts14.Minutes.ToString("00");
            }
            TimeSpan ts15 = new TimeSpan();
            string ot15 = "";
            if (item.OT15 > 0)
            {
                ts15 = new TimeSpan(0, (int)item.OT15, 0);
                ot15 = ts15.Hours.ToString("00") + ":" + ts15.Minutes.ToString("00");
            }
            TimeSpan ts16 = new TimeSpan();
            string ot16 = "";
            if (item.OT16 > 0)
            {
                ts16 = new TimeSpan(0, (int)item.OT16, 0);
                ot16 = ts16.Hours.ToString("00") + ":" + ts16.Minutes.ToString("00");
            }
            TimeSpan ts17 = new TimeSpan();
            string ot17 = "";
            if (item.OT17 > 0)
            {
                ts17 = new TimeSpan(0, (int)item.OT17, 0);
                ot17 = ts17.Hours.ToString("00") + ":" + ts17.Minutes.ToString("00");
            }
            TimeSpan ts18 = new TimeSpan();
            string ot18 = "";
            if (item.OT18 > 0)
            {
                ts18 = new TimeSpan(0, (int)item.OT18, 0);
                ot18 = ts18.Hours.ToString("00") + ":" + ts18.Minutes.ToString("00");
            }
            TimeSpan ts19 = new TimeSpan();
            string ot19 = "";
            if (item.OT19 > 0)
            {
                ts19 = new TimeSpan(0, (int)item.OT19, 0);
                ot19 = ts19.Hours.ToString("00") + ":" + ts19.Minutes.ToString("00");
            }
            TimeSpan ts20 = new TimeSpan();
            string ot20 = "";
            if (item.OT20 > 0)
            {
                ts20 = new TimeSpan(0, (int)item.OT20, 0);
                ot20 = ts20.Hours.ToString("00") + ":" + ts20.Minutes.ToString("00");
            }
            TimeSpan ts21 = new TimeSpan();
            string ot21 = "";
            if (item.OT21 > 0)
            {
                ts21 = new TimeSpan(0, (int)item.OT21, 0);
                ot21 = ts21.Hours.ToString("00") + ":" + ts21.Minutes.ToString("00");
            }
            TimeSpan ts22 = new TimeSpan();
            string ot22 = "";
            if (item.OT22 > 0)
            {
                ts22 = new TimeSpan(0, (int)item.OT22, 0);
                ot22 = ts22.Hours.ToString("00") + ":" + ts22.Minutes.ToString("00");
            }
            TimeSpan ts23 = new TimeSpan();
            string ot23 = "";
            if (item.OT23 > 0)
            {
                ts23 = new TimeSpan(0, (int)item.OT23, 0);
                ot23 = ts23.Hours.ToString("00") + ":" + ts23.Minutes.ToString("00");
            }
            TimeSpan ts24 = new TimeSpan();
            string ot24 = "";
            if (item.OT24 > 0)
            {
                ts24 = new TimeSpan(0, (int)item.OT24, 0);
                ot24 = ts24.Hours.ToString("00") + ":" + ts24.Minutes.ToString("00");
            }
            TimeSpan ts25 = new TimeSpan();
            string ot25 = "";
            if (item.OT25 > 0)
            {
                ts25 = new TimeSpan(0, (int)item.OT25, 0);
                ot25 = ts25.Hours.ToString("00") + ":" + ts25.Minutes.ToString("00");
            }
            TimeSpan ts26 = new TimeSpan();
            string ot26 = "";
            if (item.OT26 > 0)
            {
                ts26 = new TimeSpan(0, (int)item.OT26, 0);
                ot26 = ts26.Hours.ToString("00") + ":" + ts26.Minutes.ToString("00");
            }
            TimeSpan ts27 = new TimeSpan();
            string ot27 = "";
            if (item.OT27 > 0)
            {
                ts27 = new TimeSpan(0, (int)item.OT27, 0);
                ot27 = ts27.Hours.ToString("00") + ":" + ts27.Minutes.ToString("00");
            }
            TimeSpan ts28 = new TimeSpan();
            string ot28 = "";
            if (item.OT28 > 0)
            {
                ts28 = new TimeSpan(0, (int)item.OT28, 0);
                ot28 = ts28.Hours.ToString("00") + ":" + ts28.Minutes.ToString("00");
            }
            TimeSpan ts29 = new TimeSpan();
            string ot29 = "";
            if (item.OT29 > 0)
            {
                ts29 = new TimeSpan(0, (int)item.OT29, 0);
                ot29 = ts29.Hours.ToString("00") + ":" + ts29.Minutes.ToString("00");
            }
            TimeSpan ts30 = new TimeSpan();
            string ot30 = "";
            if (item.OT30 > 0)
            {
                ts30 = new TimeSpan(0, (int)item.OT30, 0);
                ot30 = ts30.Hours.ToString("00") + ":" + ts30.Minutes.ToString("00");
            }
            TimeSpan ts31 = new TimeSpan();
            string ot31 = "";
            if (item.OT31 > 0)
            {
                ts31 = new TimeSpan(0, (int)item.OT31, 0);
                ot31 = ts31.Hours.ToString("00") + ":" + ts31.Minutes.ToString("00");
            }
            string Lv1 = "";
            if (item.L1 != null)
                Lv1 = item.L1;
            string Lv2 = "";
            if (item.L2 != null)
                Lv2 = item.L2;
            string Lv3 = "";
            if (item.L3 != null)
                Lv3 = item.L3;
            string Lv4 = "";
            if (item.L4 != null)
                Lv4 = item.L4;
            string Lv5 = "";
            if (item.L5 != null)
                Lv5 = item.L5;
            string Lv6 = "";
            if (item.L6 != null)
                Lv6 = item.L6;
            string Lv7 = "";
            if (item.L7 != null)
                Lv7 = item.L7;
            string Lv8 = "";
            if (item.L8 != null)
                Lv8 = item.L8;
            string Lv9 = "";
            if (item.L9 != null)
                Lv9 = item.L9;
            string Lv10 = "";
            if (item.L10 != null)
                Lv10 = item.L10;
            string Lv11 = "";
            if (item.L11 != null)
                Lv11 = item.L11;
            string Lv12 = "";
            if (item.L12 != null)
                Lv12 = item.L12;
            string Lv13 = "";
            if (item.L13 != null)
                Lv13 = item.L13;
            string Lv14 = "";
            if (item.L14 != null)
                Lv14 = item.L14;
            string Lv15 = "";
            if (item.L15 != null)
                Lv15 = item.L15;

            string Lv16 = "";
            if (item.L16 != null)
                Lv16 = item.L16;
            string Lv17 = "";
            if (item.L17 != null)
                Lv17 = item.L17;
            string Lv18 = "";
            if (item.L18 != null)
                Lv18 = item.L18;
            string Lv19 = "";
            if (item.L19 != null)
                Lv19 = item.L19;
            string Lv20 = "";
            if (item.L20 != null)
                Lv20 = item.L20;
            string Lv21 = "";
            if (item.L21 != null)
                Lv21 = item.L21;
            string Lv22 = "";
            if (item.L22 != null)
                Lv22 = item.L22;
            string Lv23 = "";
            if (item.L23 != null)
                Lv23 = item.L23;
            string Lv24 = "";
            if (item.L24 != null)
                Lv24 = item.L24;
            string Lv25 = "";
            if (item.L25 != null)
                Lv25 = item.L25;
            string Lv26 = "";
            if (item.L26 != null)
                Lv26 = item.L26;
            string Lv27 = "";
            if (item.L27 != null)
                Lv27 = item.L27;
            string Lv28 = "";
            if (item.L28 != null)
                Lv28 = item.L28;
            string Lv29 = "";
            if (item.L29 != null)
                Lv29 = item.L29;
            string Lv30 = "";
            if (item.L30 != null)
                Lv30 = item.L30;
            string Lv31 = "";
            if (item.L31 != null)
                Lv31 = item.L31;
            val = ot1 + Lv1 + "\t" + ot2 + Lv2 + "\t" + ot3 + Lv3 + "\t" + ot4 + Lv4 + "\t" + ot5 + Lv5 + "\t" + ot6 + Lv6 + "\t" + ot7 + Lv7 + "\t" + ot8 + Lv8 + "\t" + ot9 + Lv9 + "\t" +
                ot10 + Lv10 + "\t" + ot11 + Lv11 + "\t" + ot12 + Lv12 + "\t" + ot13 + Lv13 + "\t" + ot14 + Lv14 + "\t" + ot15 + Lv15 + "\t" + ot16 + Lv16 + "\t" + ot17 + Lv17 + "\t" +
                ot18 + Lv18 + "\t" + ot19 + Lv19 + "\t" + ot20 + Lv20 + "\t" + ot21 + Lv21 + "\t" + ot22 + Lv22 + "\t" + ot23 + Lv23 + "\t" + ot24 + Lv24 + "\t" + ot25 + Lv25 + "\t" +
                ot26 + Lv26 + "\t" + ot27 + Lv27 + "\t" + ot28 + Lv28 + "\t" + ot29 + Lv29 + "\t" + ot30 + Lv30 + "\t" + ot31 + Lv31 + "\t";

            return val;
        }

        private string getTotalHours(ViewMonthlyData item)
        {
            TimeSpan tsNOT = new TimeSpan();
            TimeSpan tsGOT = new TimeSpan();
            TimeSpan tsEI = new TimeSpan();
            TimeSpan tsEO = new TimeSpan();
            TimeSpan tsLI = new TimeSpan();

            string not = "";
            string got = "";
            string ei = "";
            string eo = "";
            string li = "";
            if (item.TNOT > 0)
            {
                tsNOT = new TimeSpan(0, (int)item.TNOT, 0);
                int hours = (int)tsNOT.TotalHours;
                int min = (int)(item.TNOT - (hours * 60));
                not = hours.ToString() + ":" + min.ToString();
            }
            if (item.TGZOT > 0)
            {
                tsGOT = new TimeSpan(0, (int)item.TGZOT, 0);
                int hours = (int)tsGOT.TotalHours;
                int min = (int)(item.TGZOT - (hours * 60));
                got = hours.ToString() + ":" + min.ToString();
            }
            if (item.TEarlyIn > 0)
            {
                tsEI = new TimeSpan(0, (int)item.TEarlyIn, 0);
                int hours = (int)tsEI.TotalHours;
                int min = (int)(item.TEarlyIn - (hours * 60));
                ei = hours.ToString() + ":" + min.ToString();
            }
            if (item.TEarlyOut > 0)
            {
                tsEO = new TimeSpan(0, (int)item.TEarlyOut, 0);
                int hours = (int)tsEO.TotalHours;
                int min = (int)(item.TEarlyOut - (hours * 60));
                eo = hours.ToString() + ":" + min.ToString();
            }
            if (item.TLateIn > 0)
            {
                tsLI = new TimeSpan(0, (int)item.TLateIn, 0);
                int hours = (int)tsLI.TotalHours;
                int min = (int)(item.TLateIn - (hours * 60));
                li = hours.ToString() + ":" + min.ToString();
            }

            string val = "";
            val = not + "\t"
            + got + "\t"
            + ei + "\t"
            + eo + "\t"
            + li;
            return val;
        }

        private string GetDaysTotal(ViewMonthlyData item)
        {
            string val = "";
            val = item.PreDays + "\t" + item.AbDays + "\t" + item.LeaveDays + "\t" + item.HalfLeavesDay + "\t" + item.RestDays + "\t" + item.GZDays + "\t" + item.WorkDays + "\t" + item.TotalDays;
            return val;
        }
        #endregion

        #region -- Job Card Summary --

        public List<ModelJCSummaryData> GetJCSummaryData(List<AttDataManEdit> attEdit, List<JobCardEmp> jcEmps, List<EmpView> emps, DateTime dtFrom, DateTime dtTo)
        {
            List<ModelJCSummaryData> vmList = new List<ModelJCSummaryData>();

            foreach (var emp in emps)
            {
                List<JobCardEmp> tempJCEmps = new List<JobCardEmp>();
                tempJCEmps = jcEmps.Where(aa => aa.EmpID == emp.EmpID).ToList();
                //if (tempJCEmps.Count>0)
                //{
                ModelJCSummaryData vm = new ModelJCSummaryData();
                vm.EmpID = emp.EmpID;
                vm.EmpNo = emp.EmpNo;
                vm.EmpName = emp.EmpName;
                vm.DesignationName = emp.DesignationName;
                vm.GradeName = emp.GradeName;
                vm.CompName = emp.CompName;
                vm.CityName = emp.CityName;
                vm.RegionName = emp.RegionName;
                vm.CrewName = emp.CrewName;
                vm.LocName = emp.LocName;
                vm.SectionName = emp.SectionName;
                vm.TypeName = emp.TypeName;
                vm.DeptName = emp.DeptName;
                vm.CatName = emp.CatName;
                vm.ShiftName = emp.ShiftName;
                if (emp.JoinDate != null)
                {
                    vm.DateofBirth = emp.JoinDate;
                }
                vm.DivisionName = emp.DivisionName;
                vm.CompanyID = (short)emp.CompanyID;
                vm.TypeID = (byte)emp.TypeID;
                vm.CatID = (short)emp.CatID;
                vm.LocID = (short)emp.LocID;
                vm.CrewID = (short)emp.CrewID;
                vm.DeptID = (short)emp.DeptID;
                vm.ShiftID = (byte)emp.ShiftID;
                vm.SecID = (short)emp.SecID;
                vm.DivID = (short)emp.DivID;
                vm.DesigID = (short)emp.DesigID;
                vm.StartDate = dtFrom;
                vm.EndDate = dtTo;
                vm.EditAttCount = attEdit.Where(aa => aa.EmpID == emp.EmpID).GroupBy(aa => aa.EmpDate).Count();
                //1	Day Off//2	GZ Holiday//3	Absent//4	Official Duty//8	Double Duty//9	Baddli//10	Present//11	Swap Rest Day
                int DayOff = 0;
                int Holiday = 0;
                int Absent = 0;
                int OfficialDuty = 0;
                int DoubleDuty = 0;
                int Baddli = 0;
                int Present = 0;
                int SwapRestDay = 0;
                foreach (var jc in tempJCEmps)
                {
                    switch (jc.WrkCardID)
                    {
                        case 1:
                            DayOff = DayOff + 1;
                            break;
                        case 2:
                            Holiday = Holiday + 1;
                            break;
                        case 3:
                            Absent = Absent + 1;
                            break;
                        case 4:
                            OfficialDuty = OfficialDuty + 1;
                            break;
                        case 8:
                            DoubleDuty = DoubleDuty + 1;
                            break;
                        case 9:
                            Baddli = Baddli + 1;
                            break;
                        case 10:
                            Present = Present + 1;
                            break;
                        case 11:
                            SwapRestDay = SwapRestDay + 1;
                            break;
                    }
                }
                vm.DayOff = DayOff;
                vm.Holiday = Holiday;
                vm.Absent = Absent;
                vm.OfficialDuty = OfficialDuty;
                vm.DoubleDuty = DoubleDuty;
                vm.Baddli = Baddli;
                vm.Present = Present;
                vm.SwapRestDay = SwapRestDay;
                vm.Total = DayOff + Holiday + Absent + OfficialDuty + DoubleDuty + Baddli + Present + SwapRestDay;
                if (vm.Total > 0 || vm.EditAttCount > 0)
                {
                    vmList.Add(vm);
                }
                //}
            }
            return vmList;
        }


        #endregion

        #region Still not good Placement of Helping Funtions
        private List<ModelHSEDataEmp> GetHSEData(List<AttData> _ViewAttList, List<EmpView> emps)
        {
            List<ModelHSEDataEmp> hselist = new List<ModelHSEDataEmp>();
            foreach (var emp in emps)
            {
                ModelHSEDataEmp hseHour = new ModelHSEDataEmp();
                hseHour.EmpNo = emp.EmpNo;
                hseHour.EmpName = emp.EmpName;
                hseHour.DesignationName = emp.DesignationName;
                hseHour.SecName = emp.SectionName;
                hseHour.DeptName = emp.DeptName;
                hseHour.LocName = emp.LocName;
                hseHour.CompName = emp.CompName;
                int total = 0;
                int hours = 0;
                foreach (var att in _ViewAttList.Where(aa => aa.EmpID == emp.EmpID).ToList())
                {
                    if (att.WorkMin > 0)
                        hours = (int)(att.WorkMin + hours);
                    total = hours / 60;
                }
                hseHour.Hours = hours;
                hseHour.Total = total;
                hselist.Add(hseHour);
            }
            return hselist;
        }
        private void DownloadAppOTReport(List<ViewEmpAttMnAppOT> list, string p, int UserID)
        {
            string val = "";
            list = list.OrderBy(aa => aa.DeptID).ThenBy(aa => aa.SecID).ThenBy(aa => aa.EmpID).ToList();
            val = DownloadAppOTData(list, p, UserID);
            if (val != "")
            {
                Response.ContentType = ContentType;
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + val);
                Response.WriteFile(val);
                Response.End();
            }
        }

        private string DownloadAppOTData(List<ViewEmpAttMnAppOT> MonthlyDatas, string Date, int UserID)
        {
            string retVal = "";
            //lvPDF is nothing but the listview control name
            string[] st = new string[5];
            DirectoryInfo di = new DirectoryInfo(@"D:\Reports\");
            if (di.Exists == false)
                di.Create();
            StreamWriter sw = new StreamWriter(@"D:\Reports\MonthlyReportB" + UserID + MonthlyDatas.FirstOrDefault().StartDate.Value.Month.ToString("00") + ".xls", false);
            sw.AutoFlush = true;
            List<string> ColumnsName = new List<string>();
            ColumnsName.Add("EmpNo");
            ColumnsName.Add("Name");
            ColumnsName.Add("Designation");
            ColumnsName.Add("Section");
            ColumnsName.Add("Department");
            ColumnsName.Add("Type");
            ColumnsName.Add("1");
            ColumnsName.Add("2");
            ColumnsName.Add("3");
            ColumnsName.Add("4");
            ColumnsName.Add("5");
            ColumnsName.Add("6");
            ColumnsName.Add("7");
            ColumnsName.Add("8");
            ColumnsName.Add("9");
            ColumnsName.Add("10");
            ColumnsName.Add("11");
            ColumnsName.Add("12");
            ColumnsName.Add("13");
            ColumnsName.Add("14");
            ColumnsName.Add("15");
            ColumnsName.Add("16");
            ColumnsName.Add("17");
            ColumnsName.Add("18");
            ColumnsName.Add("19");
            ColumnsName.Add("20");
            ColumnsName.Add("21");
            ColumnsName.Add("22");
            ColumnsName.Add("23");
            ColumnsName.Add("24");
            ColumnsName.Add("25");
            ColumnsName.Add("26");
            ColumnsName.Add("27");
            ColumnsName.Add("28");
            ColumnsName.Add("29");
            ColumnsName.Add("30");
            ColumnsName.Add("31");
            ColumnsName.Add("PR");
            ColumnsName.Add("AB");
            ColumnsName.Add("LV");
            ColumnsName.Add("HLV");
            ColumnsName.Add("DO");
            ColumnsName.Add("GZ");
            ColumnsName.Add("PD");
            ColumnsName.Add("BadliDays");
            ColumnsName.Add("Total");
            ColumnsName.Add("N-OT");
            ColumnsName.Add("G-OT");
            ColumnsName.Add("E-In");
            ColumnsName.Add("E-Out");
            ColumnsName.Add("L-In");
            ColumnsName.Add("A-NOT");
            ColumnsName.Add("A-GOT");
            ColumnsName.Add("App-NOT");
            ColumnsName.Add("App-GOT");
            string ColumnString = "";
            for (int i = 0; i < ColumnsName.Count; i++)
            {
                ColumnString = ColumnString + ColumnsName[i] + "\t";
            }
            sw.Write(Date + "\n");
            sw.Write(ColumnString + "\n");
            foreach (var item in MonthlyDatas)
            {
                string Personal = item.EmpNo + "\t" + item.EmpName + "\t" + item.DesignationName + "\t" + item.SectionName + "\t" + item.DeptName + "\t" + item.TypeName;
                string DaysInfo = item.OT1 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2
                    + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2
                    + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2 + "\t" + item.OT2;

                string TotalsDays = GetDaysTotal(item);
                string OTLine = GetOTLine(item);
                sw.WriteLine(Personal + "\t" + DaysInfo + "\t" + TotalsDays);
                sw.WriteLine("\t\t\t\t\t\t" + OTLine);
            }
            sw.Close();
            FileInfo fil = new FileInfo(@"D:\Reports\MonthlyReportB" + UserID + MonthlyDatas.FirstOrDefault().StartDate.Value.Month.ToString("00") + ".xls");
            if (fil.Exists == true)
            {
                retVal = "D:\\Reports\\MonthlyReportB" + UserID + MonthlyDatas.FirstOrDefault().StartDate.Value.Month.ToString("00") + ".xls";
            }
            return retVal;
        }

        private string GetOTLine(ViewEmpAttMnAppOT item)
        {
            string val = "";

            TimeSpan ts1 = new TimeSpan();
            string ot1 = "";
            if (item.OT1 > 0)
            {
                ts1 = new TimeSpan(0, (int)item.OT1, 0);
                ot1 = ts1.Hours.ToString("00") + ":" + ts1.Minutes.ToString("00");
            }
            TimeSpan ts2 = new TimeSpan();
            string ot2 = "";
            if (item.OT2 > 0)
            {
                ts2 = new TimeSpan(0, (int)item.OT2, 0);
                ot2 = ts2.Hours.ToString("00") + ":" + ts2.Minutes.ToString("00");
            }

            TimeSpan ts3 = new TimeSpan();
            string ot3 = "";
            if (item.OT3 > 0)
            {
                ts3 = new TimeSpan(0, (int)item.OT3, 0);
                ot3 = ts3.Hours.ToString("00") + ":" + ts3.Minutes.ToString("00");
            }
            TimeSpan ts4 = new TimeSpan();
            string ot4 = "";
            if (item.OT4 > 0)
            {
                ts4 = new TimeSpan(0, (int)item.OT4, 0);
                ot4 = ts4.Hours.ToString("00") + ":" + ts4.Minutes.ToString("00");
            }
            TimeSpan ts5 = new TimeSpan();
            string ot5 = "";
            if (item.OT5 > 0)
            {
                ts5 = new TimeSpan(0, (int)item.OT5, 0);
                ot5 = ts5.Hours.ToString("00") + ":" + ts5.Minutes.ToString("00");
            }
            TimeSpan ts6 = new TimeSpan();
            string ot6 = "";
            if (item.OT6 > 0)
            {
                ts6 = new TimeSpan(0, (int)item.OT6, 0);
                ot6 = ts6.Hours.ToString("00") + ":" + ts6.Minutes.ToString("00");
            }
            TimeSpan ts7 = new TimeSpan();
            string ot7 = "";
            if (item.OT7 > 0)
            {
                ts7 = new TimeSpan(0, (int)item.OT7, 0);
                ot7 = ts7.Hours.ToString("00") + ":" + ts7.Minutes.ToString("00");
            }
            TimeSpan ts8 = new TimeSpan();
            string ot8 = "";
            if (item.OT8 > 0)
            {
                ts8 = new TimeSpan(0, (int)item.OT8, 0);
                ot8 = ts8.Hours.ToString("00") + ":" + ts8.Minutes.ToString("00");
            }
            TimeSpan ts9 = new TimeSpan();
            string ot9 = "";
            if (item.OT9 > 0)
            {
                ts9 = new TimeSpan(0, (int)item.OT9, 0);
                ot9 = ts9.Hours.ToString("00") + ":" + ts9.Minutes.ToString("00");
            } TimeSpan ts10 = new TimeSpan();
            string ot10 = "";
            if (item.OT10 > 0)
            {
                ts10 = new TimeSpan(0, (int)item.OT10, 0);
                ot10 = ts10.Hours.ToString("00") + ":" + ts10.Minutes.ToString("00");
            } TimeSpan ts11 = new TimeSpan();
            string ot11 = "";
            if (item.OT11 > 0)
            {
                ts11 = new TimeSpan(0, (int)item.OT11, 0);
                ot11 = ts11.Hours.ToString("00") + ":" + ts11.Minutes.ToString("00");
            } TimeSpan ts12 = new TimeSpan();
            string ot12 = "";
            if (item.OT12 > 0)
            {
                ts12 = new TimeSpan(0, (int)item.OT12, 0);
                ot12 = ts12.Hours.ToString("00") + ":" + ts12.Minutes.ToString("00");
            }
            TimeSpan ts13 = new TimeSpan();
            string ot13 = "";
            if (item.OT13 > 0)
            {
                ts13 = new TimeSpan(0, (int)item.OT13, 0);
                ot13 = ts13.Hours.ToString("00") + ":" + ts13.Minutes.ToString("00");
            }
            TimeSpan ts14 = new TimeSpan();
            string ot14 = "";
            if (item.OT14 > 0)
            {
                ts14 = new TimeSpan(0, (int)item.OT14, 0);
                ot14 = ts14.Hours.ToString("00") + ":" + ts14.Minutes.ToString("00");
            }
            TimeSpan ts15 = new TimeSpan();
            string ot15 = "";
            if (item.OT15 > 0)
            {
                ts15 = new TimeSpan(0, (int)item.OT15, 0);
                ot15 = ts15.Hours.ToString("00") + ":" + ts15.Minutes.ToString("00");
            }
            TimeSpan ts16 = new TimeSpan();
            string ot16 = "";
            if (item.OT16 > 0)
            {
                ts16 = new TimeSpan(0, (int)item.OT16, 0);
                ot16 = ts16.Hours.ToString("00") + ":" + ts16.Minutes.ToString("00");
            }
            TimeSpan ts17 = new TimeSpan();
            string ot17 = "";
            if (item.OT17 > 0)
            {
                ts17 = new TimeSpan(0, (int)item.OT17, 0);
                ot17 = ts17.Hours.ToString("00") + ":" + ts17.Minutes.ToString("00");
            }
            TimeSpan ts18 = new TimeSpan();
            string ot18 = "";
            if (item.OT18 > 0)
            {
                ts18 = new TimeSpan(0, (int)item.OT18, 0);
                ot18 = ts18.Hours.ToString("00") + ":" + ts18.Minutes.ToString("00");
            }
            TimeSpan ts19 = new TimeSpan();
            string ot19 = "";
            if (item.OT19 > 0)
            {
                ts19 = new TimeSpan(0, (int)item.OT19, 0);
                ot19 = ts19.Hours.ToString("00") + ":" + ts19.Minutes.ToString("00");
            }
            TimeSpan ts20 = new TimeSpan();
            string ot20 = "";
            if (item.OT20 > 0)
            {
                ts20 = new TimeSpan(0, (int)item.OT20, 0);
                ot20 = ts20.Hours.ToString("00") + ":" + ts20.Minutes.ToString("00");
            }
            TimeSpan ts21 = new TimeSpan();
            string ot21 = "";
            if (item.OT21 > 0)
            {
                ts21 = new TimeSpan(0, (int)item.OT21, 0);
                ot21 = ts21.Hours.ToString("00") + ":" + ts21.Minutes.ToString("00");
            }
            TimeSpan ts22 = new TimeSpan();
            string ot22 = "";
            if (item.OT22 > 0)
            {
                ts22 = new TimeSpan(0, (int)item.OT22, 0);
                ot22 = ts22.Hours.ToString("00") + ":" + ts22.Minutes.ToString("00");
            }
            TimeSpan ts23 = new TimeSpan();
            string ot23 = "";
            if (item.OT23 > 0)
            {
                ts23 = new TimeSpan(0, (int)item.OT23, 0);
                ot23 = ts23.Hours.ToString("00") + ":" + ts23.Minutes.ToString("00");
            }
            TimeSpan ts24 = new TimeSpan();
            string ot24 = "";
            if (item.OT24 > 0)
            {
                ts24 = new TimeSpan(0, (int)item.OT24, 0);
                ot24 = ts24.Hours.ToString("00") + ":" + ts24.Minutes.ToString("00");
            }
            TimeSpan ts25 = new TimeSpan();
            string ot25 = "";
            if (item.OT25 > 0)
            {
                ts25 = new TimeSpan(0, (int)item.OT25, 0);
                ot25 = ts25.Hours.ToString("00") + ":" + ts25.Minutes.ToString("00");
            }
            TimeSpan ts26 = new TimeSpan();
            string ot26 = "";
            if (item.OT26 > 0)
            {
                ts26 = new TimeSpan(0, (int)item.OT26, 0);
                ot26 = ts26.Hours.ToString("00") + ":" + ts26.Minutes.ToString("00");
            }
            TimeSpan ts27 = new TimeSpan();
            string ot27 = "";
            if (item.OT27 > 0)
            {
                ts27 = new TimeSpan(0, (int)item.OT27, 0);
                ot27 = ts27.Hours.ToString("00") + ":" + ts27.Minutes.ToString("00");
            }
            TimeSpan ts28 = new TimeSpan();
            string ot28 = "";
            if (item.OT28 > 0)
            {
                ts28 = new TimeSpan(0, (int)item.OT28, 0);
                ot28 = ts28.Hours.ToString("00") + ":" + ts28.Minutes.ToString("00");
            }
            TimeSpan ts29 = new TimeSpan();
            string ot29 = "";
            if (item.OT29 > 0)
            {
                ts29 = new TimeSpan(0, (int)item.OT29, 0);
                ot29 = ts29.Hours.ToString("00") + ":" + ts29.Minutes.ToString("00");
            }
            TimeSpan ts30 = new TimeSpan();
            string ot30 = "";
            if (item.OT30 > 0)
            {
                ts30 = new TimeSpan(0, (int)item.OT30, 0);
                ot30 = ts30.Hours.ToString("00") + ":" + ts30.Minutes.ToString("00");
            }
            TimeSpan ts31 = new TimeSpan();
            string ot31 = "";
            if (item.OT31 > 0)
            {
                ts31 = new TimeSpan(0, (int)item.OT31, 0);
                ot31 = ts31.Hours.ToString("00") + ":" + ts31.Minutes.ToString("00");
            }
            string Lv1 = "";
            if (item.AOT1 != null)
                Lv1 = Convert.ToString(item.AOT1);
            //string Lv2 = "";
            //if (item.L2 != null)
            //    Lv2 = item.L2;
            //string Lv3 = "";
            //if (item.L3 != null)
            //    Lv3 = item.L3;
            //string Lv4 = "";
            //if (item.L4 != null)
            //    Lv4 = item.L4;
            //string Lv5 = "";
            //if (item.L5 != null)
            //    Lv5 = item.L5;
            //string Lv6 = "";
            //if (item.L6 != null)
            //    Lv6 = item.L6;
            //string Lv7 = "";
            //if (item.L7 != null)
            //    Lv7 = item.L7;
            //string Lv8 = "";
            //if (item.L8 != null)
            //    Lv8 = item.L8;
            //string Lv9 = "";
            //if (item.L9 != null)
            //    Lv9 = item.L9;
            //string Lv10 = "";
            //if (item.L10 != null)
            //    Lv10 = item.L10;
            //string Lv11 = "";
            //if (item.L11 != null)
            //    Lv11 = item.L11;
            //string Lv12 = "";
            //if (item.L12 != null)
            //    Lv12 = item.L12;
            //string Lv13 = "";
            //if (item.L13 != null)
            //    Lv13 = item.L13;
            //string Lv14 = "";
            //if (item.L14 != null)
            //    Lv14 = item.L14;
            //string Lv15 = "";
            //if (item.L15 != null)
            //    Lv15 = item.L15;

            //string Lv16 = "";
            //if (item.L16 != null)
            //    Lv16 = item.L16;
            //string Lv17 = "";
            //if (item.L17 != null)
            //    Lv17 = item.L17;
            //string Lv18 = "";
            //if (item.L18 != null)
            //    Lv18 = item.L18;
            //string Lv19 = "";
            //if (item.L19 != null)
            //    Lv19 = item.L19;
            //string Lv20 = "";
            //if (item.L20 != null)
            //    Lv20 = item.L20;
            //string Lv21 = "";
            //if (item.L21 != null)
            //    Lv21 = item.L21;
            //string Lv22 = "";
            //if (item.L22 != null)
            //    Lv22 = item.L22;
            //string Lv23 = "";
            //if (item.L23 != null)
            //    Lv23 = item.L23;
            //string Lv24 = "";
            //if (item.L24 != null)
            //    Lv24 = item.L24;
            //string Lv25 = "";
            //if (item.L25 != null)
            //    Lv25 = item.L25;
            //string Lv26 = "";
            //if (item.L26 != null)
            //    Lv26 = item.L26;
            //string Lv27 = "";
            //if (item.L27 != null)
            //    Lv27 = item.L27;
            //string Lv28 = "";
            //if (item.L28 != null)
            //    Lv28 = item.L28;
            //string Lv29 = "";
            //if (item.L29 != null)
            //    Lv29 = item.L29;
            //string Lv30 = "";
            //if (item.L30 != null)
            //    Lv30 = item.L30;
            //string Lv31 = "";
            //if (item.L31 != null)
            //    Lv31 = item.L31;
            val = Lv1 + "\t";

            return val;
        }
        private List<VMBadliRecord> GetBadliValue(List<BadliRecordEmp> _BadliList)
        {
            TAS2013Entities db = new TAS2013Entities();
            List<VMBadliRecord> brs = new List<VMBadliRecord>();
            List<Emp> emps = new List<Emp>();
            emps = db.Emps.ToList();
            foreach (var item in _BadliList)
            {
                Emp OEmp = emps.First(aa => aa.EmpID == item.EmpID);
                Emp BEmp = emps.First(aa => aa.EmpID == item.BadliEmpID);
                VMBadliRecord br = new VMBadliRecord();
                br.BadliID = item.BadliID;
                br.BDesignation = BEmp.Designation.DesignationName;
                br.BEmpID = (int)item.BadliEmpID;
                br.BEmpName = BEmp.EmpName;
                br.BEmpNo = BEmp.EmpNo;
                br.Dated = (DateTime)item.Date;
                br.Designation = OEmp.Designation.DesignationName;
                br.EmpID = (int)item.EmpID;
                br.EmpName = OEmp.EmpName;
                br.EmpNo = OEmp.EmpNo;
                brs.Add(br);
            }
            return brs;
        }
        private string GetDaysTotal(ViewEmpAttMnAppOT item)
        {
            string val = "";
            val = item.AcTotalOT + "\t" + item.AppTotalOT;
            return val;

        }
        private List<ModelEditAttAudit> GetTotalCount(List<EmpView> emps, List<ViewEditAttendance> _editList)
        {
            List<ModelEditAttAudit> mde = new List<ModelEditAttAudit>();
            foreach (var item in emps)
            {
                ModelEditAttAudit ma = new ModelEditAttAudit();
                ma.EmpID = item.EmpID;
                ma.EmpNo = item.EmpNo;
                ma.EmpName = item.EmpName;
                ma.DesignationName = item.DesignationName;
                ma.GradeName = item.GradeName;
                ma.CompName = item.CompName;
                ma.CityName = item.CityName;
                ma.RegionName = item.RegionName;
                ma.CrewName = item.CrewName;
                ma.LocName = item.LocName;
                ma.SectionName = item.SectionName;
                ma.TypeName = item.TypeName;
                ma.DeptName = item.DeptName;
                ma.CatName = item.CatName;
                ma.ShiftName = item.ShiftName;
                ma.DivisionName = item.DivisionName;
                ma.CompanyID = (short)item.CompanyID;
                ma.TypeID = (byte)item.TypeID;
                ma.CatID = (byte)item.CatID;
                ma.LocID = (Int16)item.LocID;
                ma.CrewID = (Int16)item.CrewID;
                ma.DeptID = (Int16)item.DeptID;
                ma.ShiftID = (byte)item.ShiftID;
                ma.SecID = (Int16)item.SecID;
                ma.DesigID = (byte)item.DesigID;
                //ma.Remarks =_editList.Where(aa => aa.EmpID == item.EmpID).Select(aa => aa.Remarks).ToString();
                ma.TotalRecords = _editList.Where(aa => aa.EmpID == item.EmpID).Select(aa => aa.EmpDate).Distinct().Count();
                ma.ChangeDutyCode = _editList.Where(aa => aa.EmpID == item.EmpID && aa.NewDutyCode != aa.OldDutyCode && aa.NewTimeIn == aa.OldTimeIn && aa.NewTimeOut == aa.OldTimeOut).Select(aa => aa.EmpDate).Distinct().Count();
                ma.ChangeInOut = ma.TotalRecords - ma.ChangeDutyCode;
                ma.ChangeShiftTime = _editList.Where(aa => aa.EmpID == item.EmpID && aa.NewShiftMin != aa.OldShiftMin && aa.NewTimeIn == aa.OldTimeIn && aa.NewTimeOut == aa.OldTimeOut).Select(aa => aa.EmpDate).Distinct().Count();
                mde.Add(ma);
            }
            return mde;
        }
        private List<ViewHSEHour> GetHSEData(List<ViewHSEHour> hseDBDatas, DateTime dts, DateTime dte, List<Section> secs)
        {
            int days = (dte - dts).Days + 1;
            List<int> PMSecs = new List<int> { 57 };
            List<int> QHSETSecs = new List<int> { 54, 2, 45 };
            List<int> AdminSecs = new List<int> { 1, 3, 5, 6, 7, 8, 9, 10, 48, 51, 64, 66, 97, 99, 468, 489, 528, 667 };
            List<int> MaintinanceSecs = new List<int> { 83, 19, 20, 21, 22, 30, 35, 615, 23, 24, 25, 26, 27, 28, 29, 34, 31, 32, 33 };
            List<int> ProdNSecs = new List<int> { 53, 55, 12, 14, 16, 18 };
            List<int> ProdSSecs = new List<int> { 52, 13, 15, 17 };
            List<int> EISecs = new List<int> { 58, 474, 37 };
            List<int> TPSecs = new List<int> { 59, 60, 40, 41, 42, 49, 44, 43 };
            List<int> OtherSecs = new List<int> { 46, 11, 633, 113, 483, 50, 39, 47 };
            List<ViewHSEHour> hseHours = new List<ViewHSEHour>();
            foreach (var sec in secs)
            {
                if (GetSecIDs().Contains(sec.SectionID))
                {
                    ViewHSEHour hseHour = new ViewHSEHour();
                    if (PMSecs.Contains(sec.SectionID))
                    {
                        hseHour.DeptID = 1;
                        hseHour.DeptName = "Management (DO)";
                    }
                    if (ProdNSecs.Contains(sec.SectionID))
                    {
                        hseHour.DeptID = 2;
                        hseHour.DeptName = "Prod-North";
                    }
                    if (ProdSSecs.Contains(sec.SectionID))
                    {
                        hseHour.DeptID = 3;
                        hseHour.DeptName = "Prod-South";
                    }
                    if (MaintinanceSecs.Contains(sec.SectionID))
                    {
                        hseHour.DeptID = 4;
                        hseHour.DeptName = "Maintenance";
                    }
                    if (EISecs.Contains(sec.SectionID))
                    {
                        hseHour.DeptID = 5;
                        hseHour.DeptName = "E&I";
                    }
                    if (TPSecs.Contains(sec.SectionID))
                    {
                        hseHour.DeptID = 6;
                        hseHour.DeptName = "Technical Services Department & DBN";
                    }
                    if (AdminSecs.Contains(sec.SectionID))
                    {
                        hseHour.DeptID = 7;
                        hseHour.DeptName = "Administration and General Services";
                    }
                    if (QHSETSecs.Contains(sec.SectionID))
                    {
                        hseHour.DeptID = 8;
                        hseHour.DeptName = "QHSET";
                    }
                    if (OtherSecs.Contains(sec.SectionID))
                    {
                        hseHour.DeptID = 9;
                        hseHour.DeptName = "Misc(IT, Procurment, HR, Finance, In Audit)";
                    }
                    hseHour.SecID = sec.SectionID;
                    hseHour.SecName = sec.SectionName;
                    hseHour.ContractEmps = 0;
                    hseHour.ContractEmpHour = 0;
                    hseHour.StaffEmpHours = 0;
                    hseHour.StaffEmps = 0;
                    hseHour.MngEmpHours = 0;
                    hseHour.MngEmps = 0;
                    DateTime dt = dts;
                    int total = 0;
                    int MngEmps = 0;
                    int StaffEmps = 0;
                    int ContractEmps = 0;
                    while (dt <= dte)
                    {
                        if (hseDBDatas.Where(aa => aa.HSEDate == dt && aa.SecID == sec.SectionID).Count() > 0)
                        {
                            ViewHSEHour hse = hseDBDatas.First(aa => aa.HSEDate == dt && aa.SecID == sec.SectionID);
                            hseHour.MngEmpHours = (int)(hseHour.MngEmpHours + hse.MngEmpHours);
                            hseHour.StaffEmpHours = (int)(hseHour.StaffEmpHours + hse.StaffEmpHours);
                            hseHour.ContractEmpHour = (int)(hseHour.ContractEmpHour + hse.ContractEmpHour);
                            MngEmps = (int)((MngEmps + hse.MngEmps));
                            StaffEmps = (int)((StaffEmps + hse.StaffEmps));
                            ContractEmps = (int)((ContractEmps + hse.ContractEmps));
                        }
                        dt = dt.AddDays(1);
                    }
                    days = hseDBDatas.Where(aa => aa.SecID == sec.SectionID).Count();
                    if (days == 0)
                        days = 1;
                    hseHour.MngEmps = (int)(MngEmps / days);
                    hseHour.StaffEmps = (int)(StaffEmps / days);
                    hseHour.ContractEmps = (int)(ContractEmps / days);
                    if (hseHour.StaffEmpHours > 0 || hseHour.ContractEmpHour > 0 || hseHour.MngEmpHours > 0)
                        hseHours.Add(hseHour);
                }
            }
            return hseHours;
        }
        List<int> secIds = new List<int>();
        DataTable FlexyMonthlyReportDT = new DataTable();

        public void CreateFlexyMonthlyDataTable()
        {
            FlexyMonthlyReportDT.Columns.Add("Period", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("EmpMonth", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("StartDate", typeof(DateTime));
            FlexyMonthlyReportDT.Columns.Add("EndDate", typeof(DateTime));
            FlexyMonthlyReportDT.Columns.Add("EmpNo", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("EmpID", typeof(int));
            FlexyMonthlyReportDT.Columns.Add("EmpName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("SectionName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("DeptName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("TypeName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("LocName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("ShiftName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D21", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D22", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D23", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D24", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D25", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D26", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D27", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D28", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D29", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D30", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D31", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D1", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D2", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D3", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D4", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D5", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D6", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D7", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D8", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D9", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D10", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D11", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D12", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D13", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D14", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D15", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D16", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D17", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D18", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D19", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D20", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("TotalDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("WorkDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("PreDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("AbDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("RestDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("GZDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("LeaveDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OfficialDutyDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TEarlyIn", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TEarlyOut", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TLateIn", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TLateOut", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TWorkTime", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TNOT", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TGZOT", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("ExpectedWrkTime", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT1", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT2", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT3", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT4", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT5", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT6", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT7", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT8", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT9", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT10", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT11", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT12", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT13", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT14", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT15", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT16", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT17", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT18", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT19", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT20", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT21", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT22", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT23", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT24", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT25", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT26", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT27", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT28", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT29", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT30", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT31", typeof(Int16));

        }

        public void AddDataToMonthlyDataAtable(string Period, string EmpMonth, DateTime StartDate, DateTime EndDate, string EmpNo, int EmpID, string EmpName, string SectionName, string DeptName, string TypeName, string LocName, string ShiftName, string D21, string D22, string D23, string D24, string D25, string D26, string D27, string D28, string D29, string D30, string D31, string D1, string D2, string D3, string D4, string D5, string D6, string D7, string D8, string D9, string D10, string D11, string D12, string D13, string D14, string D15, string D16, string D17, string D18, string D19, string D20, Int16 TotalDays, Int16 WorkDays, Int16 PreDays, Int16 AbDays, Int16 RestDays, Int16 GZDays, Int16 LeaveDays, Int16 OfficialDutyDays, Int16 TEarlyIn, Int16 TEarlyOut, Int16 TLateIn, Int16 TLateOut, Int16 TWorkTime, Int16 TNOT, Int16 TGZOT, Int16 ExpectedWrkTime, Int16 OT1, Int16 OT2, Int16 OT3, Int16 OT4, Int16 OT5, Int16 OT6, Int16 OT7, Int16 OT8, Int16 OT9, Int16 OT10, Int16 OT11, Int16 OT12, Int16 OT13, Int16 OT14, Int16 OT15, Int16 OT16, Int16 OT17, Int16 OT18, Int16 OT19, Int16 OT20, Int16 OT21, Int16 OT22, Int16 OT23, Int16 OT24, Int16 OT25, Int16 OT26, Int16 OT27, Int16 OT28, Int16 OT29, Int16 OT30, Int16 OT31)
        {
            FlexyMonthlyReportDT.Rows.Add(Period, EmpMonth, StartDate, EndDate, EmpNo, EmpID, EmpName, SectionName, DeptName, TypeName, LocName, ShiftName, D21, D22, D23, D24, D25, D26, D27, D28, D29, D30, D31, D1, D2, D3, D4, D5, D6, D7, D8, D9, D10, D11, D12, D13, D14, D15, D16, D17, D18, D19, D20, TotalDays, WorkDays, PreDays, AbDays, RestDays, GZDays, LeaveDays, OfficialDutyDays, TEarlyIn, TEarlyOut, TLateIn, TLateOut, TWorkTime, TNOT, TGZOT, ExpectedWrkTime, OT1, OT2, OT3, OT4, OT5, OT6, OT7, OT8, OT9, OT10, OT11, OT12, OT13, OT14, OT15, OT16, OT17, OT18, OT19, OT20, OT21, OT22, OT23, OT24, OT25, OT26, OT27, OT28, OT29, OT30, OT31);

        }

        #endregion

        #endregion

        #region --Reports Filter Implementations--

        #region -- Filters Implementation OLD--
        private List<HSEHour> ReportsFilterImplementation(FiltersModel fm, List<HSEHour> _TempViewList, List<HSEHour> _ViewList)
        {

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            return _ViewList;
        }

        public List<ViewAttDataAppOT> ReportsFilterImplementation(FiltersModel fm, List<ViewAttDataAppOT> _TempViewList, List<ViewAttDataAppOT> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        private List<ViewFatimaPayroll> ReportsFilterImplementation(FiltersModel fm, List<ViewFatimaPayroll> _TempViewList, List<ViewFatimaPayroll> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        public List<ViewEmpAttMnAppOT> ReportsFilterImplementation(FiltersModel fm, List<ViewEmpAttMnAppOT> _TempViewListMonthlyApprovedOT, List<ViewEmpAttMnAppOT> _ViewListMonthlyDataApprovedOT)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewListMonthlyApprovedOT.AddRange(_ViewListMonthlyDataApprovedOT.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _TempViewListMonthlyApprovedOT = _ViewListMonthlyDataApprovedOT.ToList();
            }
            else
                _TempViewListMonthlyApprovedOT = _ViewListMonthlyDataApprovedOT.ToList();
            _TempViewListMonthlyApprovedOT.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewListMonthlyApprovedOT.AddRange(_ViewListMonthlyDataApprovedOT.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewListMonthlyDataApprovedOT = _TempViewListMonthlyApprovedOT.ToList();
            }
            else
                _TempViewListMonthlyApprovedOT = _ViewListMonthlyDataApprovedOT.ToList();
            _TempViewListMonthlyApprovedOT.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewListMonthlyApprovedOT.AddRange(_ViewListMonthlyDataApprovedOT.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewListMonthlyDataApprovedOT = _TempViewListMonthlyApprovedOT.ToList();
            }
            else
                _TempViewListMonthlyApprovedOT = _ViewListMonthlyDataApprovedOT.ToList();


            _TempViewListMonthlyApprovedOT.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewListMonthlyApprovedOT.AddRange(_ViewListMonthlyDataApprovedOT.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewListMonthlyDataApprovedOT = _TempViewListMonthlyApprovedOT.ToList();
            }
            else
                _TempViewListMonthlyApprovedOT = _ViewListMonthlyDataApprovedOT.ToList();
            _TempViewListMonthlyApprovedOT.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewListMonthlyApprovedOT.AddRange(_ViewListMonthlyDataApprovedOT.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewListMonthlyDataApprovedOT = _TempViewListMonthlyApprovedOT.ToList();
            }
            else
                _TempViewListMonthlyApprovedOT = _ViewListMonthlyDataApprovedOT.ToList();
            _TempViewListMonthlyApprovedOT.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewListMonthlyApprovedOT.AddRange(_ViewListMonthlyDataApprovedOT.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewListMonthlyDataApprovedOT = _TempViewListMonthlyApprovedOT.ToList();
            }
            else
                _TempViewListMonthlyApprovedOT = _ViewListMonthlyDataApprovedOT.ToList();
            _TempViewListMonthlyApprovedOT.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewListMonthlyApprovedOT.AddRange(_ViewListMonthlyDataApprovedOT.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewListMonthlyDataApprovedOT = _TempViewListMonthlyApprovedOT.ToList();
            }
            else
                _TempViewListMonthlyApprovedOT = _ViewListMonthlyDataApprovedOT.ToList();
            _TempViewListMonthlyApprovedOT.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewListMonthlyApprovedOT.AddRange(_ViewListMonthlyDataApprovedOT.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewListMonthlyDataApprovedOT = _TempViewListMonthlyApprovedOT.ToList();
            }
            else
                _TempViewListMonthlyApprovedOT = _ViewListMonthlyDataApprovedOT.ToList();
            _TempViewListMonthlyApprovedOT.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewListMonthlyApprovedOT.AddRange(_ViewListMonthlyDataApprovedOT.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewListMonthlyDataApprovedOT = _TempViewListMonthlyApprovedOT.ToList();
            }
            else
                _TempViewListMonthlyApprovedOT = _ViewListMonthlyDataApprovedOT.ToList();
            _TempViewListMonthlyApprovedOT.Clear();


            return _ViewListMonthlyDataApprovedOT;

        }


        private List<ViewJobCardAudit> ReportsFilterImplementation(FiltersModel fm, List<ViewJobCardAudit> _TempViewListeditor, List<ViewJobCardAudit> _ViewListeditor)
        {
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewListeditor.AddRange(_ViewListeditor.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewListeditor = _ViewListeditor.ToList();
            }
            else
                _TempViewListeditor = _ViewListeditor.ToList();
            _TempViewListeditor.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewListeditor.AddRange(_ViewListeditor.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewListeditor = _TempViewListeditor.ToList();
            }
            else
                _TempViewListeditor = _ViewListeditor.ToList();
            _TempViewListeditor.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewListeditor.AddRange(_ViewListeditor.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewListeditor = _TempViewListeditor.ToList();
            }
            else
                _TempViewListeditor = _ViewListeditor.ToList();
            _TempViewListeditor.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewListeditor.AddRange(_ViewListeditor.Where(aa => aa.CriteriaData == _empID).ToList());
                }
                _ViewListeditor = _TempViewListeditor.ToList();
            }
            else
                _TempViewListeditor = _ViewListeditor.ToList();
            _TempViewListeditor.Clear();


            return _ViewListeditor;
        }

        private List<Department> ReportsFilterImplementation(FiltersModel fm, List<Department> _TempViewList, List<Department> _ViewList)
        {
            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            return _ViewList;
        }

        private List<ViewAttDataOT> ReportsFilterImplementation(FiltersModel fm, List<ViewAttDataOT> _TempViewListeditor, List<ViewAttDataOT> _ViewListeditor)
        {
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewListeditor.AddRange(_ViewListeditor.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewListeditor = _ViewListeditor.ToList();
            }
            else
                _TempViewListeditor = _ViewListeditor.ToList();
            _TempViewListeditor.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewListeditor.AddRange(_ViewListeditor.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewListeditor = _TempViewListeditor.ToList();
            }
            else
                _TempViewListeditor = _ViewListeditor.ToList();
            _TempViewListeditor.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewListeditor.AddRange(_ViewListeditor.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewListeditor = _TempViewListeditor.ToList();
            }
            else
                _TempViewListeditor = _ViewListeditor.ToList();
            _TempViewListeditor.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewListeditor.AddRange(_ViewListeditor.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewListeditor = _TempViewListeditor.ToList();
            }
            else
                _TempViewListeditor = _ViewListeditor.ToList();
            _TempViewListeditor.Clear();


            return _ViewListeditor;
        }

        private List<ViewMultipleInOut> ReportsFilterImplementation(FiltersModel fm, List<ViewMultipleInOut> _TempViewList, List<ViewMultipleInOut> _ViewList)
        {

            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        private List<ViewAttBadliMonth> ReportsFilterImplementation(FiltersModel fm, List<ViewAttBadliMonth> _TempViewListB, List<ViewAttBadliMonth> _ViewListB)
        {
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewListB.AddRange(_ViewListB.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewListB = _TempViewListB.ToList();
            }
            else
                _TempViewListB = _ViewListB.ToList();
            _TempViewListB.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewListB.AddRange(_ViewListB.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewListB = _TempViewListB.ToList();
            }
            else
                _TempViewListB = _ViewListB.ToList();
            _TempViewListB.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewListB.AddRange(_ViewListB.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewListB = _TempViewListB.ToList();
            }
            else
                _TempViewListB = _ViewListB.ToList();


            _TempViewListB.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewListB.AddRange(_ViewListB.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewListB = _TempViewListB.ToList();
            }
            else
                _TempViewListB = _ViewListB.ToList();
            _TempViewListB.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewListB.AddRange(_ViewListB.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewListB = _TempViewListB.ToList();
            }
            else
                _TempViewListB = _ViewListB.ToList();
            _TempViewListB.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewListB.AddRange(_ViewListB.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewListB = _TempViewListB.ToList();
            }
            else
                _TempViewListB = _ViewListB.ToList();
            _TempViewListB.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewListB.AddRange(_ViewListB.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewListB = _TempViewListB.ToList();
            }
            else
                _TempViewListB = _ViewListB.ToList();
            _TempViewListB.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewListB.AddRange(_ViewListB.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewListB = _TempViewListB.ToList();
            }
            else
                _TempViewListB = _ViewListB.ToList();
            _TempViewListB.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewListB.AddRange(_ViewListB.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewListB = _TempViewListB.ToList();
            }
            else
                _TempViewListB = _ViewListB.ToList();
            _TempViewListB.Clear();


            return _ViewListB;
        }
        private List<ViewLvApplication> ReportsFilterImplementation(FiltersModel fm, List<ViewLvApplication> _TempViewList, List<ViewLvApplication> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        //EmpView
        private List<DailySummary> ReportsFilterImplementation(FiltersModel fm, string dateFrom, string dateTo, string Criteria)
        {
            List<DailySummary> ViewDS = new List<DailySummary>();
            List<DailySummary> TempDS = new List<DailySummary>();
            QueryBuilder qb = new QueryBuilder();
            DataTable dt = new DataTable();
            switch (Criteria)
            {
                case "C":
                    //for company
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.CompanyFilter.Count > 0)
                    {
                        foreach (var comp in fm.CompanyFilter)
                        {
                            short _compID = Convert.ToInt16(comp.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _compID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "L":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.LocationFilter.Count > 0)
                    {
                        foreach (var loc in fm.LocationFilter)
                        {
                            short _locID = Convert.ToInt16(loc.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _locID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "D":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.DepartmentFilter.Count > 0)
                    {
                        foreach (var dept in fm.DepartmentFilter)
                        {
                            short _deptID = Convert.ToInt16(dept.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _deptID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "E":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.SectionFilter.Count > 0)
                    {
                        foreach (var sec in fm.SectionFilter)
                        {
                            short _secID = Convert.ToInt16(sec.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _secID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;

                case "S":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.ShiftFilter.Count > 0)
                    {
                        foreach (var shift in fm.ShiftFilter)
                        {
                            short _shiftID = Convert.ToInt16(shift.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _shiftID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "T":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.TypeFilter.Count > 0)
                    {
                        foreach (var type in fm.TypeFilter)
                        {
                            short _typeID = Convert.ToInt16(type.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _typeID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                //case "A":
                //    if (fm.CompanyFilter.Count > 0)
                //    {
                //        foreach (var comp in fm.CompanyFilter)
                //        {
                //            short _compID = Convert.ToInt16(comp.ID);
                //            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _compID && aa.Criteria == Criteria).ToList());
                //        }
                //        ViewDS = TempDS.ToList();
                //    }
                //    else
                //        TempDS = ViewDS.ToList();
                //    TempDS.Clear();
                //    break;
            }
            return ViewDS;
        }
        public List<EmpView> ReportsFilterImplementation(FiltersModel fm, List<EmpView> _TempViewList, List<EmpView> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        private List<ViewBadli> ReportsFilterImplementation(FiltersModel fm, List<ViewBadli> _TempViewList, List<ViewBadli> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    //baldi doesnt have the division id so using division name in
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == Convert.ToInt16(div.ID)).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        //ViewAttData
        public List<ViewAttData> ReportsFilterImplementation(FiltersModel fm, List<ViewAttData> _TempViewList, List<ViewAttData> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        public List<ViewAttDataOld1> ReportsFilterImplementation(FiltersModel fm, List<ViewAttDataOld1> _TempViewList, List<ViewAttDataOld1> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        public List<EmpPhoto> GetCompanyImages(FiltersModel fm)
        {
            TAS2013Entities ctx = new TAS2013Entities();
            companyimage = new List<EmpPhoto>();
            if (fm.CompanyFilter.Count > 1)
            {
                companyimage.Add(ctx.EmpPhotoes.Where(aa => aa.PhotoID == 4785).First());
            }
            else
            {
                if (fm.CompanyFilter.Count > 0)
                {
                    int id = Int32.Parse(fm.CompanyFilter.First().ID);
                    Company comp = ctx.Companies.Where(aa => aa.CompID == id).FirstOrDefault();
                    companyimage.Add(ctx.EmpPhotoes.Where(aa => aa.PhotoID == comp.ImageID).First());
                }
                else
                {
                    companyimage.Add(ctx.EmpPhotoes.Where(aa => aa.PhotoID == 4785).First());
                }
            }

            return companyimage;

        }

        //ViewAttData
        public List<ViewDetailAttData> ReportsFilterImplementation(FiltersModel fm, List<ViewDetailAttData> _TempViewList, List<ViewDetailAttData> _ViewList)
        {

            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        //ViewMonthlyData
        public List<ViewMonthlyData> ReportsFilterImplementation(FiltersModel fm, List<ViewMonthlyData> _TempViewList, List<ViewMonthlyData> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        private List<ViewAttMonthBadli> ReportsFilterImplementation(FiltersModel fm, List<ViewAttMonthBadli> _TempViewList, List<ViewAttMonthBadli> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        //ViewMonthlyDataPer
        public List<ViewMonthlyDataPer> ReportsFilterImplementation(FiltersModel fm, List<ViewMonthlyDataPer> _TempViewList, List<ViewMonthlyDataPer> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        //DeviceData
        public List<ViewPollData> ReportsFilterImplementation(FiltersModel fm, List<ViewPollData> _TempViewList, List<ViewPollData> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        private List<ViewEditAttendance> ReportsFilterImplementation(FiltersModel fm, List<ViewEditAttendance> _TempViewList, List<ViewEditAttendance> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        #endregion

        #endregion

        #region --Commented Code --
        //private void LoadReport(string PathString, List<viewatt> list, string date)
        //{
        //    string _Header = title;
        //    this.ReportViewer1.LocalReport.DisplayName = title;
        //    ReportViewer1.ProcessingMode = ProcessingMode.Local;
        //    ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
        //    System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
        //    ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
        //    IEnumerable<ViewAttMnAppOT> ie;
        //    ie = list.AsQueryable();
        //    IEnumerable<EmpPhoto> companyImage;
        //    companyImage = companyimage.AsQueryable();
        //    ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
        //    ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
        //    ReportViewer1.LocalReport.DataSources.Clear();
        //    ReportViewer1.LocalReport.EnableExternalImages = true;
        //    ReportViewer1.LocalReport.DataSources.Add(datasource1);
        //    ReportViewer1.LocalReport.DataSources.Add(datasource2);
        //    ReportParameter rp = new ReportParameter("Date", date, false);
        //    ReportParameter rp1 = new ReportParameter("Header", _Header, false);
        //    this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
        //    ReportViewer1.LocalReport.Refresh();

        //}
        //Fatima Payroll report load Function


        // fatima Payeoll Report


        //public List<ViewFatimaPayroll> ReportsFilterImplementation(FiltersModel fm, List<ViewFatimaPayroll> _TempViewList, List<ViewFatimaPayroll> _ViewList)
        //{
        //    //for company
        //    if (fm.CompanyFilter.Count > 0)
        //    {
        //        foreach (var comp in fm.CompanyFilter)
        //        {
        //            short _compID = Convert.ToInt16(comp.ID);
        //            _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
        //        }
        //        _ViewList = _TempViewList.ToList();
        //    }
        //    else
        //        _TempViewList = _ViewList.ToList();
        //    _TempViewList.Clear();



        //    //for location
        //    if (fm.LocationFilter.Count > 0)
        //    {
        //        foreach (var loc in fm.LocationFilter)
        //        {
        //            short _locID = Convert.ToInt16(loc.ID);
        //            _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
        //        }
        //        _ViewList = _TempViewList.ToList();
        //    }
        //    else
        //        _TempViewList = _ViewList.ToList();
        //    _TempViewList.Clear();

        //    //for shifts
        //    if (fm.ShiftFilter.Count > 0)
        //    {
        //        foreach (var shift in fm.ShiftFilter)
        //        {
        //            short _shiftID = Convert.ToInt16(shift.ID);
        //            _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
        //        }
        //        _ViewList = _TempViewList.ToList();
        //    }
        //    else
        //        _TempViewList = _ViewList.ToList();


        //    _TempViewList.Clear();

        //    //for type
        //    if (fm.TypeFilter.Count > 0)
        //    {
        //        foreach (var type in fm.TypeFilter)
        //        {
        //            short _typeID = Convert.ToInt16(type.ID);
        //            _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
        //        }
        //        _ViewList = _TempViewList.ToList();
        //    }
        //    else
        //        _TempViewList = _ViewList.ToList();
        //    _TempViewList.Clear();

        //    //for crews
        //    if (fm.CrewFilter.Count > 0)
        //    {
        //        foreach (var cre in fm.CrewFilter)
        //        {
        //            short _crewID = Convert.ToInt16(cre.ID);
        //            _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
        //        }
        //        _ViewList = _TempViewList.ToList();
        //    }
        //    else
        //        _TempViewList = _ViewList.ToList();
        //    _TempViewList.Clear();





        //    //for division
        //    if (fm.DivisionFilter.Count > 0)
        //    {
        //        foreach (var div in fm.DivisionFilter)
        //        {
        //            short _divID = Convert.ToInt16(div.ID);
        //            _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
        //        }
        //        _ViewList = _TempViewList.ToList();
        //    }
        //    else
        //        _TempViewList = _ViewList.ToList();
        //    _TempViewList.Clear();

        //    //for department
        //    if (fm.DepartmentFilter.Count > 0)
        //    {
        //        foreach (var dept in fm.DepartmentFilter)
        //        {
        //            short _deptID = Convert.ToInt16(dept.ID);
        //            _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
        //        }
        //        _ViewList = _TempViewList.ToList();
        //    }
        //    else
        //        _TempViewList = _ViewList.ToList();
        //    _TempViewList.Clear();

        //    //for sections
        //    if (fm.SectionFilter.Count > 0)
        //    {
        //        foreach (var sec in fm.SectionFilter)
        //        {
        //            short _secID = Convert.ToInt16(sec.ID);
        //            _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
        //        }
        //        _ViewList = _TempViewList.ToList();
        //    }
        //    else
        //        _TempViewList = _ViewList.ToList();
        //    _TempViewList.Clear();

        //    //Employee
        //    if (fm.EmployeeFilter.Count > 0)
        //    {
        //        foreach (var emp in fm.EmployeeFilter)
        //        {
        //            int _empID = Convert.ToInt32(emp.ID);
        //            _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
        //        }
        //        _ViewList = _TempViewList.ToList();
        //    }
        //    else
        //        _TempViewList = _ViewList.ToList();
        //    _TempViewList.Clear();


        //    return _ViewList;
        //}


        #endregion

        #region -- Data tables region Old--

        #region -Leaves Report with LWOP and CPL

        private DataTable GetComplteLV(List<EmpView> _Emp, int month, DateTime startDate, DateTime EndDate)
        {
            using (var ctx = new TAS2013Entities())
            {

                List<LvData> _lvData = new List<LvData>();
                _lvData = ctx.LvDatas.Where(aa => aa.AttDate >= startDate && aa.AttDate <= EndDate).ToList();
                List<LvType> _lvTypes = ctx.LvTypes.ToList();
                double AL = 0;
                double CL = 0;
                double SL = 0;
                double Earned = 0;
                double CPL = 0;
                double LWOP = 0;
                double Maturnity = 0;
                double Accident = 0;
                double SSL = 0;
                //List<LvData> lvData = ctx.LvDatas.Where(aa=>aa.AttDate>=)
                foreach (var emp in _Emp)
                {
                    foreach (var lvType in _lvTypes)
                    {
                        List<LvData> LvDataTemp = new List<LvData>();
                        LvDataTemp = _lvData.Where(aa => aa.EmpID == emp.EmpID && aa.LvCode == lvType.LvType1).ToList();
                        foreach (var item in LvDataTemp)
                        {
                            if (item.HalfLeave == true)
                            {
                                if (item.LvCode == "A")//Casual
                                    CL = CL + 0.5;
                                if (item.LvCode == "B")//Annual
                                    AL = AL + 0.5;
                                if (item.LvCode == "C")//Sick
                                    SL = SL + 0.5;
                                if (item.LvCode == "D")//Earned
                                    Earned = Earned + 0.5;
                                if (item.LvCode == "E")//CPL
                                    CPL = CPL + 0.5;
                                if (item.LvCode == "F")//LWOP
                                    LWOP = LWOP + 0.5;
                                if (item.LvCode == "G")//Maturnity
                                    Maturnity = Maturnity + 0.5;
                                if (item.LvCode == "H")//Accident
                                    Accident = Accident + 0.5;
                                if (item.LvCode == "I")//Social Security
                                    SSL = SSL + 0.5;
                            }
                            else
                            {
                                if (item.LvCode == "A")//Casual
                                    CL = CL + 1;
                                if (item.LvCode == "B")//Annual
                                    AL = AL + 1;
                                if (item.LvCode == "C")//Sick
                                    SL = SL + 1;
                                if (item.LvCode == "D")//Earned
                                    Earned = Earned + 1;
                                if (item.LvCode == "E")//CPL
                                    CPL = CPL + 1;
                                if (item.LvCode == "F")//LWOP
                                    LWOP = LWOP + 1;
                                if (item.LvCode == "G")//Maturnity
                                    Maturnity = Maturnity + 1;
                                if (item.LvCode == "H")//Accident
                                    Accident = Accident + 1;
                                if (item.LvCode == "I")//Social Security
                                    SSL = SSL + 1;

                            }

                        }

                    }
                    AddCompleteLVDataToDT(emp.EmpNo, emp.EmpName, emp.DesignationName, emp.SectionName,
                        emp.DeptName, emp.TypeName, emp.CatName, emp.LocName, AL, CL, SL, Earned, CPL, LWOP, Maturnity, Accident, SSL);
                }
            }
            return ComplteLvSummaryMonth;
        }
        public void AddCompleteLVDataToDT(string EmpNo, string EmpName, string Designation, string Section,
                                 string Department, string EmpType, string Category, string Location,
                                double TotalAL, double TotalCL, double TotalSL, double TotalEarned, double TotalCPL, double TotalLWOP
            , double TotalMaturnity, double TotalAccident, double TotalSSL)
        {
            ComplteLvSummaryMonth.Rows.Add(EmpNo, EmpName, Designation, Section, Department, EmpType, Category, Location,
                TotalAL, TotalCL, TotalSL, TotalEarned, TotalCPL, TotalLWOP, TotalMaturnity, TotalAccident, TotalSSL);
        }
        DataTable ComplteLvSummaryMonth = new DataTable();
        #endregion
        #region -- Emp Summary Report with Time Difference --
        DataTable EmpSummTimes = new DataTable();
        public void CreateEmpSumTimeDatatable()
        {
            EmpSummTimes.Columns.Add("EmpDate", typeof(string));
            EmpSummTimes.Columns.Add("AttDate", typeof(DateTime));
            EmpSummTimes.Columns.Add("EmpNo", typeof(string));
            EmpSummTimes.Columns.Add("EmpID", typeof(int));
            EmpSummTimes.Columns.Add("EmpName", typeof(string));
            EmpSummTimes.Columns.Add("SectionName", typeof(string));
            EmpSummTimes.Columns.Add("DeptName", typeof(string));
            EmpSummTimes.Columns.Add("TypeName", typeof(string));
            EmpSummTimes.Columns.Add("LocName", typeof(string));
            EmpSummTimes.Columns.Add("ShiftName", typeof(string));
            EmpSummTimes.Columns.Add("Tin0", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut0", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff0", typeof(int));
            EmpSummTimes.Columns.Add("Tin1", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut1", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff1", typeof(int));
            EmpSummTimes.Columns.Add("Tin2", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut2", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff2", typeof(int));
            EmpSummTimes.Columns.Add("Tin3", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut3", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff3", typeof(int));
            EmpSummTimes.Columns.Add("Tin4", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut4", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff4", typeof(int));
            EmpSummTimes.Columns.Add("Tin5", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut5", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff5", typeof(int));
            EmpSummTimes.Columns.Add("WorkMin", typeof(int));
            EmpSummTimes.Columns.Add("ShiftMin", typeof(int));
            EmpSummTimes.Columns.Add("DutyCode", typeof(string));
            EmpSummTimes.Columns.Add("Loss", typeof(int));
            EmpSummTimes.Columns.Add("Extra", typeof(int));
            EmpSummTimes.Columns.Add("Remarks", typeof(string));
        }

        public DataTable AddEmpSumTime(List<ViewDetailAttData> attDatas, List<EmpView> emps)
        {
            List<Shift> shift = new List<Shift>();
            foreach (var emp in emps)
            {
                List<ViewDetailAttData> tempAttData = new List<ViewDetailAttData>();
                tempAttData = attDatas.Where(aa => aa.EmpID == emp.EmpID).ToList();
                foreach (var attdata in tempAttData)
                {
                    int diff0 = 0;
                    int diff1 = 0;
                    int diff2 = 0;
                    int diff3 = 0;
                    int diff4 = 0;
                    int diff5 = 0;
                    int WorkMins = 0;
                    int Loss = 0;
                    int Extra = 0;
                    if (attdata.StatusMN == true)
                    {
                        if (attdata.TimeIn != null && attdata.TimeOut != null)
                        {
                            TimeSpan t0 = (TimeSpan)(attdata.TimeOut - attdata.TimeIn);
                            diff0 = (int)t0.TotalMinutes;
                            attdata.Tin0 = attdata.TimeIn;
                            attdata.Tout0 = attdata.TimeOut;
                        }
                    }
                    else
                    {
                        #region -- Calculate Time Difference --
                        if (attdata.Tin0 != null && attdata.Tout0 != null)
                        {
                            if (attdata.Tout0 < attdata.Tin0)
                            {
                                if (attdata.Tout1 != null)
                                {
                                    attdata.Tout0 = attdata.Tout1;
                                }
                            }
                            TimeSpan t0 = (TimeSpan)(attdata.Tout0 - attdata.Tin0);
                            diff0 = (int)t0.TotalMinutes;
                        }
                        if (attdata.Tin1 != null && attdata.Tout1 != null)
                        {
                            if (attdata.Tin0 == attdata.Tin1 && attdata.Tout0 == attdata.Tout1)
                            {
                                attdata.Tin1 = null;
                                attdata.Tout1 = null;
                            }
                            else
                            {
                                TimeSpan t1 = (TimeSpan)(attdata.Tout1 - attdata.Tin1);
                                diff1 = (int)t1.TotalMinutes;
                            }
                        }
                        if (attdata.Tin2 != null && attdata.Tout2 != null)
                        {
                            if ((attdata.Tin1 == attdata.Tin2 || attdata.Tin2 == attdata.Tin0) &&
                                (attdata.Tout1 == attdata.Tout2 || attdata.Tout2 == attdata.Tout0))
                            {
                                attdata.Tin2 = null;
                                attdata.Tout2 = null;
                            }
                            else
                            {
                                TimeSpan t2 = (TimeSpan)(attdata.Tout2 - attdata.Tin2);
                                diff2 = (int)t2.TotalMinutes;
                            }
                        }
                        if (attdata.Tin3 != null && attdata.Tout3 != null)
                        {
                            if ((attdata.Tin2 == attdata.Tin3 || attdata.Tin3 == attdata.Tin0) &&
                                (attdata.Tout2 == attdata.Tout3 || attdata.Tout3 == attdata.Tout0))
                            {
                                attdata.Tin3 = null;
                                attdata.Tout3 = null;
                            }
                            else
                            {
                                TimeSpan t3 = (TimeSpan)(attdata.Tout3 - attdata.Tin3);
                                diff3 = (int)t3.TotalMinutes;
                            }
                        }
                        if (attdata.Tin4 != null && attdata.Tout4 != null)
                        {
                            if ((attdata.Tin3 == attdata.Tin4 || attdata.Tin4 == attdata.Tin0)
                                && (attdata.Tout3 == attdata.Tout4 || attdata.Tout4 == attdata.Tout0))
                            {

                            }
                            else
                            {
                                TimeSpan t4 = (TimeSpan)(attdata.Tout4 - attdata.Tin4);
                                diff4 = (int)t4.TotalMinutes;
                            }
                        }
                        if (attdata.Tin5 != null && attdata.Tout5 != null)
                        {
                            if (attdata.Tin4 == attdata.Tin5 && attdata.Tout4 == attdata.Tout5)
                            {
                            }
                            else
                            {
                                TimeSpan t5 = (TimeSpan)(attdata.Tout5 - attdata.Tin5);
                                diff5 = (int)t5.TotalMinutes;
                            }
                        }
                        #endregion
                    }
                    WorkMins = diff0 + diff1 + diff2 + diff3 + diff4 + diff5;
                    if (attdata.ShiftID == 1)
                    {
                        if (attdata.StatusMN == true)
                        {
                            //if (WorkMins > 300)
                            //{
                            //    if (attdata.AttDate.Value.Day < 5)
                            //        WorkMins = WorkMins - 0;
                            //    else
                            //        WorkMins = WorkMins - 60;
                            //}
                        }
                        else
                        {
                            //if (WorkMins > 300)
                            //{
                            //    if (attdata.AttDate.Value.Day < 5)
                            //        WorkMins = WorkMins - 0;
                            //    else
                            //        WorkMins = WorkMins - 60;
                            //}
                        }
                        if (attdata.ShifMin > 0)
                        {
                            if (attdata.AttDate.Value.DayOfWeek != DayOfWeek.Friday)
                            {
                                //if (attdata.AttDate.Value.Day < 5)
                                //    attdata.ShifMin = (short)(attdata.ShifMin - 0);
                                //else
                                attdata.ShifMin = (short)(attdata.ShifMin - 60);
                            }
                        }
                    }
                    else if (attdata.ShiftID == 3)
                    {
                        if (attdata.DutyCode == "G")
                        {
                            attdata.DutyCode = "D";
                            attdata.ShifMin = 480;
                            if (attdata.Expr3 != null)
                                attdata.Expr3 = attdata.Expr3.Replace("[GZ]", "");
                        }
                    }
                    if (attdata.DutyCode != "D")
                    {
                        Extra = WorkMins;
                        WorkMins = 0;
                    }
                    else
                    {
                        if (WorkMins > attdata.ShifMin)
                        {
                            Extra = (int)(WorkMins - attdata.ShifMin);
                            WorkMins = WorkMins - Extra;
                        }
                        else
                        {
                            Loss = (int)(attdata.ShifMin - WorkMins);
                        }
                    }

                    AddDataToSumTimeDatatable(attdata.EmpDate, attdata.AttDate, attdata.EmpNo, attdata.EmpID, emp.EmpName, emp.SectionName,
                        emp.DeptName, emp.TypeName, emp.LocName, emp.ShiftName, attdata.Tin0, attdata.Tout0, diff0, attdata.Tin1, attdata.Tout1,
                        diff1, attdata.Tin2, attdata.Tout2, diff2, attdata.Tin3, attdata.Tout3, diff3, attdata.Tin4, attdata.Tout4, diff4,
                        attdata.Tin5, attdata.Tout5, diff5, WorkMins, attdata.ShifMin, attdata.DutyCode, Loss, Extra, attdata.Expr3);

                }
            }
            return EmpSummTimes;
        }
        public void AddDataToSumTimeDatatable(string EmpDate, DateTime? AttDate, string EmpNo, int? EmpID, string EmpName,
    string SectionName, string DeptName, string TypeName, string LocName, string ShiftName,
                                      DateTime? Tin0, DateTime? Tout0, int? Diff0, DateTime? Tin1, DateTime? Tout1, int? Diff1,
    DateTime? Tin2, DateTime? Tout2, int? Diff2, DateTime? Tin3, DateTime? Tout3, int? Diff3, DateTime? Tin4, DateTime? Tout4, int? Diff4,
    DateTime? Tin5, DateTime? Tout5, int? Diff5, int? WorkMin, int? ShiftMin, string DutyCode, int? LossMin, int? ExtraMin, string Remarks)
        {
            EmpSummTimes.Rows.Add(EmpDate, AttDate, EmpNo, EmpID, EmpName, SectionName, DeptName, TypeName, LocName, ShiftName, Tin0,
                Tout0, Diff0, Tin1, Tout1, Diff1, Tin2, Tout2, Diff2, Tin3, Tout3, Diff3, Tin4, Tout4, Diff4, Tin5, Tout5, Diff5,
                WorkMin, ShiftMin, DutyCode, LossMin, ExtraMin, Remarks);
        }
        #endregion

        #region Not good Place

        private DataTable LoadPermanentMonthlyDT(List<EmpView> list, DateTime datefrom, DateTime dateto)
        {
            TAS2013Entities db = new TAS2013Entities();
            List<AttData> attData = new List<AttData>();
            List<AttData> _EmpAttData = new List<AttData>();
            foreach (EmpView emp in list)
            {
                try
                {
                    attData = db.AttDatas.Where(aa => aa.EmpID == emp.EmpID && aa.AttDate >= datefrom && aa.AttDate <= dateto).ToList();
                    PermanentMonthly cmp = new PermanentMonthly();
                    _EmpAttData = attData.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    AttMnDataPer attMn = new AttMnDataPer();
                    attMn = cmp.processPermanentMonthlyAttSingle((DateTime)datefrom, (DateTime)dateto, emp, _EmpAttData);
                    AddDataToMonthlyDataAtable(attMn.Period.ToString(), attMn.EmpMonth.ToString(), (DateTime)attMn.StartDate, (DateTime)attMn.EndDate, attMn.EmpNo, (int)attMn.EmpID, attMn.EmpName, emp.SectionName, emp.DeptName, emp.TypeName, emp.LocName, emp.ShiftName, attMn.D21, attMn.D22, attMn.D23, attMn.D24, attMn.D25, attMn.D26, attMn.D27, attMn.D28, attMn.D29, attMn.D30, attMn.D31, attMn.D1, attMn.D2, attMn.D3, attMn.D4, attMn.D5, attMn.D6, attMn.D7, attMn.D8, attMn.D9, attMn.D10, attMn.D11, attMn.D12, attMn.D13, attMn.D14, attMn.D15, attMn.D16, attMn.D17, attMn.D18, attMn.D19, attMn.D20, (short)attMn.TotalDays, (short)attMn.WorkDays, (short)attMn.PreDays, (short)attMn.AbDays, (short)attMn.RestDays, (short)attMn.GZDays, (short)attMn.LeaveDays, (short)attMn.OfficialDutyDays, (short)attMn.TEarlyIn, (short)attMn.TEarlyOut, (short)attMn.TLateIn, (short)attMn.TLateOut, (short)attMn.TWorkTime, (short)attMn.TNOT, (short)attMn.TGZOT, (short)attMn.ExpectedWrkTime, (short)attMn.OT1, (short)attMn.OT2, (short)attMn.OT3, (short)attMn.OT4, (short)attMn.OT5, (short)attMn.OT6, (short)attMn.OT7, (short)attMn.OT8, (short)attMn.OT9, (short)attMn.OT10, (short)attMn.OT11, (short)attMn.OT12, (short)attMn.OT13, (short)attMn.OT14, (short)attMn.OT15, (short)attMn.OT16, (short)attMn.OT17, (short)attMn.OT18, (short)attMn.OT19, (short)attMn.OT20, (short)attMn.OT21, (short)attMn.OT22, (short)attMn.OT23, (short)attMn.OT24, (short)attMn.OT25, (short)attMn.OT26, (short)attMn.OT27, (short)attMn.OT28, (short)attMn.OT29, (short)attMn.OT30, (short)attMn.OT31);
                }
                catch (Exception ex)
                {

                }
            }
            return FlexyMonthlyReportDT;
        }

        #endregion
        #endregion

        #region -- Monthly and Yearly Leave Calculation Helper Functions --
        #region --Leave Process--
        private DataTable GetLV(List<EmpView> _Emp, int month, DateTime dateTimeLv)
        {
            using (var ctx = new TAS2013Entities())
            {

                List<LvConsumed> _lvConsumed = new List<LvConsumed>();
                LvConsumed _lvTemp = new LvConsumed();
                string year = dateTimeLv.Year.ToString();
                _lvConsumed = ctx.LvConsumeds.Where(aa => aa.Year == year).ToList();
                List<LvType> _lvTypes = ctx.LvTypes.ToList();
                //List<LvData> lvData = ctx.LvDatas.Where(aa=>aa.AttDate>=)
                foreach (var emp in _Emp)
                {
                    float BeforeCL = 0, UsedCL = 0, BalCL = 0;
                    float BeforeSL = 0, UsedSL = 0, BalSL = 0;
                    float BeforeAL = 0, UsedAL = 0, BalAL = 0;
                    float BeforeCPL = 0, UsedCPL = 0, BalCPL = 0;
                    string _month = "";
                    List<LvConsumed> entries = _lvConsumed.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    string EmpLvC = emp.EmpID.ToString() + "A" + dateTimeLv.Year.ToString();
                    string EmpLvS = emp.EmpID.ToString() + "C" + dateTimeLv.Year.ToString();
                    string EmpLvA = emp.EmpID.ToString() + "B" + dateTimeLv.Year.ToString();
                    string EmpLvE = emp.EmpID.ToString() + "E" + dateTimeLv.Year.ToString();
                    //string EmpLvA = emp.EmpID.ToString() + "B" + dateTimeLv.Year.ToString();
                    LvConsumed eCL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvC);
                    LvConsumed eSL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvS);
                    LvConsumed eAL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvA);
                    LvConsumed eCPL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvE);
                    if (entries.Count > 0 && eCL != null && eSL != null && eAL != null)
                    {
                        switch (month)
                        {
                            case 1:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear;
                                UsedCL = (float)eCL.JanConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear;
                                UsedSL = (float)eSL.JanConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear;
                                UsedAL = (float)eAL.JanConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear;
                                //UsedCPL = (float)eCPL.JanConsumed;
                                _month = "January";
                                break;
                            case 2:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - (float)eCL.JanConsumed;
                                UsedCL = (float)eCL.FebConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - (float)eSL.JanConsumed;
                                UsedSL = (float)eSL.FebConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - (float)eAL.JanConsumed;
                                UsedAL = (float)eAL.FebConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - (float)eCPL.JanConsumed;
                                //UsedCPL = (float)eCPL.FebConsumed;
                                break;
                                _month = "Febu";
                            case 3:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed);
                                UsedCL = (float)eCL.MarchConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed);
                                UsedSL = (float)eSL.MarchConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed);
                                UsedAL = (float)eAL.MarchConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed);
                                UsedAL = (float)eAL.MarchConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed);
                                //UsedCPL = (float)eCPL.MarchConsumed;
                                break;
                            case 4:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed);
                                UsedCL = (float)eCL.AprConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed);
                                UsedSL = (float)eSL.AprConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed);
                                UsedAL = (float)eAL.AprConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed);
                                //UsedCPL = (float)eCPL.AprConsumed;
                                break;
                            case 5:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed);
                                UsedCL = (float)eCL.MayConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed);
                                UsedSL = (float)eSL.MayConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed);
                                UsedAL = (float)eAL.MayConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed);
                                //UsedCPL = (float)eCPL.MayConsumed;
                                break;
                            case 6:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed);
                                UsedCL = (float)eCL.JuneConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed);
                                UsedSL = (float)eSL.JuneConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed);
                                UsedAL = (float)eAL.JuneConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed);
                                //UsedCPL = (float)eCPL.JuneConsumed;
                                break;
                            case 7:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed);
                                UsedCL = (float)eCL.JulyConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed);
                                UsedSL = (float)eSL.JulyConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed);
                                UsedAL = (float)eAL.JulyConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed);
                                //UsedCPL = (float)eCPL.JulyConsumed;
                                break;
                            case 8:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed);
                                UsedCL = (float)eCL.AugustConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed);
                                UsedSL = (float)eSL.AugustConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed);
                                UsedAL = (float)eAL.AugustConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed + (float)eCPL.JulyConsumed);
                                //UsedCPL = (float)eCPL.AugustConsumed;
                                break;
                            case 9:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed);
                                UsedCL = (float)eCL.SepConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed);
                                UsedSL = (float)eSL.SepConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed);
                                UsedAL = (float)eAL.SepConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed + (float)eCPL.JulyConsumed + (float)eCPL.AugustConsumed);
                                //UsedCPL = (float)eCPL.SepConsumed;
                                break;
                            case 10:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed);
                                UsedCL = (float)eCL.OctConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed);
                                UsedSL = (float)eSL.OctConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.AugustConsumed);
                                UsedAL = (float)eAL.OctConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed + (float)eCPL.JulyConsumed + (float)eCPL.AugustConsumed + (float)eCPL.AugustConsumed);
                                //UsedCPL = (float)eCPL.SepConsumed;
                                break;
                            case 11:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed + (float)eCL.OctConsumed);
                                UsedCL = (float)eCL.NovConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed + (float)eSL.OctConsumed);
                                UsedSL = (float)eSL.NovConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.AugustConsumed + (float)eAL.OctConsumed);
                                UsedAL = (float)eAL.NovConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed + (float)eCPL.JulyConsumed + (float)eCPL.AugustConsumed + (float)eCPL.AugustConsumed + (float)eCPL.OctConsumed);
                                //UsedCPL = (float)eCPL.NovConsumed;
                                break;
                            case 12:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed + (float)eCL.OctConsumed + (float)eCL.NovConsumed);
                                UsedCL = (float)eCL.DecConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed + (float)eSL.OctConsumed + (float)eSL.NovConsumed);
                                UsedSL = (float)eSL.DecConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.SepConsumed + (float)eAL.OctConsumed + (float)eAL.NovConsumed);
                                UsedAL = (float)eAL.DecConsumed;
                                ////CPL
                                //BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed + (float)eCPL.JulyConsumed + (float)eCPL.AugustConsumed + (float)eAL.AugustConsumed + (float)eCPL.OctConsumed + (float)eCPL.NovConsumed);
                                //UsedCPL = (float)eCPL.DecConsumed;
                                break;

                        }
                        BalCL = (float)(BeforeCL - UsedCL);
                        BalSL = (float)(BeforeSL - UsedSL);
                        BalAL = (float)(BeforeAL - UsedAL);
                        BalCPL = (float)(BeforeCPL - UsedCPL);
                        AddDataToDT(emp.EmpNo, emp.EmpName, emp.DesignationName, emp.SectionName,
                            emp.DeptName, emp.TypeName, emp.CatName, emp.LocName,
                            BeforeCL, BeforeSL, BeforeAL, BeforeCPL, UsedCL, UsedSL, UsedAL, UsedCPL, BalCL, BalSL, BalAL, BalCPL, _month);
                    }

                }
            }
            return LvSummaryMonth;
        }
        public void AddDataToDT(string EmpNo, string EmpName, string Designation, string Section,
                                 string Department, string EmpType, string Category, string Location,
                                 float TotalCL, float TotalSL, float TotalAL, float TotalCPL,
                                 float ConsumedCL, float ConsumedSL, float ConsumedAL, float ConsumedCPL,
                                 float BalanaceCL, float BalanaceSL, float BalananceAL, float BalanceCPL, string Month)
        {
            LvSummaryMonth.Rows.Add(EmpNo, EmpName, Designation, Section, Department, EmpType, Category, Location,
                TotalCL, TotalSL, TotalAL, TotalCPL, ConsumedCL, ConsumedSL, ConsumedAL, ConsumedCPL,
                BalanaceCL, BalanaceSL, BalananceAL, BalanceCPL, Month);
        }
        DataTable LvSummaryMonth = new DataTable();


        #endregion

        private DataTable GYL(List<EmpView> _Emp, DateTime dateTimeLv)
        {
            TAS2013Entities context = new TAS2013Entities();
            List<LvConsumed> leaveQuota = new List<LvConsumed>();
            List<LvConsumed> tempLeaveQuota = new List<LvConsumed>();
            string year = dateTimeLv.Year.ToString();
            leaveQuota = context.LvConsumeds.Where(aa => aa.Year == year).ToList();
            foreach (var emp in _Emp)
            {
                int EmpID = 0;
                string EmpNo = ""; string EmpName = "";
                float TotalAL = 0; float BalAL = 0; float TotalCL = 0; float BalCL = 0; float TotalSL = 0; float BalSL = 0;
                float JanAL = 0; float JanCL = 0; float JanSL = 0; float FebAL = 0; float FebCL = 0; float FebSL = 0;
                float MarchAL = 0; float MarchCL = 0; float MarchSL = 0;
                float AprilAL = 0; float AprilCL = 0; float AprilSL = 0;
                float MayAL = 0; float MayCL = 0; float MaySL = 0;
                float JunAL = 0; float JunCL = 0; float JunSL = 0;
                float JullyAL = 0; float JullyCL = 0; float JullySL = 0;
                float AugAL = 0; float AugCL = 0; float AugSL = 0;
                float SepAL = 0; float SepCL = 0; float SepSL = 0;
                float OctAL = 0; float OctCL = 0; float OctSL = 0;
                float NovAL = 0; float NovCL = 0; float NovSL = 0;
                float DecAL = 0; float DecCL = 0; float DecSL = 0;
                DateTime dt = DateTime.Today;
                string Remarks = ""; string DeptName = ""; short DeptID = 0; string LocationName = ""; short LocationID = 0; string SecName = ""; short SecID = 0; string DesgName = ""; short DesigID = 0; string CrewName = ""; short CrewID = 0; string CompanyName = ""; short CompanyID = 0;
                tempLeaveQuota = leaveQuota.Where(aa => aa.EmpID == emp.EmpID).ToList();
                foreach (var leave in tempLeaveQuota)
                {
                    EmpID = emp.EmpID;
                    EmpNo = emp.EmpNo;
                    EmpName = emp.EmpName;
                    DeptID = (short)emp.DeptID;
                    DeptName = emp.DeptName;
                    LocationName = emp.LocName;
                    LocationID = (short)emp.LocID;
                    SecName = emp.SectionName;
                    SecID = (short)emp.SecID;
                    DesgName = emp.DesignationName;
                    DesigID = (short)emp.DesigID;
                    CrewName = emp.CrewName;
                    CrewID = (short)emp.CrewID;
                    CompanyName = emp.CompName;
                    CompanyID = (short)emp.CompanyID;

                    if (emp.JoinDate != null)
                        dt = (DateTime)emp.JoinDate;
                    switch (leave.LeaveType)
                    {
                        case "A"://Casual
                            JanCL = (float)leave.JanConsumed;
                            FebCL = (float)leave.FebConsumed;
                            MarchCL = (float)leave.MarchConsumed;
                            AprilCL = (float)leave.AprConsumed;
                            MayCL = (float)leave.MayConsumed;
                            JunCL = (float)leave.JuneConsumed;
                            JullyCL = (float)leave.JulyConsumed;
                            AugCL = (float)leave.AugustConsumed;
                            SepCL = (float)leave.SepConsumed;
                            OctCL = (float)leave.OctConsumed;
                            NovCL = (float)leave.NovConsumed;
                            DecCL = (float)leave.DecConsumed;
                            TotalCL = (float)leave.TotalForYear;
                            BalCL = (float)leave.YearRemaining;
                            break;
                        case "B"://Anual
                            JanAL = (float)leave.JanConsumed;
                            FebAL = (float)leave.FebConsumed;
                            MarchAL = (float)leave.MarchConsumed;
                            AprilAL = (float)leave.AprConsumed;
                            MayAL = (float)leave.MayConsumed;
                            JunAL = (float)leave.JuneConsumed;
                            JullyAL = (float)leave.JulyConsumed;
                            AugAL = (float)leave.AugustConsumed;
                            SepAL = (float)leave.SepConsumed;
                            OctAL = (float)leave.OctConsumed;
                            NovAL = (float)leave.NovConsumed;
                            DecAL = (float)leave.DecConsumed;
                            TotalAL = (float)leave.TotalForYear;
                            BalAL = (float)leave.YearRemaining;
                            break;
                        case "C"://Sick
                            JanSL = (float)leave.JanConsumed;
                            FebSL = (float)leave.FebConsumed;
                            MarchSL = (float)leave.MarchConsumed;
                            AprilSL = (float)leave.AprConsumed;
                            MaySL = (float)leave.MayConsumed;
                            JunSL = (float)leave.JuneConsumed;
                            JullySL = (float)leave.JulyConsumed;
                            AugSL = (float)leave.AugustConsumed;
                            SepSL = (float)leave.SepConsumed;
                            OctSL = (float)leave.OctConsumed;
                            NovSL = (float)leave.NovConsumed;
                            DecSL = (float)leave.DecConsumed;
                            TotalSL = (float)leave.TotalForYear;
                            BalSL = (float)leave.YearRemaining;
                            break;
                    }
                }
                if (tempLeaveQuota.Count > 0)
                    AddDataToDT2(EmpID, EmpNo, EmpName, dt, TotalAL, BalAL, TotalCL, BalCL, TotalSL, BalSL, JanAL, JanCL, JanSL, FebAL, FebCL, FebSL, MarchAL, MarchCL, MarchSL, AprilAL, AprilCL, AprilSL, MayAL, MayCL, MaySL, JunAL, JunCL, JunSL, JullyAL, JullyCL, JullySL, AugAL, AugCL, AugSL, SepAL, SepCL, SepSL, OctAL, OctCL, OctSL, NovAL, NovCL, NovSL, DecAL, DecCL, DecSL, Remarks, DeptName, (short)DeptID, LocationName, (short)LocationID, SecName, (short)SecID, DesgName, DesigID, CrewName, CrewID, CompanyName, (short)CompanyID);

            }
            return MYLeaveSummaryDT;
        }
        DataTable MYLeaveSummaryDT = new DataTable();




        public void CreateDataTable()
        {
            MYLeaveSummaryDT.Columns.Add("EmpID", typeof(int));
            MYLeaveSummaryDT.Columns.Add("EmpNo", typeof(string));
            MYLeaveSummaryDT.Columns.Add("EmpName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("DOJ", typeof(DateTime));

            MYLeaveSummaryDT.Columns.Add("TotalAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("BalAL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("TotalCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("BalCL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("TotalSL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("BalSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("JanAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JanCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JanSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("FebAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("FebCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("FebSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("MarchAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MarchCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MarchSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("AprilAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AprilCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AprilSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("MayAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MayCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MaySL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("JunAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JunCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JunSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("JulyAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JulyCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JulySL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("AugAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AugCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AugSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("SepAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("SepCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("SepSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("OctAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("OctCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("OctSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("NovAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("NovCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("NovSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("DecAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("DecCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("DecSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("Remarks", typeof(string));
            MYLeaveSummaryDT.Columns.Add("DeptName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("DeptID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("SecName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("SecID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("DesgName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("DesgID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("CrewName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("CrewID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("CompanyName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("CompanyID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("LocationName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("LocationID", typeof(short));
            LvSummaryMonth.Columns.Add("EmpNo", typeof(string));
            LvSummaryMonth.Columns.Add("EmpName", typeof(string));
            LvSummaryMonth.Columns.Add("Designation", typeof(string));
            LvSummaryMonth.Columns.Add("Section", typeof(string));
            LvSummaryMonth.Columns.Add("Department", typeof(string));
            LvSummaryMonth.Columns.Add("EmpType", typeof(string));
            LvSummaryMonth.Columns.Add("Category", typeof(string));
            LvSummaryMonth.Columns.Add("Location", typeof(string));
            LvSummaryMonth.Columns.Add("TotalCL", typeof(float));
            LvSummaryMonth.Columns.Add("TotalSL", typeof(float));
            LvSummaryMonth.Columns.Add("TotalAL", typeof(float));
            LvSummaryMonth.Columns.Add("TotalCPL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedCL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedSL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedAL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedCPL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceCL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceSL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceAL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceCPL", typeof(float));
            LvSummaryMonth.Columns.Add("Remarks", typeof(string));
            LvSummaryMonth.Columns.Add("Month", typeof(string));

            ComplteLvSummaryMonth.Columns.Add("EmpNo", typeof(string));
            ComplteLvSummaryMonth.Columns.Add("EmpName", typeof(string));
            ComplteLvSummaryMonth.Columns.Add("Designation", typeof(string));
            ComplteLvSummaryMonth.Columns.Add("Section", typeof(string));
            ComplteLvSummaryMonth.Columns.Add("Department", typeof(string));
            ComplteLvSummaryMonth.Columns.Add("EmpType", typeof(string));
            ComplteLvSummaryMonth.Columns.Add("Category", typeof(string));
            ComplteLvSummaryMonth.Columns.Add("Location", typeof(string));
        }

        public void AddDataToDT2(int EmpID, string EmpNo, string EmpName, DateTime DOJ, float TotalAL, float BalAL,
            float TotalCL, float BalCL, float TotalSL, float BalSL,
            float JanAL, float JanCL, float JanSL,
            float FebAL, float FebCL, float FebSL,
            float MarchAL, float MarchCL, float MarchSL,
            float AprilAL, float AprilCL, float AprilSL,
            float MayAL, float MayCL, float MaySL,
            float JunAL, float JunCL, float JunSL,
            float JullyAL, float JullyCL, float JullySL,
            float AugAL, float AugCL, float AugSL,
            float SepAL, float SepCL, float SepSL,
            float OctAL, float OctCL, float OctSL,
            float NovAL, float NovCL, float NovSL,
            float DecAL, float DecCL, float DecSL,
            string Remarks, string DeptName, short DeptID, string LocationName, short LocationID, string SecName, short SecID, string DesgName, short DesgID, string CrewName, short CrewID, string CompanyName, short CompanyID)
        {
            MYLeaveSummaryDT.Rows.Add(EmpID, EmpNo, EmpName, DOJ, TotalAL, BalAL, TotalCL, BalCL, TotalSL, BalSL, JanAL, JanCL, JanSL, FebAL, FebCL, FebSL, MarchAL, MarchCL, MarchSL,
                AprilAL, AprilCL, AprilSL, MayAL, MayCL, MaySL, JunAL, JunCL, JunSL, JullyAL, JullyCL, JullySL, AugAL, AugCL, AugSL,
                SepAL, SepCL, SepSL, OctAL, OctCL, OctSL, NovAL, NovCL, NovSL, DecAL, DecCL, DecSL, Remarks, DeptName, DeptID, SecName, SecID, DesgName, DesgID, CrewName, CrewID, LocationName, LocationID, CompanyName, CompanyID);
        }

        private DataTable GetLVCPL(List<EmpView> _Emp, int month, DateTime dateTimeLv)
        {
            using (var ctx = new TAS2013Entities())
            {

                List<LvConsumed> _lvConsumed = new List<LvConsumed>();
                LvConsumed _lvTemp = new LvConsumed();
                string year = dateTimeLv.Year.ToString();
                _lvConsumed = ctx.LvConsumeds.Where(aa => aa.Year == year).ToList();
                List<LvType> _lvTypes = ctx.LvTypes.ToList();
                //List<LvData> lvData = ctx.LvDatas.Where(aa=>aa.AttDate>=)
                foreach (var emp in _Emp)
                {
                    float BeforeCL = 0, UsedCL = 0, BalCL = 0;
                    float BeforeSL = 0, UsedSL = 0, BalSL = 0;
                    float BeforeAL = 0, UsedAL = 0, BalAL = 0;
                    float BeforeCPL = 0, UsedCPL = 0, BalCPL = 0;
                    string _month = "";
                    List<LvConsumed> entries = _lvConsumed.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    string EmpLvC = emp.EmpID.ToString() + "A" + dateTimeLv.Year.ToString();
                    string EmpLvS = emp.EmpID.ToString() + "C" + dateTimeLv.Year.ToString();
                    string EmpLvA = emp.EmpID.ToString() + "B" + dateTimeLv.Year.ToString();
                    string EmpLvE = emp.EmpID.ToString() + "E" + dateTimeLv.Year.ToString();
                    //string EmpLvA = emp.EmpID.ToString() + "B" + dateTimeLv.Year.ToString();
                    LvConsumed eCL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvC);
                    LvConsumed eSL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvS);
                    LvConsumed eAL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvA);
                    LvConsumed eCPL = new LvConsumed();
                    if (entries.Where(lv => lv.EmpLvTypeYear == EmpLvE).Count() > 0)
                        eCPL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvE);
                    else
                    {
                        eCPL.JanConsumed = 0;
                        eCPL.FebConsumed = 0;
                        eCPL.MarchConsumed = 0;
                        eCPL.AprConsumed = 0;
                        eCPL.MayConsumed = 0;
                        eCPL.JuneConsumed = 0;
                        eCPL.JulyConsumed = 0;
                        eCPL.AugustConsumed = 0;
                        eCPL.SepConsumed = 0;
                        eCPL.OctConsumed = 0;
                        eCPL.NovConsumed = 0;
                        eCPL.DecConsumed = 0;
                        eCPL.TotalForYear = 0;
                        eCPL.GrandTotal = 0;
                        eCPL.YearRemaining = 0;
                        eCPL.GrandTotalRemaining = 0;
                    }
                    if (entries.Count > 0 && eCL != null && eSL != null && eAL != null)
                    {
                        switch (month)
                        {
                            case 1:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear;
                                UsedCL = (float)eCL.JanConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear;
                                UsedSL = (float)eSL.JanConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear;
                                UsedAL = (float)eAL.JanConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear;
                                UsedCPL = (float)eCPL.JanConsumed;
                                _month = "January";
                                break;
                            case 2:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - (float)eCL.JanConsumed;
                                UsedCL = (float)eCL.FebConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - (float)eSL.JanConsumed;
                                UsedSL = (float)eSL.FebConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - (float)eAL.JanConsumed;
                                UsedAL = (float)eAL.FebConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - (float)eCPL.JanConsumed;
                                UsedCPL = (float)eCPL.FebConsumed;
                                break;
                                _month = "Febu";
                            case 3:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed);
                                UsedCL = (float)eCL.MarchConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed);
                                UsedSL = (float)eSL.MarchConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed);
                                UsedAL = (float)eAL.MarchConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed);
                                UsedAL = (float)eAL.MarchConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed);
                                UsedCPL = (float)eCPL.MarchConsumed;
                                break;
                            case 4:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed);
                                UsedCL = (float)eCL.AprConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed);
                                UsedSL = (float)eSL.AprConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed);
                                UsedAL = (float)eAL.AprConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed);
                                UsedCPL = (float)eCPL.AprConsumed;
                                break;
                            case 5:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed);
                                UsedCL = (float)eCL.MayConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed);
                                UsedSL = (float)eSL.MayConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed);
                                UsedAL = (float)eAL.MayConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed);
                                UsedCPL = (float)eCPL.MayConsumed;
                                break;
                            case 6:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed);
                                UsedCL = (float)eCL.JuneConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed);
                                UsedSL = (float)eSL.JuneConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed);
                                UsedAL = (float)eAL.JuneConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed);
                                UsedCPL = (float)eCPL.JuneConsumed;
                                break;
                            case 7:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed);
                                UsedCL = (float)eCL.JulyConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed);
                                UsedSL = (float)eSL.JulyConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed);
                                UsedAL = (float)eAL.JulyConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed);
                                UsedCPL = (float)eCPL.JulyConsumed;
                                break;
                            case 8:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed);
                                UsedCL = (float)eCL.AugustConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed);
                                UsedSL = (float)eSL.AugustConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed);
                                UsedAL = (float)eAL.AugustConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed + (float)eCPL.JulyConsumed);
                                UsedCPL = (float)eCPL.AugustConsumed;
                                break;
                            case 9:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed);
                                UsedCL = (float)eCL.SepConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed);
                                UsedSL = (float)eSL.SepConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed);
                                UsedAL = (float)eAL.SepConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed + (float)eCPL.JulyConsumed + (float)eCPL.AugustConsumed);
                                UsedCPL = (float)eCPL.SepConsumed;
                                break;
                            case 10:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed);
                                UsedCL = (float)eCL.OctConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed);
                                UsedSL = (float)eSL.OctConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.SepConsumed);
                                UsedAL = (float)eAL.SepConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed + (float)eCPL.JulyConsumed + (float)eCPL.AugustConsumed + (float)eCPL.SepConsumed);
                                UsedCPL = (float)eCPL.SepConsumed;
                                break;
                            case 11:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed + (float)eCL.OctConsumed);
                                UsedCL = (float)eCL.NovConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed + (float)eSL.OctConsumed);
                                UsedSL = (float)eSL.NovConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.SepConsumed + (float)eAL.OctConsumed);
                                UsedAL = (float)eAL.NovConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed + (float)eCPL.JulyConsumed + (float)eCPL.AugustConsumed + (float)eCPL.SepConsumed + (float)eCPL.OctConsumed);
                                UsedCPL = (float)eCPL.NovConsumed;
                                break;
                            case 12:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed + (float)eCL.OctConsumed + (float)eCL.NovConsumed);
                                UsedCL = (float)eCL.DecConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed + (float)eSL.OctConsumed + (float)eSL.NovConsumed);
                                UsedSL = (float)eSL.DecConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.SepConsumed + (float)eAL.OctConsumed + (float)eAL.NovConsumed);
                                UsedAL = (float)eAL.DecConsumed;
                                ////CPL
                                BeforeCPL = (float)eCPL.TotalForYear - ((float)eCPL.JanConsumed + (float)eCPL.FebConsumed + (float)eCPL.MarchConsumed + (float)eCPL.AprConsumed + (float)eCPL.MayConsumed + (float)eCPL.JuneConsumed + (float)eCPL.JulyConsumed + (float)eCPL.AugustConsumed + (float)eAL.SepConsumed + (float)eCPL.OctConsumed + (float)eCPL.NovConsumed);
                                UsedCPL = (float)eCPL.DecConsumed;
                                break;

                        }
                        BalCL = (float)(BeforeCL - UsedCL);
                        BalSL = (float)(BeforeSL - UsedSL);
                        BalAL = (float)(BeforeAL - UsedAL);
                        BalCPL = (float)(BeforeCPL - UsedCPL);
                        AddDataToDT(emp.EmpNo, emp.EmpName, emp.DesignationName, emp.SectionName,
                            emp.DeptName, emp.TypeName, emp.CatName, emp.LocName,
                            BeforeCL, BeforeSL, BeforeAL, BeforeCPL, UsedCL, UsedSL, UsedAL, UsedCPL, BalCL, BalSL, BalAL, BalCPL, _month);
                    }

                }
            }
            return LvSummaryMonth;
        }


        #endregion

        public List<int> GetSecIDs()
        {
            List<int> secid = new List<int>();
            secid.Add(57);
            secid.Add(54);
            secid.Add(2);
            secid.Add(45);
            secid.Add(3);
            secid.Add(55);
            secid.Add(51);
            secid.Add(5);
            secid.Add(99);
            secid.Add(7);
            secid.Add(83);
            secid.Add(19);
            secid.Add(20);
            secid.Add(21);
            secid.Add(22);
            secid.Add(35);
            secid.Add(615);
            secid.Add(23);
            secid.Add(24);
            secid.Add(25);
            secid.Add(26);
            secid.Add(27);
            secid.Add(28);
            secid.Add(29);
            secid.Add(30);
            secid.Add(34);
            secid.Add(31);
            secid.Add(32);
            secid.Add(33);
            secid.Add(53);
            secid.Add(12);
            secid.Add(14);
            secid.Add(16);
            secid.Add(18);
            secid.Add(52);
            secid.Add(13);
            secid.Add(15);
            secid.Add(17);
            secid.Add(58);
            secid.Add(474);
            secid.Add(37);
            secid.Add(60);
            secid.Add(40);
            secid.Add(41);
            secid.Add(42);
            secid.Add(49);
            secid.Add(44);
            secid.Add(43);
            secid.Add(46);
            secid.Add(11);
            secid.Add(633);
            secid.Add(113);
            secid.Add(483);
            secid.Add(50);
            secid.Add(39);
            secid.Add(47);

            secid.Add(1);
            secid.Add(3);
            secid.Add(5);
            secid.Add(6);
            secid.Add(7);
            secid.Add(8);
            secid.Add(9);
            secid.Add(10);
            secid.Add(48);
            secid.Add(51);
            secid.Add(64);
            secid.Add(66);
            secid.Add(97);
            secid.Add(99);
            secid.Add(468);
            secid.Add(489);
            secid.Add(528);
            secid.Add(667);
            return secid;

        }

    }


    #endregion

    #region -- Main Models and Sub Models
    public class ModelJCSummaryData
    {
        public int EmpID { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string DesignationName { get; set; }
        public string GradeName { get; set; }
        public string CompName { get; set; }
        public string CityName { get; set; }
        public string RegionName { get; set; }
        public string CrewName { get; set; }
        public string LocName { get; set; }
        public string SectionName { get; set; }
        public string TypeName { get; set; }
        public string DeptName { get; set; }
        public string CatName { get; set; }
        public string ShiftName { get; set; }
        public string DivisionName { get; set; }
        public short CompanyID { get; set; }
        public byte TypeID { get; set; }
        public short CatID { get; set; }
        public short LocID { get; set; }
        public short CrewID { get; set; }
        public short DeptID { get; set; }
        public byte ShiftID { get; set; }
        public short SecID { get; set; }
        public short DivID { get; set; }
        public int DesigID { get; set; }
        public DateTime? DateofBirth { get; set; }
        //1	Day Off//2	GZ Holiday//3	Absent//4	Official Duty//8	Double Duty//9	Baddli//10	Present//11	Swap Rest Day
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DayOff { get; set; }
        public int Holiday { get; set; }
        public int Absent { get; set; }
        public int OfficialDuty { get; set; }
        public int DoubleDuty { get; set; }
        public int Baddli { get; set; }
        public int Present { get; set; }
        public int SwapRestDay { get; set; }
        public int Total { get; set; }

        public int EditAttCount { get; set; }
    }
    public class ModelEditAttAudit
    {
        public int EmpID { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string DesignationName { get; set; }
        public string GradeName { get; set; }
        public string CompName { get; set; }
        public string CityName { get; set; }
        public string RegionName { get; set; }
        public string CrewName { get; set; }
        public string LocName { get; set; }
        public string SectionName { get; set; }
        public string TypeName { get; set; }
        public string DeptName { get; set; }
        public string CatName { get; set; }
        public string ShiftName { get; set; }
        public string DivisionName { get; set; }
        public short CompanyID { get; set; }
        public byte TypeID { get; set; }
        public short CatID { get; set; }
        public short LocID { get; set; }
        public short CrewID { get; set; }
        public short DeptID { get; set; }
        public byte ShiftID { get; set; }
        public short SecID { get; set; }
        public short DivID { get; set; }
        public int DesigID { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int ChangeDutyCode { get; set; }
        public int ChangeInOut { get; set; }
        public int ChangeShiftTime { get; set; }
        public int TotalRecords { get; set; }
        public string Remarks { get; set; }
    }
    public class ModelAppMonthlyOT
    {
        public int EmpID { get; set; }
        public string Period { get; set; }
        public string EmpMonth { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string DesignationName { get; set; }
        public string GradeName { get; set; }
        public string CompName { get; set; }
        public string CityName { get; set; }
        public string RegionName { get; set; }
        public string CrewName { get; set; }
        public string LocName { get; set; }
        public string SectionName { get; set; }
        public string TypeName { get; set; }
        public string DeptName { get; set; }
        public string CatName { get; set; }
        public string ShiftName { get; set; }
        public string DivisionName { get; set; }
        public short CompanyID { get; set; }
        public byte TypeID { get; set; }
        public short CatID { get; set; }
        public short LocID { get; set; }
        public short CrewID { get; set; }
        public short DeptID { get; set; }
        public byte ShiftID { get; set; }
        public short SecID { get; set; }
        public short DivID { get; set; }
        public int DesigID { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public short GradeID { get; set; }
        public short TAppNOT { get; set; }
        public short TAppGOT { get; set; }
        public short TNOT { get; set; }
        public short TGOT { get; set; }
        public Nullable<short> OT1 { get; set; }
        public Nullable<short> OT2 { get; set; }
        public Nullable<short> OT3 { get; set; }
        public Nullable<short> OT4 { get; set; }
        public Nullable<short> OT5 { get; set; }
        public Nullable<short> OT6 { get; set; }
        public Nullable<short> OT7 { get; set; }
        public Nullable<short> OT8 { get; set; }
        public Nullable<short> OT9 { get; set; }
        public Nullable<short> OT10 { get; set; }
        public Nullable<short> OT11 { get; set; }
        public Nullable<short> OT12 { get; set; }
        public Nullable<short> OT13 { get; set; }
        public Nullable<short> OT14 { get; set; }
        public Nullable<short> OT15 { get; set; }
        public Nullable<short> OT16 { get; set; }
        public Nullable<short> OT17 { get; set; }
        public Nullable<short> OT18 { get; set; }
        public Nullable<short> OT19 { get; set; }
        public Nullable<short> OT20 { get; set; }
        public Nullable<short> OT21 { get; set; }
        public Nullable<short> OT22 { get; set; }
        public Nullable<short> OT23 { get; set; }
        public Nullable<short> OT24 { get; set; }
        public Nullable<short> OT26 { get; set; }
        public Nullable<short> OT25 { get; set; }
        public Nullable<short> OT27 { get; set; }
        public Nullable<short> OT28 { get; set; }
        public Nullable<short> OT29 { get; set; }
        public Nullable<short> OT30 { get; set; }
        public Nullable<short> OT31 { get; set; }
        public Nullable<short> AO1 { get; set; }
        public Nullable<short> AO2 { get; set; }
        public Nullable<short> AO3 { get; set; }
        public Nullable<short> AO4 { get; set; }
        public Nullable<short> AO5 { get; set; }
        public Nullable<short> AO6 { get; set; }
        public Nullable<short> AO7 { get; set; }
        public Nullable<short> AO8 { get; set; }
        public Nullable<short> AO9 { get; set; }
        public Nullable<short> AO11 { get; set; }
        public Nullable<short> AO10 { get; set; }
        public Nullable<short> AO12 { get; set; }
        public Nullable<short> AO13 { get; set; }
        public Nullable<short> AO14 { get; set; }
        public Nullable<short> AO15 { get; set; }
        public Nullable<short> AO16 { get; set; }
        public Nullable<short> AO17 { get; set; }
        public Nullable<short> AO19 { get; set; }
        public Nullable<short> AO18 { get; set; }
        public Nullable<short> AO20 { get; set; }
        public Nullable<short> AO22 { get; set; }
        public Nullable<short> AO21 { get; set; }
        public Nullable<short> AO23 { get; set; }
        public Nullable<short> AO24 { get; set; }
        public Nullable<short> AO25 { get; set; }
        public Nullable<short> AO26 { get; set; }
        public Nullable<short> AO27 { get; set; }
        public Nullable<short> AO28 { get; set; }
        public Nullable<short> AO29 { get; set; }
        public Nullable<short> AO30 { get; set; }
        public Nullable<short> AO31 { get; set; }


    }
    #region --HSE Reports Model--
    public class ModelHSEDataEmp
    {
        public string CompName { get; set; }
        public string LocName { get; set; }
        public string DeptName { get; set; }
        public string EmpName { get; set; }
        public string EmpNo { get; set; }
        public string DesignationName { get; set; }
        public string SecName { get; set; }
        public int Hours { get; set; }
        public int Total { get; set; }
    }
    public class ModelHSEData
    {
        public int DeptID { get; set; }
        public string DeptName { get; set; }
        public int MgmHours { get; set; }
        public int StaffHrs { get; set; }
        public int ContHours { get; set; }
        public int Total { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    #endregion

    #endregion






}