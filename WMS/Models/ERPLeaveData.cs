//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ERPLeaveData
    {
        public int PID { get; set; }
        public Nullable<int> EmpID { get; set; }
        public string EmpNo { get; set; }
        public string ERPKey { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string LeaveType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public Nullable<bool> IsUpdated { get; set; }
    }
}