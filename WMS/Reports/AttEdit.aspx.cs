using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WMS.Models;

namespace WMS.Reports
{
    public partial class AttEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["EmpNo"] = "159";
        }
        TAS2013Entities context = new TAS2013Entities();
        protected void Button1_Click1(object sender, EventArgs e)
        {
            var emp = context.Emps.Where(aa => aa.EmpNo == tbEmpNo.Text).ToList();
            tbTimeIn.Text = emp.FirstOrDefault().EmpName;
            tbTimeOut.Enabled = false;
        }




    }
}