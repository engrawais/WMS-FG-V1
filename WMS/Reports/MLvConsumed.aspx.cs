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
    public partial class MLvConsumed : System.Web.UI.Page
    {
        string PathString = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CreateDatatable();
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
                string _period = DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
                if (GlobalVariables.DeploymentType == false)
                {
                    PathString = "/Reports/RDLC/MLvApplication.rdlc";
                }
                else
                    PathString = "/WMS/Reports/RDLC/MLvApplication.rdlc";
                User LoggedInUser = HttpContext.Current.Session["LoggedUser"] as User;
                QueryBuilder qb = new QueryBuilder();
                string query = qb.MakeCustomizeQuery(LoggedInUser);
                //DataTable dt = qb.GetValuesfromDB("select * from EmpView " + query);
                DataTable dt = qb.GetValuesfromDB("select * from EmpView ");
                List<EmpView> _emp = dt.ToList<EmpView>();
                LoadReport(PathString, GetLV(_emp, DateTime.Now.Month));
            }
        }
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
        //for Company
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
            CreateDatatable();
            DivGridSection.Visible = false;
            DivGridCrew.Visible = false;
            DivGridDept.Visible = false;
            DivGridEmp.Visible = false;
            DivShiftGrid.Visible = false;
            DivLocGrid.Visible = false;
            DivTypeGrid.Visible = false;
            ReportViewer1.Visible = true;
            DivGridComapny.Visible = false;
            List<EmpView> _ViewList = new List<EmpView>();
            List<EmpView> _TempViewList = new List<EmpView>();
            _ViewList = context.EmpViews.ToList();
            if (SelectedEmps.Count > 0)
            {
                foreach (var emp in SelectedEmps)
                {
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == emp.EmpID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();
            //for department
            if (SelectedDepts.Count > 0)
            {
                string _deptName = "";
                foreach (var dept in SelectedDepts)
                {
                    _deptName = context.Departments.FirstOrDefault(aa => aa.DeptID == dept.DeptID).DeptName;
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptName == _deptName).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for company
            if (SelectedComps.Count > 0)
            {
                foreach (var comp in SelectedComps)
                {
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompName == comp.CompName).ToList());
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
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == shift.ShiftID).ToList());
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
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == cat.TypeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();
            if (GlobalVariables.DeploymentType == false)
            {
                PathString = "/Reports/RDLC/MLvApplication.rdlc";
            }
            else
                PathString = "/WMS/Reports/RDLC/MLvApplication.rdlc";
            LoadReport(PathString, GetLV(_ViewList.ToList(), DateTo.Date.Month));
        }
        public DateTime DateTo
        {
            get
            {
                if (dateTo.Value == "")
                    return DateTime.Today.Date.AddDays(-1);
                else
                    return DateTime.Parse(dateTo.Value);
            }
        }
        private void LoadReport(string path, DataTable _LvSummary)
        {
            string _Header ="Monthly Sheet Contactual Employees";
            string Date = DateTo.Date.ToString("dd-MMM-yyyy");
            this.ReportViewer1.LocalReport.DisplayName = "Leave Balance Report";
            string _Date = "Month: " + DateTo.Date.ToString("MMMM") + " , " + DateTo.Year.ToString();
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", _LvSummary);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", Date, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp,rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;
            string filename;

            byte[] bytes = ReportViewer1.LocalReport.Render(
               "Excel", null, out mimeType, out encoding,
                out extension,
               out streamids, out warnings);

            filename = string.Format("{0}.{1}", "ExportToExcel", "xls");
            Response.ClearHeaders();
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
            Response.ContentType = mimeType;
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }


        #region --Leave Process--
        private DataTable GetLV(List<EmpView> _Emp, int month)
        {
            using (var ctx = new TAS2013Entities())
            {
                
                List<LvConsumed> _lvConsumed = new List<LvConsumed>();
                LvConsumed _lvTemp = new LvConsumed();
                _lvConsumed = ctx.LvConsumeds.ToList();
                List<LvType> _lvTypes = ctx.LvTypes.ToList();
                foreach (var emp in _Emp)
                {
                    float BeforeCL = 0, UsedCL = 0, BalCL = 0;
                    float BeforeSL = 0, UsedSL = 0, BalSL = 0;
                    float BeforeAL = 0, UsedAL = 0, BalAL = 0;
                    string _month = "";
                    List<LvConsumed> entries = ctx.LvConsumeds.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    LvConsumed eCL = entries.FirstOrDefault(lv => lv.LeaveType == "A");
                    LvConsumed eSL = entries.FirstOrDefault(lv => lv.LeaveType == "C");
                    LvConsumed eAL = entries.FirstOrDefault(lv => lv.LeaveType == "B");
                    if (entries.Count > 0)
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
                                UsedAL = (float)eAL.SepConsumed;
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
                                break;
                            case 12:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed + (float)eCL.OctConsumed + (float)eCL.NovConsumed);
                                UsedCL = (float)eCL.DecConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed + (float)eSL.OctConsumed + (float)eSL.NovConsumed);
                                UsedSL = (float)eSL.DecConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.AugustConsumed + (float)eAL.OctConsumed + (float)eAL.NovConsumed);
                                UsedAL = (float)eAL.DecConsumed;
                                break;

                        }
                        BalCL = (float)(BeforeCL - UsedCL);
                        BalSL = (float)(BeforeSL - UsedSL);
                        BalAL = (float)(BeforeAL - UsedAL);
                        AddDataToDT(emp.EmpNo, emp.EmpName, emp.DesignationName, emp.SectionName, emp.DeptName, emp.TypeName, emp.CatName, emp.LocName, BeforeCL, BeforeSL, BeforeAL, UsedCL, UsedSL, UsedAL, BalCL, BalSL, BalAL, _month);
                    }

                }
            }
            return LvSummaryMonth;
        }
        public void AddDataToDT(string EmpNo, string EmpName, string Designation, string Section,
                                 string Department, string EmpType, string Category, string Location,
                                 float TotalCL, float TotalSL, float TotalAL,
                                 float ConsumedCL, float ConsumedSL, float ConsumedAL,
                                 float BalanaceCL, float BalanaceSL, float BalananceAL, string Month)
        {
            LvSummaryMonth.Rows.Add(EmpNo, EmpName, Designation, Section, Department, EmpType, Category, Location,
                TotalCL, TotalSL, TotalAL, ConsumedCL, ConsumedSL, ConsumedAL,
                BalanaceCL, BalanaceSL, BalananceAL, Month);
        }
        DataTable LvSummaryMonth = new DataTable();

        public void CreateDatatable()
        {
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
            LvSummaryMonth.Columns.Add("ConsumedCL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedSL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedAL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceCL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceSL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceAL", typeof(float));
            LvSummaryMonth.Columns.Add("Remarks", typeof(string));
            LvSummaryMonth.Columns.Add("Month", typeof(string));
        }
        #endregion

    }
}