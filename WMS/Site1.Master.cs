using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WMS.Reports;

namespace WMS
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["LogedUserID"].ToString() != "")
            {
                //Deployment Type =false : Local Deployment
                //Deployment Type =true: Server Deployment
                GlobalVariables.DeploymentType = true;
            }
            else
                Response.Redirect("~/Home");
        }
    }
}