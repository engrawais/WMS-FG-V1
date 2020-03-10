using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using WMS.CustomClass;
using WMS.Models;

namespace WMS.Reports
{
    public partial class MYLeaveSummary : System.Web.UI.Page
    {
        string PathString = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DivGridSection.Visible = false;
                DivGridCrew.Visible = false;
                DivGridDept.Visible = false;
                DivGridEmp.Visible = false;
                DivShiftGrid.Visible = false;
                DivLocGrid.Visible = false;
                DivTypeGrid.Visible = false;
                ReportViewer1.Visible = true;
                ReportViewer1.Width = 1050;
                ReportViewer1.Height = 700;
                SelectedTypes.Clear();
                SelectedCrews.Clear();
                SelectedDepts.Clear();
                SelectedEmps.Clear();
                SelectedLocs.Clear();
                SelectedSections.Clear();
                SelectedShifts.Clear();
                DivGridComapny.Visible = false;
                SelectedComps.Clear();
                RefreshLabels();
                LoadGridViews();
                CreateDataTable();
                if (GlobalVariables.DeploymentType == false)
                {
                    PathString = "/Reports/RDLC/MYLeaveSummary.rdlc";
                }
                else
                    PathString = "/WMS/Reports/RDLC/MYLeaveSummary.rdlc";
                User LoggedInUser = HttpContext.Current.Session["LoggedUser"] as User;
                QueryBuilder qb = new QueryBuilder();
                string query = qb.MakeCustomizeQuery(LoggedInUser);
                DataTable dt = qb.GetValuesfromDB("select * from EmpView " + query);
                //List<ViewYLSummary> _ViewList = dt.ToList<ViewYLSummary>();
                List<EmpView> _ViewList = dt.ToList<EmpView>();
                LoadReport(PathString, GYL(_ViewList));
            }
        }
        #region --Load GridViews --
        private void LoadGridViews()
        {
            User _loggedUser = HttpContext.Current.Session["LoggedUser"] as User;
            LoadEmpTypeGrid(_loggedUser);
            LoadLocationGrid(_loggedUser);
            LoadShiftGrid(_loggedUser);
            LoadEmpGrid(_loggedUser);
            LoadSectionGrid(_loggedUser);
            LoadDeptGrid(_loggedUser);
            LoadCrewGrid(_loggedUser);
            LoadCompanyGrid();
        }

        private void LoadEmpTypeGrid(User _loggedUser)
        {
            QueryBuilder qb = new QueryBuilder();
            string query = qb.QueryForCompanySegeration(_loggedUser);
            DataTable dt = qb.GetValuesfromDB("select * from ViewEmpType " + query);
            List<ViewEmpType> _View = dt.ToList<ViewEmpType>();
            grid_EmpType.DataSource = _View;
            grid_EmpType.DataBind();
        }

        private void LoadLocationGrid(User _loggedUser)
        {
            List<Location> _objectList = new List<Location>();
            _objectList = context.Locations.ToList();
            //_Query = "SELECT * FROM TAS2013.dbo.EmpType where " + selectSQL;
            //grid_EmpType.DataSource = GetValuesFromDatabase(_Query, "EmpType");
            //grid_EmpType.DataBind();
            grid_Location.DataSource = _objectList;
            grid_Location.DataBind();
        }
        private void LoadCompanyGrid()
        {
            List<Company> _objectList = new List<Company>();
            _objectList = context.Companies.ToList();
            //_Query = "SELECT * FROM TAS2013.dbo.EmpType where " + selectSQL;
            //grid_EmpType.DataSource = GetValuesFromDatabase(_Query, "EmpType");
            //grid_EmpType.DataBind();
            grid_Company.DataSource = _objectList;
            grid_Company.DataBind();
        }

        private void LoadShiftGrid(User _loggedUser)
        {
            List<Shift> _objectList = new List<Shift>();
            _objectList = context.Shifts.Where(aa => aa.CompanyID == _loggedUser.CompanyID).ToList();
            //_Query = "SELECT * FROM TAS2013.dbo.EmpType where " + selectSQL;
            //grid_EmpType.DataSource = GetValuesFromDatabase(_Query, "EmpType");
            //grid_EmpType.DataBind();
            grid_Shift.DataSource = _objectList;
            grid_Shift.DataBind();
        }

        private void LoadEmpGrid(User _loggedUser)
        {
            QueryBuilder qb = new QueryBuilder();
            string query = qb.MakeCustomizeQuery(_loggedUser);
            DataTable dt = qb.GetValuesfromDB("select * from EmpView " + query + " and (Status=1)");
            List<EmpView> _View = dt.ToList<EmpView>();
            grid_Employee.DataSource = _View;
            grid_Employee.DataBind();


        }

        private void LoadSectionGrid(User _loggedUser)
        {
            QueryBuilder qb = new QueryBuilder();
            string query = qb.QueryForCompanySegeration(_loggedUser);
            DataTable dt = qb.GetValuesfromDB("select * from ViewSection " + query);
            List<ViewSection> _View = dt.ToList<ViewSection>();
            grid_Section.DataSource = _View;
            grid_Section.DataBind();
        }

        private void LoadDeptGrid(User _loggedUser)
        {
            QueryBuilder qb = new QueryBuilder();
            string query = qb.QueryForCompanySegeration(_loggedUser);
            DataTable dt = qb.GetValuesfromDB("select * from ViewDepartment " + query);
            List<ViewDepartment> _View = dt.ToList<ViewDepartment>();
            grid_Dept.DataSource = _View;
            grid_Dept.DataBind();
        }

        private void LoadCrewGrid(User _loggedUser)
        {
            QueryBuilder qb = new QueryBuilder();
            string query = qb.QueryForCompanySegeration(_loggedUser);
            DataTable dt = qb.GetValuesfromDB("select * from ViewCrew " + query);
            List<ViewCrew> _View = dt.ToList<ViewCrew>();
            grid_Crew.DataSource = _View;
            grid_Crew.DataBind();
        }

        //private DataSet GetValuesFromDatabase(string _query, string _tableName)
        //{
        //    string connectionString = WebConfigurationManager.ConnectionStrings["TAS2013ConnectionString"].ConnectionString;
        //    SqlConnection con = new SqlConnection(connectionString);
        //    SqlCommand cmd = new SqlCommand(_query, con);
        //    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();

        //    adapter.Fill(ds, _tableName);
        //    return ds;
        //}

        #endregion
        protected void grid_Employee_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gridView = (GridView)sender;

            if (gridView.SortExpression.Length > 0)
            {
                int cellIndex = -1;
                //  find the column index for the sorresponding sort expression
                foreach (DataControlField field in gridView.Columns)
                {
                    if (field.SortExpression == gridView.SortExpression)
                    {
                        cellIndex = gridView.Columns.IndexOf(field);
                        break;
                    }
                }

                if (cellIndex > -1)
                {
                    if (e.Row.RowType == DataControlRowType.Header)
                    {
                        //  this is a header row,
                        //  set the sort style
                        e.Row.Cells[cellIndex].CssClass +=
                            (gridView.SortDirection == SortDirection.Ascending
                            ? " sortascheader" : " sortdescheader");
                    }
                    else if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        //  this is a data row
                        e.Row.Cells[cellIndex].CssClass +=
                            (e.Row.RowIndex % 2 == 0
                            ? " sortaltrow" : "sortrow");
                    }
                }
            }
        }
        #region --Filters Button--
        protected void btnEmployeeGrid_Click(object sender, EventArgs e)
        {
            DivGridSection.Visible = false;
            DivGridCrew.Visible = false;
            DivGridDept.Visible = false;
            DivShiftGrid.Visible = false;
            DivLocGrid.Visible = false;
            DivTypeGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivGridEmp.Visible = true;
            RefreshLabels();
            DivGridComapny.Visible = false;
        }

        protected void btnSectionGrid_Click(object sender, EventArgs e)
        {
            DivGridCrew.Visible = false;
            DivGridDept.Visible = false;
            DivGridEmp.Visible = false;
            DivShiftGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivLocGrid.Visible = false;
            DivTypeGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivGridSection.Visible = true;
            DivGridComapny.Visible = false;
            RefreshLabels();
        }

        protected void btnDepartmentGrid_Click(object sender, EventArgs e)
        {
            DivGridSection.Visible = false;
            DivGridCrew.Visible = false;
            DivGridEmp.Visible = false;
            DivShiftGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivLocGrid.Visible = false;
            DivTypeGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivGridDept.Visible = true;
            DivGridComapny.Visible = false;
            RefreshLabels();
        }

        protected void btnCrewGrid_Click(object sender, EventArgs e)
        {
            DivGridSection.Visible = false;
            DivGridDept.Visible = false;
            DivGridEmp.Visible = false;
            DivShiftGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivLocGrid.Visible = false;
            DivTypeGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivGridCrew.Visible = true;
            DivGridComapny.Visible = false;
            RefreshLabels();
        }

        protected void btnShiftGrid_Click(object sender, EventArgs e)
        {
            DivGridSection.Visible = false;
            DivGridCrew.Visible = false;
            DivGridDept.Visible = false;
            DivGridEmp.Visible = false;
            ReportViewer1.Visible = false;
            DivLocGrid.Visible = false;
            DivTypeGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivShiftGrid.Visible = true;
            DivGridComapny.Visible = false;
            RefreshLabels();
        }
        protected void btnLoc_Click(object sender, EventArgs e)
        {
            DivGridSection.Visible = false;
            DivGridCrew.Visible = false;
            DivGridDept.Visible = false;
            DivGridEmp.Visible = false;
            ReportViewer1.Visible = false;
            DivShiftGrid.Visible = false;
            DivTypeGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivLocGrid.Visible = true;
            DivGridComapny.Visible = false;
            RefreshLabels();
        }
        protected void btnEmpType_Click(object sender, EventArgs e)
        {
            DivGridSection.Visible = false;
            DivGridCrew.Visible = false;
            DivGridDept.Visible = false;
            DivGridEmp.Visible = false;
            ReportViewer1.Visible = false;
            DivShiftGrid.Visible = false;
            DivLocGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivTypeGrid.Visible = true;
            DivGridComapny.Visible = false;
            RefreshLabels();
        }
        protected void btnCompanyGrid_Click(object sender, EventArgs e)
        {
            DivGridSection.Visible = false;
            DivGridCrew.Visible = false;
            DivGridDept.Visible = false;
            DivGridEmp.Visible = false;
            ReportViewer1.Visible = false;
            DivShiftGrid.Visible = false;
            DivLocGrid.Visible = false;
            ReportViewer1.Visible = false;
            DivTypeGrid.Visible = false;
            DivGridComapny.Visible = true;
            RefreshLabels();
        }
        #endregion

        #region --ViewStates--

        protected List<Section> SelectedSections
        {
            get
            {
                if (this.Session["SelectedSection"] == null)
                    this.Session["SelectedSection"] = new List<Section>();
                return (List<Section>)this.Session["SelectedSection"];
            }
            set
            {
                Session["SelectedSection"] = value;
            }
        }

        protected List<Department> SelectedDepts
        {
            get
            {
                if (this.Session["SelectedDept"] == null)
                    this.Session["SelectedDept"] = new List<Department>();
                return (List<Department>)this.Session["SelectedDept"];
            }
            set
            {
                Session["SelectedDept"] = value;
            }
        }

        protected List<Shift> SelectedShifts
        {
            get
            {
                if (this.Session["SelectedShifts"] == null)
                    this.Session["SelectedShifts"] = new List<Shift>();
                return (List<Shift>)this.Session["SelectedShifts"];
            }
            set
            {
                Session["SelectedSection"] = value;
            }
        }

        protected List<Crew> SelectedCrews
        {
            get
            {
                if (this.Session["SelectedCrew"] == null)
                    this.Session["SelectedCrew"] = new List<Crew>();
                return (List<Crew>)this.Session["SelectedCrew"];
            }
            set
            {
                Session["SelectedCrew"] = value;
            }
        }

        protected List<EmpView> SelectedEmps
        {
            get
            {
                if (this.Session["SelectedEmp"] == null)
                    this.Session["SelectedEmp"] = new List<EmpView>();
                return (List<EmpView>)this.Session["SelectedEmp"];
            }
            set
            {
                Session["SelectedEmp"] = value;
            }
        }

        protected List<Location> SelectedLocs
        {
            get
            {
                if (this.Session["SelectedLoc"] == null)
                    this.Session["SelectedLoc"] = new List<Location>();
                return (List<Location>)this.Session["SelectedLoc"];
            }
            set
            {
                Session["SelectedLoc"] = value;
            }
        }
        protected List<Company> SelectedComps
        {
            get
            {
                if (this.Session["SelectedComp"] == null)
                    this.Session["SelectedComp"] = new List<Company>();
                return (List<Company>)this.Session["SelectedComp"];
            }
            set
            {
                Session["SelectedComp"] = value;
            }
        }
        protected List<EmpType> SelectedTypes
        {
            get
            {
                if (this.Session["SelectedCat"] == null)
                    this.Session["SelectedCat"] = new List<EmpType>();
                return (List<EmpType>)this.Session["SelectedCat"];
            }
            set
            {
                Session["SelectedCat"] = value;
            }
        }
        #endregion

        private void RefreshLabels()
        {
            lbSectionCount.Text = "Selected Sections : " + SelectedSections.Count.ToString();
            lbEmpCount.Text = "Selected Employees : " + SelectedEmps.Count.ToString();
            lbDeptCount.Text = "Selected Departments : " + SelectedDepts.Count.ToString();
            lbCrewCount.Text = "Selected Crews : " + SelectedCrews.Count.ToString();
            lbShiftCount.Text = "Selected Shifts : " + SelectedShifts.Count.ToString();
            lbLocCount.Text = "Selected Locations : " + SelectedLocs.Count.ToString();
            lbCatCount.Text = "Selected Types : " + SelectedTypes.Count.ToString();
            lbCompCount.Text = "Selected Companies : " + SelectedComps.Count.ToString();
        }

        #region --CheckBoxes Changed Events --
        //for sections
        protected void chkCtrlSection_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grid_Section.Rows)
            {
                CheckBox ck = ((CheckBox)row.FindControl("chkCtrlSection"));
                Section _sec = new Section();
                _sec.SectionID = Convert.ToInt16(row.Cells[1].Text);
                _sec.SectionName = row.Cells[2].Text;
                if (ck.Checked)
                {
                    if (SelectedSections.Where(sec => sec.SectionID == _sec.SectionID).Count() == 0)
                        SelectedSections.Add(_sec);
                }
                else
                {
                    if (SelectedSections.Where(sec => sec.SectionID == _sec.SectionID).Count() > 0)
                    {
                        var _section = SelectedSections.Where(sec => sec.SectionID == _sec.SectionID).First();
                        SelectedSections.Remove(_section);
                    }
                }
            }
        }

        //for departments
        protected void chkCtrlDept_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grid_Dept.Rows)
            {
                CheckBox ck = ((CheckBox)row.FindControl("chkCtrlDept"));
                Department _dept = new Department();
                _dept.DeptID = Convert.ToInt16(row.Cells[1].Text);
                _dept.DeptName = row.Cells[2].Text;
                if (ck.Checked)
                {
                    if (SelectedDepts.Where(aa => aa.DeptID == _dept.DeptID).Count() == 0)
                        SelectedDepts.Add(_dept);
                }
                else
                {
                    if (SelectedDepts.Where(aa => aa.DeptID == _dept.DeptID).Count() > 0)
                    {
                        var dept = SelectedDepts.Where(aa => aa.DeptID == _dept.DeptID).First();
                        SelectedDepts.Remove(dept);
                    }
                }
            }
        }

        //for shift
        protected void chkCtrlShift_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grid_Shift.Rows)
            {
                CheckBox ck = ((CheckBox)row.FindControl("chkCtrlShift"));
                Shift _Shift = new Shift();
                _Shift.ShiftID = Convert.ToByte(row.Cells[1].Text);
                _Shift.ShiftName = row.Cells[2].Text;
                if (ck.Checked)
                {
                    if (SelectedShifts.Where(aa => aa.ShiftID == _Shift.ShiftID).Count() == 0)
                        SelectedShifts.Add(_Shift);
                }
                else
                {
                    if (SelectedShifts.Where(aa => aa.ShiftID == _Shift.ShiftID).Count() > 0)
                    {
                        var shift = SelectedShifts.Where(aa => aa.ShiftID == _Shift.ShiftID).First();
                        SelectedShifts.Remove(shift);
                    }
                }
            }
        }
        //For employees
        protected void chkCtrlEmp_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grid_Employee.Rows)
            {
                CheckBox ck = ((CheckBox)row.FindControl("chkCtrlEmp"));
                EmpView _EmpView = new EmpView();
                _EmpView.EmpID = Convert.ToInt32(row.Cells[1].Text);
                _EmpView.EmpName = row.Cells[3].Text;
                if (ck.Checked)
                {
                    if (SelectedEmps.Where(aa => aa.EmpID == _EmpView.EmpID).Count() == 0)
                        SelectedEmps.Add(_EmpView);
                }
                else
                {
                    if (SelectedEmps.Where(aa => aa.EmpID == _EmpView.EmpID).Count() > 0)
                    {
                        var _emp = SelectedEmps.Where(aa => aa.EmpID == _EmpView.EmpID).First();
                        SelectedEmps.Remove(_emp);
                    }
                }
            }
        }
        //For crew
        protected void chkCtrlCrew_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grid_Crew.Rows)
            {
                CheckBox ck = ((CheckBox)row.FindControl("chkCtrlCrew"));
                Crew _crew = new Crew();
                _crew.CrewID = Convert.ToInt16(row.Cells[1].Text);
                _crew.CrewName = row.Cells[2].Text;
                if (ck.Checked)
                {
                    if (SelectedCrews.Where(aa => aa.CrewID == _crew.CrewID).Count() == 0)
                        SelectedCrews.Add(_crew);
                }
                else
                {
                    if (SelectedCrews.Where(aa => aa.CrewID == _crew.CrewID).Count() > 0)
                    {
                        var crew = SelectedCrews.Where(aa => aa.CrewID == _crew.CrewID).First();
                        SelectedCrews.Remove(crew);
                    }
                }
            }
        }
        //for locations
        protected void chkCtrlLoc_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grid_Location.Rows)
            {
                CheckBox ck = ((CheckBox)row.FindControl("chkCtrlLoc"));
                Location _loc = new Location();
                _loc.LocID = Convert.ToInt16(row.Cells[1].Text);
                _loc.LocName = row.Cells[2].Text;
                if (ck.Checked)
                {
                    if (SelectedLocs.Where(aa => aa.LocID == _loc.LocID).Count() == 0)
                        SelectedLocs.Add(_loc);
                }
                else
                {
                    if (SelectedLocs.Where(aa => aa.LocID == _loc.LocID).Count() > 0)
                    {
                        var loc = SelectedLocs.Where(aa => aa.LocID == _loc.LocID).First();
                        SelectedLocs.Remove(loc);
                    }
                }
            }
        }
        //for locations
        protected void chkCtrlCompany_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grid_Company.Rows)
            {
                CheckBox ck = ((CheckBox)row.FindControl("chkCtrlCompany"));
                Company _comp = new Company();
                _comp.CompID = Convert.ToInt16(row.Cells[1].Text);
                _comp.CompName = row.Cells[2].Text;
                if (ck.Checked)
                {
                    if (SelectedLocs.Where(aa => aa.LocID == _comp.CompID).Count() == 0)
                        SelectedComps.Add(_comp);
                }
                else
                {
                    if (SelectedComps.Where(aa => aa.CompID == _comp.CompID).Count() > 0)
                    {
                        var comp = SelectedComps.Where(aa => aa.CompID == _comp.CompID).First();
                        SelectedComps.Remove(comp);
                    }
                }
            }
        }
        //for Types
        protected void chkCtrlType_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grid_EmpType.Rows)
            {
                CheckBox ck = ((CheckBox)row.FindControl("chkCtrlType"));
                EmpType _Cat = new EmpType();
                _Cat.TypeID = Convert.ToByte(row.Cells[1].Text);
                _Cat.TypeName = row.Cells[2].Text;
                if (ck.Checked)
                {
                    if (SelectedTypes.Where(aa => aa.TypeID == _Cat.TypeID).Count() == 0)
                        SelectedTypes.Add(_Cat);
                }
                else
                {
                    if (SelectedTypes.Where(aa => aa.TypeID == _Cat.TypeID).Count() > 0)
                    {
                        var cat = SelectedTypes.Where(aa => aa.TypeID == _Cat.TypeID).First();
                        SelectedTypes.Remove(cat);
                    }
                }
            }
        }
        #endregion

        TAS2013Entities context = new TAS2013Entities();

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            RefreshLabels();
            DivGridSection.Visible = false;
            DivGridCrew.Visible = false;
            DivGridDept.Visible = false;
            DivGridEmp.Visible = false;
            DivShiftGrid.Visible = false;
            DivLocGrid.Visible = false;
            DivTypeGrid.Visible = false;
            ReportViewer1.Visible = true;
            List<EmpView> _TempViewList = new List<EmpView>();
            User LoggedInUser = HttpContext.Current.Session["LoggedUser"] as User;
            QueryBuilder qb = new QueryBuilder();
            string query = qb.MakeCustomizeQuery(LoggedInUser);
            DataTable dt = qb.GetValuesfromDB("select * from EmpView " + query);
            //List<ViewYLSummary> _ViewList = dt.ToList<ViewYLSummary>();
            List<EmpView> _ViewList = dt.ToList<EmpView>();
            if (SelectedEmps.Count > 0)
            {
                foreach (var emp in SelectedEmps)
                {
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpNo == emp.EmpNo).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();
            //for department
            if (SelectedDepts.Count > 0)
            {
                foreach (var dept in SelectedDepts)
                {
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptName == dept.DeptName).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (SelectedSections.Count > 0)
            {
                foreach (var sec in SelectedSections)
                {
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SectionName == sec.SectionName).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for crews
            if (SelectedCrews.Count > 0)
            {
                foreach (var cre in SelectedCrews)
                {
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewName == cre.CrewName).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for location
            if (SelectedLocs.Count > 0)
            {
                foreach (var loc in SelectedLocs)
                {
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocName == loc.LocName).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (SelectedShifts.Count > 0)
            {
                foreach (var shift in SelectedShifts)
                {
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftName == shift.ShiftName).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for category
            if (SelectedTypes.Count > 0)
            {
                foreach (var cat in SelectedTypes)
                {
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeName == cat.TypeName).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();
            if (GlobalVariables.DeploymentType == false)
            {
                PathString = "/Reports/RDLC/MYLeaveSummary.rdlc";
            }
            else
                PathString = "/WMS/Reports/RDLC/MYLeaveSummary.rdlc";
            LoadReport(PathString, GYL(_ViewList));
        }

        //public DateTime DateFrom
        //{
        //    get
        //    {
        //        if (dateFrom.Value == "")
        //            return DateTime.Today.Date.AddDays(-1);
        //        else
        //            return DateTime.Parse(dateFrom.Value);
        //    }
        //}
        //public DateTime DateTo
        //{
        //    get
        //    {
        //        if (dateTo.Value == "")
        //            return DateTime.Today.Date.AddDays(-1);
        //        else
        //            return DateTime.Parse(dateTo.Value);
        //    }
        //}

        private void LoadReport(string path, DataTable _LvSummary)
        {
            string _Header = "Year wise Leaves Summary";
            this.ReportViewer1.LocalReport.DisplayName = "Leave Summary Report";
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", _LvSummary);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportViewer1.LocalReport.Refresh();
        }

        #region --Leave Process--
        private DataTable GYL(List<EmpView> _Emp)
        {
            List<LvConsumed> leaveQuota = new List<LvConsumed>();
            List<LvConsumed> tempLeaveQuota = new List<LvConsumed>();
            leaveQuota = context.LvConsumeds.ToList();
            foreach (var emp in _Emp)
            {
                int EmpID;
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
                string Remarks = ""; string DeptName; short DeptID; string LocationName; short LocationID; string SecName; short SecID; string DesgName; short DesigID; string CrewName; short CrewID; string CompanyName; short CompanyID;
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
                    AddDataToDT(EmpID, EmpNo, EmpName, TotalAL, BalAL, TotalCL, BalCL, TotalSL, BalSL, JanAL, JanCL, JanSL, FebAL, FebCL, FebSL, MarchAL, MarchCL, MarchSL, AprilAL, AprilCL, AprilSL, MayAL, MayCL, MaySL, JunAL, JunCL, JunSL, JullyAL, JullyCL, JullySL, AugAL, AugCL, AugSL, SepAL, SepCL, SepSL, OctAL, OctCL, OctSL, NovAL, NovCL, NovSL, DecAL, DecCL, DecSL, Remarks, DeptName, (short)DeptID, LocationName, (short)LocationID, SecName, (short)SecID, DesgName, DesigID,CrewName,CrewID, CompanyName, (short)CompanyID);
                }
            }
            return MYLeaveSummaryDT;
        }



        public void AddDataToDT(int EmpID,string EmpNo, string EmpName, float TotalAL,float BalAL,
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
            MYLeaveSummaryDT.Rows.Add(EmpID, EmpNo, EmpName, TotalAL, BalAL, TotalCL, BalCL, TotalSL, BalSL, JanAL, JanCL, JanSL, FebAL, FebCL, FebSL, MarchAL, MarchCL, MarchSL,
                AprilAL, AprilCL, AprilSL, MayAL, MayCL, MaySL, JunAL, JunCL, JunSL, JullyAL, JullyCL, JullySL, AugAL, AugCL, AugSL,
                SepAL, SepCL, SepSL, OctAL, OctCL, OctSL, NovAL, NovCL, NovSL, DecAL, DecCL, DecSL, Remarks, DeptName, DeptID, LocationName, LocationID, CrewName, CrewID, SecName, SecID, CompanyName, CompanyID);
        }
        DataTable MYLeaveSummaryDT = new DataTable();
        public void CreateDataTable()
        {
            MYLeaveSummaryDT.Columns.Add("EmpID", typeof(int));
            MYLeaveSummaryDT.Columns.Add("EmpNo", typeof(string));
            MYLeaveSummaryDT.Columns.Add("EmpName", typeof(string));

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
            MYLeaveSummaryDT.Columns.Add("DecL", typeof(float));
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
        }
        #endregion
    }
}