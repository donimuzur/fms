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
    
    public partial class WarehouseStockCount
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WarehouseStockCount()
        {
            this.WarehouseStockCountDetails = new HashSet<WarehouseStockCountDetail>();
        }
    
        public int IDStockCount { get; set; }
        public System.DateTime TransDate { get; set; }
        public bool Type { get; set; }
        public string IDLocation { get; set; }
        public byte Shift { get; set; }
        public string WarehouseStaff1 { get; set; }
        public string WarehouseStaff2 { get; set; }
        public Nullable<byte> NextShift { get; set; }
        public string Attachment { get; set; }
        public Nullable<System.DateTime> SyncedTime { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> TotalSAP1000Box { get; set; }
        public Nullable<decimal> TotalSAP1000Pack { get; set; }
        public Nullable<decimal> TotalMan1000Box { get; set; }
        public Nullable<decimal> TotalMan1000Pack { get; set; }
        public Nullable<decimal> TotalSAP1040Box { get; set; }
        public Nullable<decimal> TotalSAP1040Pack { get; set; }
        public Nullable<decimal> TotalMan1040Box { get; set; }
        public Nullable<decimal> TotalMan1040Pack { get; set; }
        public Nullable<decimal> TotalSAP1041Box { get; set; }
        public Nullable<decimal> TotalSAP1041Pack { get; set; }
        public Nullable<decimal> TotalMan1041Box { get; set; }
        public Nullable<decimal> TotalMan1041Pack { get; set; }
        public Nullable<decimal> TotalSAP1042Box { get; set; }
        public Nullable<decimal> TotalSAP1042Pack { get; set; }
        public Nullable<decimal> TotalMan1042Box { get; set; }
        public Nullable<decimal> TotalMan1042Pack { get; set; }
        public Nullable<decimal> TotalSAP1043Box { get; set; }
        public Nullable<decimal> TotalSAP1043Pack { get; set; }
        public Nullable<decimal> TotalMan1043Box { get; set; }
        public Nullable<decimal> TotalMan1043Pack { get; set; }
        public Nullable<decimal> TotalSAP1046Box { get; set; }
        public Nullable<decimal> TotalSAP1046Pack { get; set; }
        public Nullable<decimal> TotalMan1046Box { get; set; }
        public Nullable<decimal> TotalMan1046Pack { get; set; }
        public Nullable<decimal> InvAcc { get; set; }
        public Nullable<decimal> InvUtil { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    
        public virtual MasterLocation MasterLocation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WarehouseStockCountDetail> WarehouseStockCountDetails { get; set; }
    }
}
