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
    
    public partial class CustomReportState
    {
        public int IDCustomReportLayout { get; set; }
        public string IDUser { get; set; }
        public string PageName { get; set; }
        public string FieldName { get; set; }
        public string LayoutName { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    
        public virtual MasterUser MasterUser { get; set; }
    }
}
