using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using WMS.HelperClass;
using WMS.Models;
using WMS.Reports;

namespace WMS.fonts.Reports
{
    public partial class AuditLog : System.Web.UI.Page
    {
        string PathString = "";
        TAS2013Entities context = new TAS2013Entities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                User _user = HttpContext.Current.Session["LoggedUser"] as User;
                if (MyHelper.CheckforPermission(_user, MyHelper.ReportName.Audit))
                {
                    DateTime dt = DateTime.Today.Date.AddDays(-1);
                    if (GlobalVariables.DeploymentType == false)
                    {
                        PathString = "/Reports/RDLC/RAuditLog.rdlc";
                    }
                    else
                        PathString = "/WMS/Reports/RDLC/RAuditLog.rdlc";
                    LoadCBOOperations();
                    LoadCBOForms();
                    LoadCBOUsers();
                    LoadReport(PathString, context.ViewAuditLogs.Where(aa => aa.AuditDateTime == dt && aa.Status == true).ToList());
                }
                else
                    Response.Redirect("~/");
            }
        }
        private void LoadReport(string path, List<ViewAuditLog> _Employee)
        {
            string DateToFor = "";
            if (DateFrom.Date.ToString("d") == DateTo.ToString("d"))
            {
                DateToFor = "Date : " + DateFrom.Date.ToString("d");
            }
            else
            {
                DateToFor = "From : " + DateFrom.Date.ToString("d") + " To: " + DateTo.Date.ToString("d");
            }

            string _Header = "Audit Log";
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewAuditLog> ie;
            ie = _Employee.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportParameter rp = new ReportParameter("Date", DateToFor, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        public DateTime DateFrom
        {
            get
            {
                if (dateFrom.Value == "")
                    return DateTime.Today.Date.AddDays(-1);
                else
                    return DateTime.Parse(dateFrom.Value);
            }
        }

        public DateTime DateTo
        {
            get
            {
                if (dateTo.Value == "")
                    return DateTime.Today.Date;
                else
                    return DateTime.Parse(dateTo.Value);
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            List<ViewAuditLog> _AuditLog = new List<ViewAuditLog>();
            _AuditLog = context.ViewAuditLogs.Where(aa => aa.AuditDateTime >= DateFrom && aa.AuditDateTime <= DateTo).ToList();
            if (CBUser())
            {
                _AuditLog = _AuditLog.Where(aa => aa.Name == UserName).ToList();
            }
            if (CBOpe())
            {
                _AuditLog = _AuditLog.Where(aa => aa.Name == OperationName).ToList();
            }
            if (CBForm())
            {
                _AuditLog = _AuditLog.Where(aa => aa.Name == FormName).ToList();
            }

            if (GlobalVariables.DeploymentType == false)
            {
                PathString = "/Reports/RDLC/RAuditLog.rdlc";
            }
            else
                PathString = "/WMS/Reports/RDLC/RAuditLog.rdlc";
            LoadReport(PathString, _AuditLog);

        }

        private void LoadCBOOperations()
        {
            cboOperations.DataSource = context.AuditOperations.ToList();
            cboOperations.DataTextField = "OperationName";
            cboOperations.DataValueField = "OperationID";
            cboOperations.DataBind();
        }
        private void LoadCBOForms()
        {
            cboForms.DataSource = context.AuditForms.ToList();
            cboForms.DataTextField = "FormName";
            cboForms.DataValueField = "FormID";
            cboForms.DataBind();
        }
        private void LoadCBOUsers()
        {
            cboUsers.DataSource = context.Users.ToList();
            cboUsers.DataTextField = "Name";
            cboUsers.DataValueField = "UserID";
            cboUsers.DataBind();
        }

        public string OperationName
        {
            get { return cboOperations.SelectedItem.Text; }
            set { }
        }
        public string FormName
        {
            get { return cboForms.SelectedItem.Text; }
            set { }
        }
        public string UserName
        {
            get { return cboUsers.SelectedItem.Text; }
            set { }
        }

        public bool CBUser()
        {
            if (ChkUsers.Checked == true)
                return true;
            else
                return false;
        }
        public bool CBForm()
        {
            if (ChkForms.Checked == true)
                return true;
            else
                return false;
        }
        public bool CBOpe()
        {
            if (ChkOperation.Checked == true)
                return true;
            else
                return false;
        }

    }
}