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
    
    public partial class WarehouseBeetleMonitoring
    {
        public int IDBeetleMonitoring { get; set; }
        public string IDLocation { get; set; }
        public System.DateTime InspectionDate { get; set; }
        public int IDEqupmentType { get; set; }
        public int NumberOfBeetle { get; set; }
        public bool Status { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    
        public virtual MasterEquipmentType MasterEquipmentType { get; set; }
        public virtual MasterLocation MasterLocation { get; set; }
    }
}
