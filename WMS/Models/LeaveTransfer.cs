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
    
    public partial class LeaveTransfer
    {
        public int PID { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EntDate { get; set; }
        public string Status { get; set; }
        public string EmpNo { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public string LeaveType { get; set; }
    }
}