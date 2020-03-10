using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WMS.Models;

namespace WMS
{
    public partial class LogIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        protected void btnlogin_Click(object sender, EventArgs e)
        {
            TAS2013Entities context = new TAS2013Entities();
            //lbError.Text = "";
            List<User> _User = context.Users.ToList();

            //Request.Cookies[user]
            if (tbUserName.Text != "" && tbPassword.Text != "")
            {
             foreach (var item in _User)
                {
                    if (item.UserName.ToUpper() == tbUserName.Text.ToUpper())
                    {
                        if (item.Password.ToUpper() == tbPassword.Text.ToUpper())
                        {
                            if (item.RoleID == 2 || item.RoleID == 3)
                            {
                                Response.Redirect("/WMS/Reports/ReportHome.aspx");
                                Session["RoleID"] = "Rpt";
                                //SaveAuditLogEntry(item.UserID, item.UserName);
                            }
                            else
                            {
                                Response.Redirect("~/Home");
                                if (item.RoleID == 4)
                                {
                                    Session["RoleID"] = "Entry";
                                }
                                //SaveAuditLogEntry(item.UserID, item.UserName);
                            }
                            if (item.RoleID == 5)
                            {
                                Session["RoleID"] = "Admin";
                            }
                        }
                        else
                        {
                            //lbError.Text = "*Password doesnot match";
                        }
                    }
                    else
                    {
                        //lbError.Text = "*Username is not valid";
                    }
                }
            }
        }
        //public void SaveAuditLogEntry(int _userId, string _name)
        //{
        //    using (var ctx = new TAS2013Entities())
        //    {
        //        AuditLog _AuditLog = new AuditLog();
        //        _AuditLog.AuditUserID = _userId;
        //        _AuditLog.AuditUserName = _name;
        //        _AuditLog.AuditTime = DateTime.Now;
        //        ctx.AuditLogs.Add(_AuditLog);
        //        ctx.SaveChanges();
        //    }

        //}
    }
}