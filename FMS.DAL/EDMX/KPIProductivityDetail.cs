//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DFIS.EntitiesDAL.EDMX
{
    using System;
    using System.Collections.Generic;
    
    public partial class KPIProductivityDetail
    {
        public int IDProductivityDetail { get; set; }
        public string ProductivityNumber { get; set; }
        public string Status { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Value { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    
        public virtual KPIProductivity KPIProductivity { get; set; }
    }
}
