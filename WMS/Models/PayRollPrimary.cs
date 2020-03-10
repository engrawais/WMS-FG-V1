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
    
    public partial class PayRollPrimary
    {
        public string EmpMonthYear { get; set; }
        public Nullable<short> CompanyID { get; set; }
        public string MonthYear { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string EmpNo { get; set; }
        public Nullable<short> TotalDays { get; set; }
        public Nullable<double> EarnedDays { get; set; }
        public Nullable<double> PresentDays { get; set; }
        public Nullable<double> RestDays { get; set; }
        public Nullable<double> GZDays { get; set; }
        public Nullable<double> AbsentDays { get; set; }
        public Nullable<double> SickLeaveDays { get; set; }
        public Nullable<double> AnnualLeaveDays { get; set; }
        public Nullable<double> CasualDays { get; set; }
        public Nullable<double> OtherLeaveDays { get; set; }
        public Nullable<short> LateInMins { get; set; }
        public Nullable<short> OverTimeMins { get; set; }
        public Nullable<short> GZOTMins { get; set; }
        public Nullable<int> EmpID { get; set; }
        public Nullable<int> AnnualLVEncashmentDays { get; set; }
        public string BadliPNo { get; set; }
        public Nullable<short> BadliCompanyID { get; set; }
        public Nullable<short> BadliDays { get; set; }
        public Nullable<int> PreparedBy { get; set; }
        public Nullable<System.DateTime> PreparedDate { get; set; }
        public Nullable<bool> Approved { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> IsEdited { get; set; }
        public Nullable<bool> TransferedToOracle { get; set; }
        public Nullable<System.DateTime> TransferedDate { get; set; }
        public Nullable<int> ApprovedUser { get; set; }
        public Nullable<System.DateTime> ApprovedDateTime { get; set; }
    }
}