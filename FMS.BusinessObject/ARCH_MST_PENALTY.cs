//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FMS.BusinessObject
{
    using System;
    using System.Collections.Generic;
    
    public partial class ARCH_MST_PENALTY
    {
        public int MST_PENALTY_ID { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIES { get; set; }
        public Nullable<int> YEAR { get; set; }
        public Nullable<int> MONTH_START { get; set; }
        public Nullable<int> MONTH_END { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public Nullable<int> PENALTY { get; set; }
        public Nullable<bool> RESTITUTION { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<bool> IS_ACTIVE { get; set; }
        public Nullable<int> VENDOR { get; set; }
        public string BODY_TYPE { get; set; }
        public Nullable<System.DateTime> ARCHIVED_DATE { get; set; }
        public string ARCHIVED_BY { get; set; }
    }
}