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
    
    public partial class TransportVesselMonitoring
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TransportVesselMonitoring()
        {
            this.TransportVesselMonitoringDetails = new HashSet<TransportVesselMonitoringDetail>();
        }
    
        public string GPNumber { get; set; }
        public Nullable<System.DateTime> DateOfStuffing { get; set; }
        public string Region { get; set; }
        public string IDVendor { get; set; }
        public string ContainerNumber { get; set; }
        public string ContainerSeal { get; set; }
        public string TypeOfContainer { get; set; }
        public string ShipName { get; set; }
        public string POWeekAgent { get; set; }
        public Nullable<System.DateTime> EDT1 { get; set; }
        public Nullable<System.DateTime> EDT2 { get; set; }
        public Nullable<System.DateTime> ATD { get; set; }
        public Nullable<System.DateTime> ETA1 { get; set; }
        public Nullable<System.DateTime> ETA2 { get; set; }
        public Nullable<System.DateTime> ATA { get; set; }
        public Nullable<System.DateTime> EstimationReceived { get; set; }
        public Nullable<System.DateTime> ActualReceived { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<decimal> Distance { get; set; }
    
        public virtual MasterVendor MasterVendor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransportVesselMonitoringDetail> TransportVesselMonitoringDetails { get; set; }
    }
}
