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
    
    public partial class AuditEmail
    {
        public int ID { get; set; }
        public Nullable<int> EmailFormID { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> Dated { get; set; }
    }
}