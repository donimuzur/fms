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
    
    public partial class GeneralBuildingFacilityIcon
    {
        public int IDBuildingFacilityIcon { get; set; }
        public Nullable<int> IDBuildingFacility { get; set; }
        public int IDIconFacility { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    
        public virtual GeneralBuildingFacility GeneralBuildingFacility { get; set; }
        public virtual MasterIconFacility MasterIconFacility { get; set; }
    }
}
