<%@ Page Title="" Language="C#" MasterPageFile="~/ReportingEngine.Master" AutoEventWireup="true" CodeBehind="StepSixFilter.aspx.cs" Inherits="WMS.Reports.Filters.StepSixFilter" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <section class="container" style="margin-left:0;margin-right:0;">
        <div class="col-sm-3 col-md-3 col-lg-3" >
            <!-- Sidebar -->
            <div id="sidebar-wrapper">
                <ul class="sidebar-nav">
                    <li class="sidebar-brand">
                        <h4>Filters Navigation</h4>
                    </li>
                    <li >
                        <a class="inactive-link" href="StepOneFilter.aspx">Step One<p>Company, Locations</p></a>
                    </li>
                    <li>
                        <a class="inactive-link" href="StepTwoFilter.aspx">Step Two<p>Divisions, Shifts</p></a>
                    </li>
                    <li>
                        <a class="inactive-link" href="StepThreeFilter.aspx">Step Three<p>Departments, Employee Type</p></a>
                    </li>
                    <li>
                        <a class="inactive-link" href="StepFourFilter.aspx">Step Four<p>Sections, Crew</p></a>
                    </li>
                    <li>
                        <a class="inactive-link" href="StepFiveFilter.aspx">Step Five<p>Employee</p></a>
                    </li>
                    <%--<div style=" margin-left:40px; margin-top:20px">
                        <asp:Button ID="ButtonSkip" runat="server"  Text="Skip"  CssClass="btn-warning btn-sm btnCustomMargin" OnClick="ButtonSkip_Click" />
                        <asp:Button ID="ButtonNext" runat="server"  Text="Next" CssClass="btn-info btn-sm"  OnClick="ButtonNext_Click" />
                        <asp:Button ID="ButtonFinish" runat="server"  Text="Finish"  CssClass="btn-success btn-sm" OnClick="ButtonFinish_Click" />
                    </div>--%>
                </ul>
                
            <!-- /#sidebar-wrapper -->
        </div>
        </div>
        <div class="col-sm-9 col-md-9 col-lg-9">
                <div class="row">
                    <div class="col-md-8">
                        <section class="row">
                            <h2>Choose Report</h2>
                            <ul>
                                 <% if (((WMS.Models.User)HttpContext.Current.Session["LoggedUser"]).MREmployee == true) %> 
                                <% { %> 
                                <li>
                                    <h5>HR Reports</h5>
                                    <ul>
                                        <li><a href="../ReportContainer.aspx?reportname=emp_record">Employee Record</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=emp_record_active">Active Employees Record</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=emp_record_inactive">Inactive Employees Record</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=emp_detail_excel">Employee Detail (Only for Excel)</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=department_attendance_summary">Department Attendance Summary</a></li>
                                       <%-- <li><a href="../ReportContainer.aspx?reportname=summarized_monthly_report">Summarized Monthly Report</a></li>--%>
                                    </ul>
                                </li>
                                <% }%> 
                                 <% if (((WMS.Models.User)HttpContext.Current.Session["LoggedUser"]).MRDailyAtt == true) %> 
                                <% { %> 
                                <li>
                                    <h5>Daily</h5>
                                    <ul>
                                         <li><a href="../ReportContainer.aspx?reportname=detailed_att">Detailed Attendance</a></li>                                      
                                        <li><a href="../ReportContainer.aspx?reportname=polldata_out">Device Data Out</a></li>
                                         <li><a href="../ReportContainer.aspx?reportname=consolidated_att">Consolidated Attendance</a></li>
                                         <li><a href="../ReportContainer.aspx?reportname=present">Present</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=presentFather">Present (With Father's Name)</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=absent">Absent</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=lv_application">Leave Application</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=lv_applicationCPL">Leave Application (CPL)</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=lv_applicationLWOP">Leave Application (LWOP)</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=short_lv">Short Leave</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=late_in">Late In</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=late_out">Late Out</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=early_in">Early In</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=early_out">Early Out</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=overtime">Overtime</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=missing_attendance">Missing Attendance</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=multiple_in_out">Multiple In/Out</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=badli_report">Badli Report</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=polldata_in">Device Data In</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=polldata_out">Device Data Out</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=da_sheet">Daily Attendance Sheet</a></li>
                                         <li><a href="../ReportContainer.aspx?reportname=Approved_Overtime">Approved Overtime</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=Emp_JobCards">Job Cards Application</a></li>
                                         <li><a href="../ReportContainer.aspx?reportname=mrSum_DOJ">New Joining Employees Reports</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=mrSum_DOR">Resign Employees Reports</a></li>
                                         <li><a href="../ReportContainer.aspx?reportname=mrJCSummary">Job Card Summary</a></li>
                                         <li><a href="../ReportContainer.aspx?reportname=Emp_JobCardsSummary">Job Card Detail</a></li>
                                         <li><a href="../ReportContainer.aspx?reportname=NegativeWorkMin">Negative Work Minutes</a></li>
                                        <%--<li><a href="../ReportContainer.aspx?reportname=editor_ot">Editor Overtime</a></li>--%>
                                    </ul>
                                    
                                </li>
                                 <% }%>
                                
                                
                                 <% if (((WMS.Models.User)HttpContext.Current.Session["LoggedUser"]).MRAudit==true) %> 
                                       
                                    <% { %> 
                                <li>
                                <h5>Audit Reports </h5>
                                    <ul>
                                         <li><a href='../ReportContainer.aspx?reportname=emp_audit'>Audit Report</a></li>
                                         <li><a href='../ReportContainer.aspx?reportname=monthly_1-31_conBadliIE'>Monthly Consolidated with Badli Days(Inactive Employees)</a></li>
                                        <li><a href='../ReportContainer.aspx?reportname=monthly_1-31_consolidatedIE'>Monthly Consolidated(Inactive Employees)</a></li>
                                       <%-- //<li><a href='../ReportContainer.aspx?reportname=monthly_1-31_consolidated_AppOTIE'>Monthly Consolidated with Approve OT(Inactive Employees)</a></li>"
                        --%>          </ul>
                                </li>
                                  <% }%> 

                                    
                                
                                <% if (((WMS.Models.User)HttpContext.Current.Session["LoggedUser"]).MHSEReport==true) %> 
                                <% { %>  
                                <li>
                                <h5>Safety Head Count Reports</h5>
                                    <ul>
                                        <li><a href="../ReportContainer.aspx?reportname=HSEHour">HSE By Department (Daily)</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=HSEHour_con">Plant: HSE By Employee (Consolidated)</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=HSEHourEmp">Plant: HSE By Employee</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=THSEHour">Township: HSE By Department (Daily)</a></li>
                                         <li><a href="../ReportContainer.aspx?reportname=THSEHour_con">Township: HSE By Employee (Consolidated)</a></li>
                                    </ul>
                                </li>
                                  <% }%> 

                                       <% if (((WMS.Models.User)HttpContext.Current.Session["LoggedUser"]).MRDailyAtt == true) %> 
                                <% { %> 
                                
                                
                                
                                <li>
                                    <h5>Monthly Attendance Reports (21st to 20th)</h5>
                                    <ul>
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_21-20">Monthly Sheet (21th to 20th)</a></li>
                                        
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_21-20_excel">Monthly Sheet (21th to 20th)(Excel)</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=monthlysummary_21-20">Monthly Summary (21th to 20th)</a></li>                       <li><a href="../ReportContainer.aspx?reportname=monthly_21-20_consolidated">Monthly Consolidated (21th to 20th)(Excel)</a></li>
                                         <li><a href="../ReportContainer.aspx?reportname=monthly_21-20_flexy">Monthly Flexy (21th to 20th)(Excel)</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_productivity">Monthly Productivity</a></li>
                                    </ul>
                                </li>
                                <li>
                                    <h5>Monthly Attendance Reports (26th to 25th)</h5>
                                    <ul>
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_26-25_consolidated">Monthly Sheet Consolidated (26th to 25th)</a></li>                                       
                                       </ul>
                                </li>
                                <% }%> 
                                 <% if (((WMS.Models.User)HttpContext.Current.Session["LoggedUser"]).MRMonthly == true) %> 
                                <% { %> 
                                <li>
                                    <h5>Monthly Attendance Reports (1st to 30th)</h5>
                                    <ul>
                                        <li><a href="../ReportContainer.aspx?reportname=monthlysummary_1-31">Monthly Summary</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_1-31">Monthly Sheet</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_1-31_consolidated">Monthly Consolidated </a><a href="../ReportContainer.aspx?reportname=monthlyConDownload">: Click here to Download in Excel</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_1-31_consolidated_AppOT">Monthly Consolidated With Approve OT</a><a href="../ReportContainer.aspx?reportname=monthlyAppOTConDownload">: Click here to Download in Excel</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_1-31_conBadli">Monthly Consolidated with Badli Days</a><a href="../ReportContainer.aspx?reportname=monthlyBadliConDownload">: Click here to Download in Excel</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=MonthlySheetAbandLv">Monthly Summary Absents and Leaves Only</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=MonthlySheetIncompleteOnly">Monthly Summary Missing Only</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_1-31_overtime">Monthly Overtime Only</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=badli_month">Badli Monthly Report</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=FatimaPayroll_Report">Fatima payroll Report</a></li>
                                        
                                    </ul>
                                </li>
                                 <% }%> 
                                <% if (((WMS.Models.User)HttpContext.Current.Session["LoggedUser"]).MRLeave == true) %> 
                                <% { %> 
                                <li>
                                    <h5>Monthly Leave Sheet</h5>
                                    <ul>
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_leave_sheet">Monthly Leave Sheet</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=monthly_leave_sheetCPL">Monthly Leave Sheet with CPL</a></li></ul>
                                </li>
                                 <% }%>
                                <% if (((WMS.Models.User)HttpContext.Current.Session["LoggedUser"]).MReaderReport == true) %> 
                                <% { %> 
                                <li>
                                    <h5>Device Data</h5>
                                    <ul>
                                        <li><a href="../ReportContainer.aspx?reportname=device_in_out">Device Wise In/Out Report</a></li>
                                        </ul>
                                </li>
                                 <% }%>
                                <% if (((WMS.Models.User)HttpContext.Current.Session["LoggedUser"]).MRDetail == true) %> 
                                <% { %> 
                                <li>
                                    <h5>Detailed</h5>
                                    <ul>
                                        <li><a href="../ReportContainer.aspx?reportname=emp_att">Employee Attendance</a></li>
                                          <li><a href="../ReportContainer.aspx?reportname=emp_att_old">Employee Attendance (Old)</a></li>
                                        <li><a href="../ReportContainer.aspx?reportname=emp_attTime">Employee Attendance (Time Difference)</a></li>
                                        <%--<li><a href="../ReportContainer.aspx?reportname=emp_absent">Employee Absent</a></li>--%>
                                        <li><a href="../ReportContainer.aspx?reportname=Employee_Att_Summary_New_report">Employee AttSummary (Page Break)</a></li>
                                    </ul>
                                    </li>
                                 <% }%>
                                <% if (((WMS.Models.User)HttpContext.Current.Session["LoggedUser"]).MRLeave == true) %> 
                                <% { %>  
                                <li>
                                    <h5>Yearly</h5>
                                    <ul>
                                        <li><a href="../ReportContainer.aspx?reportname=lv_quota">Leave Quota</a></li>
                                        <%--<li><a href="../ReportContainer.aspx?reportname=edit_Attendance">Edit Attendance</a></li>--%>
                                        <%--<li><a href="../SummaryReports.aspx?reportname=">Summary</a></li>--%>
                                    </ul>
                                </li>
                                 <% }%> 
                            </ul>
                        </section>
                    </div>
                    <section class="col-md-4 selected-filters-wrapper">
                    <h2>Selected Filters...</h2><hr />
                    <div class="panel-group" id="accordion">

	

    
                    <% if (((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).CompanyFilter.Count > 0)
                       {
                           {
                               int d = ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).CompanyFilter.Count;
                               Response.Write("<div class='panel panel-default'><div class='panel-heading'><h4 class='panel-title'><a style = 'text-decoration: none !important;' data-toggle='collapse' data-parent='#accordion' href='#collapseOne'>Companies</a>  <span style ='float:right;' class='badge' id='CompanySpan'>" + d + "</span></h4></div><div id='collapseOne' class='panel-collapse collapse out'><div class='list-group'>");
                           }
                           foreach (var item in ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).CompanyFilter)
                           {
                               { Response.Write("<a class='list-group-item' id='Company'>" + item.FilterName + "<button type='button' id='" + item.ID + "' onclick = 'deleteFromFilters(this)' class='btn btn-danger btn-sm' style='float:right;'>[X]</button></a> "); }
                           }
                           { Response.Write("</div></div></div>"); }
                       }%>
                       
                   </div>
                          <div class="panel-group" id="Div1">

                         <% if (((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).LocationFilter.Count > 0)
                       {
                           {
                               int d = ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).LocationFilter.Count;
                               Response.Write("<div class='panel panel-default'><div class='panel-heading'><h4 class='panel-title'><a style = 'text-decoration: none !important;' data-toggle='collapse' data-parent='#Div1' href='#collapseShift'>Locations</a>  <span style ='float:right;' class='badge' id ='LocationSpan'>" + d + "</span></h4></div><div id='collapseShift' class='panel-collapse collapse out'><div class='list-group'>");
                           }
                           foreach (var item in ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).LocationFilter)
                           {
                               { Response.Write("<a class='list-group-item' id ='Location'>" + item.FilterName + "<button type='button' id='" + item.ID + "' onclick = 'deleteFromFilters(this)' class='btn btn-danger btn-sm' style='float:right;'>[X]</button></a>"); }
                           }
                           { Response.Write("</div></div></div>"); }
                    }%>
                               </div>
                       
                        <div class="panel-group" id="Div2">
                    <% if (((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).DivisionFilter.Count > 0)
                       {
                           {
                               int d = ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).DivisionFilter.Count;
                               Response.Write("<div class='panel panel-default'><div class='panel-heading'><h4 class='panel-title'><a style = 'text-decoration: none !important;' data-toggle='collapse' data-parent='#Div2' href='#collapseCity'>Divisions</a>  <span style ='float:right;' class='badge' id ='DivisionSpan'>" + d + "</span></h4></div><div id='collapseCity' class='panel-collapse collapse out'><div class='list-group'>");
                           }
                           foreach (var item in ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).DivisionFilter)
                           {
                               { Response.Write("<a class='list-group-item' id='Division'>" + item.FilterName + "<button type='button' id='" + item.ID + "' onclick = 'deleteFromFilters(this)' class='btn btn-danger btn-sm' style='float:right;'>[X]</button></a>"); }
                           }
                           { Response.Write("</div></div></div>"); }
                    }%> </div>
                       
                       <div class="panel-group" id="Div3">
                         <% if (((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).ShiftFilter.Count > 0)
                       {
                           {
                               int d = ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).ShiftFilter.Count;
                               Response.Write("<div class='panel panel-default'><div class='panel-heading'><h4 class='panel-title'><a style = 'text-decoration: none !important;' data-toggle='collapse' data-parent='#Div3' href='#collapseType'>Shifts</a>  <span style ='float:right;' class='badge' id ='ShiftSpan'>" + d + "</span></h4></div><div id='collapseType' class='panel-collapse collapse out'><div class='list-group'>");
                           }
                           foreach (var item in ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).ShiftFilter)
                           {
                               { Response.Write("<a class='list-group-item' id='Shift'>" + item.FilterName + "<button type='button' id='" + item.ID + "' onclick = 'deleteFromFilters(this)' class='btn btn-danger btn-sm' style='float:right;'>[X]</button></a>"); }
                           } 
                                { Response.Write("</div></div></div>"); }
                    }%>

                       </div>
                         <div class="panel-group" id="Div4">
                         <% if (((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).DepartmentFilter.Count > 0)
                       {
                           {
                               int d = ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).DepartmentFilter.Count;
                               Response.Write("<div class='panel panel-default'><div class='panel-heading'><h4 class='panel-title'><a style = 'text-decoration: none !important;'  data-toggle='collapse' data-parent='#Div4' href='#collapseLocation'>Departments<span  style ='float:right;' class='badge' id ='DepartmentSpan'>" + d + "</span></a></h4></div><div id='collapseLocation' class='panel-collapse collapse out'><div class='list-group'>");
                           }
                           foreach (var item in ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).DepartmentFilter)
                           {
                               { Response.Write("<a class='list-group-item' id='Department'>" + item.FilterName + "<button type='button' id='" + item.ID + "' onclick = 'deleteFromFilters(this)' class='btn btn-danger btn-sm' style='float:right;'>[X]</button></a>"); }
                           }
                           { Response.Write("</div></div></div>"); }
                    }%>
                             </div>
                         <div class="panel-group" id="Div5">
                         <% if (((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).TypeFilter.Count > 0)
                       {
                           {
                               int d = ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).TypeFilter.Count;
                               Response.Write("<div class='panel panel-default'><div class='panel-heading'><h4 class='panel-title'><a data-toggle='collapse' data-parent='#Div5' href='#collapseWing'>Employee Type</a><span style ='float:right;' class='badge' id ='TypeSpan'>" + d + "</span></h4></div><div id='collapseWing' class='panel-collapse collapse out'><div class='list-group'>");
                           }
                           foreach (var item in ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).TypeFilter)
                           {
                               { Response.Write("<a class='list-group-item' id='Type'>" + item.FilterName + "<button type='button' id='" + item.ID + "' onclick = 'deleteFromFilters(this)' class='btn btn-danger btn-sm' style='float:right;'>[X]</button></a>"); }
                           }
                           { Response.Write("</div></div></div>"); }
                    }%>

                         </div>

                         <div class="panel-group" id="Div6">
                        <% if (((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).CrewFilter.Count > 0)
                       {
                           {
                               int d = ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).CrewFilter.Count;
                               Response.Write("<div class='panel panel-default'><div class='panel-heading'><h4 class='panel-title'><a data-toggle='collapse' data-parent='#Div6' href='#collapseCrew'>Crews</a><span style ='float:right;' class='badge' id ='CrewSpan'>" + d + "</span></h4></div><div id='collapseCrew' class='panel-collapse collapse out'><div class='list-group'>");
                           }
                           foreach (var item in ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).CrewFilter)
                           {
                               { Response.Write("<a class='list-group-item' id='Crew'>" + item.FilterName + "<button type='button' id='" + item.ID + "' onclick = 'deleteFromFilters(this)' class='btn btn-danger btn-sm' style='float:right;'>[X]</button></a>"); }
                           }   { Response.Write("</div></div></div>"); }
                    }%>

                         </div>
                         <% if (((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).SectionFilter.Count > 0)
                       {
                           {
                               int d = ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).SectionFilter.Count;
                               Response.Write("<div class='panel panel-default'><div class='panel-heading'><h4 class='panel-title'><a data-toggle='collapse' data-parent='#Div7' href='#collapseSection'>Sections</a><span style ='float:right;' class='badge' id ='SectionSpan'>" + d + "</span></h4></div><div id='collapseSection' class='panel-collapse collapse out'><div class='list-group'>");
                           }
                           foreach (var item in ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).SectionFilter)
                           {
                               { Response.Write("<a class='list-group-item' id='Section'>" + item.FilterName + "<button type='button' id='" + item.ID + "' onclick = 'deleteFromFilters(this)' class='btn btn-danger btn-sm' style='float:right;'>[X]</button></a>"); }
                           }  { Response.Write("</div></div></div><div>"); }
                    }%>
                        <% if (((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).EmployeeFilter.Count > 0)
                       {
                           {
                               int d = ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).EmployeeFilter.Count;
                               Response.Write("<div class='panel panel-default'><div class='panel-heading'><h4 class='panel-title'><a data-toggle='collapse' data-parent='#Div7' href='#collapseEmployee'>Employees</a><span style ='float:right;' class='badge' id ='EmployeeSpan'>" + d + "</span></h4></div><div id='collapseEmployee' class='panel-collapse collapse out'><div class='list-group'>");
                           }
                           foreach (var item in ((WMSLibrary.FiltersModel)HttpContext.Current.Session["FiltersModel"]).EmployeeFilter)
                           {
                               { Response.Write("<a class='list-group-item' id='Employee'>" + item.FilterName + "<button type='button' id='" + item.ID + "' onclick = 'deleteFromFilters(this)' class='btn btn-danger btn-sm' style='float:right;'>[X]</button></a>"); }
                           }  { Response.Write("</div></div></div><div>"); }
                    }%>

                </section>
                </div>
                <div class="row">
                    
                </div>
        </div>
    </section>
       <script src="../../Scripts/Filters/DeleteSingleFilters.js"></script>
</asp:Content>
