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
    
    public partial class MST_COST_OB
    {
        public long MST_COST_OB_ID { get; set; }
        public int YEAR { get; set; }
        public string ZONE { get; set; }
        public string MODEL { get; set; }
        public string TYPE { get; set; }
        public Nullable<decimal> OB_COST { get; set; }
        public string REMARK { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string COST_CENTER { get; set; }
        public Nullable<int> QTY { get; set; }
        public Nullable<int> MONTH { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string FUNCTION_NAME { get; set; }
        public string REGIONAL { get; set; }
    }
}
