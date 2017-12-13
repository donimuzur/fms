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
    
    public partial class MasterFABrand
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MasterFABrand()
        {
            this.TransportLostClaimFACodes = new HashSet<TransportLostClaimFACode>();
            this.WarehouseStockCountDetails = new HashSet<WarehouseStockCountDetail>();
        }
    
        public string FACode { get; set; }
        public string SpeakingCode { get; set; }
        public string Type { get; set; }
        public decimal StickPerBox { get; set; }
        public decimal PackPerBox { get; set; }
        public decimal StickPerPack { get; set; }
        public int HJE { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransportLostClaimFACode> TransportLostClaimFACodes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WarehouseStockCountDetail> WarehouseStockCountDetails { get; set; }
    }
}
