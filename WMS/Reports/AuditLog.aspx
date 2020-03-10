<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AuditLog.aspx.cs" Inherits="WMS.fonts.Reports.AuditLog" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script src="../Scripts/modernizr-2.6.2.js"></script>
    <script src="../Scripts/jquery-2.1.1.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="report-container">
         <div class="report-filter">
             <div style="height:20px;"></div>
             <div class="button-divDate">
                 <span style="color:whitesmoke; font-size:15px;">From: </span><input id="dateFrom" type="date" runat="server" style="height:30px;" />
                 <span style="color:whitesmoke; font-size:15px;">To: </span><input id="dateTo" type="date" runat="server" style="height:30px;" />
             </div>


             <div class="PanelFilters">
                <asp:CheckBox ID="ChkOperation" runat="server" Text="Operations" ForeColor="White" Width="150px" />
                <asp:DropDownList ID="cboOperations" runat="server" Width="150px" >
                </asp:DropDownList>
            </div>
             <div class="PanelFilters">
                <asp:CheckBox ID="ChkForms" runat="server" Text="Forms" ForeColor="White" Width="150px" />
                <asp:DropDownList ID="cboForms" runat="server" Width="150px" >
                </asp:DropDownList>
            </div>
             <div class="PanelFilters">
                <asp:CheckBox ID="ChkUsers" runat="server" Text="Users" ForeColor="White" Width="150px" />
                <asp:DropDownList ID="cboUsers" runat="server" Width="150px" >
                </asp:DropDownList>
            </div>

            <div class="button-div">
                <asp:Button ID="btnGenerateReport" CssClass="btn btn-success" 
                    runat="server" Text="Generate Report" onclick="btnGenerateReport_Click" Width="190px" />
            </div>

            <div class="button-div"></div>

         </div> <%--End div Report-filter--%>

         <div class="report-viewer-container" style="margin: 0 auto;">
            <div>
                 <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="719px">
                     <localreport reportpath="Reports\RDLC\RAuditLog.rdlc">
                         <datasources>
                             <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet1" />
                         </datasources>
                     </localreport>
                 </rsweb:ReportViewer>
                 <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" TypeName="WMS.Models.TASReportDataSetTableAdapters.ViewAuditLogTableAdapter"></asp:ObjectDataSource>
                 <asp:ScriptManager ID="ScriptManager1" runat="server">
                 </asp:ScriptManager>
             </div>
         </div> <%--End div Report-viewer-container--%>
         <div class="clearfix">
            
         </div> 
    </div><%--End div Report-container--%>
</asp:Content>

