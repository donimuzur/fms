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
    
    public partial class TRA_CSF
    {
        public long TRA_CSF_ID { get; set; }
        public string DOCUMENT_NUMBER { get; set; }
        public FMS.Core.Enums.DocumentStatus DOCUMENT_STATUS { get; set; }
        public Nullable<long> EPAF_ID { get; set; }
        public int REASON { get; set; }
        public System.DateTime EFFECTIVE_DATE { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string COST_CENTER { get; set; }
        public Nullable<int> GROUP_LEVEL { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string VEHICLE_CATEGORY { get; set; }
        public string VEHICLE_USAGE { get; set; }
        public string LOCATION_CITY { get; set; }
        public string LOCATION_ADDRESS { get; set; }
        public string POLICE_NUMBER { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIES { get; set; }
        public string BODY_TYPE { get; set; }
        public string COLOUR { get; set; }
        public Nullable<System.DateTime> START_PERIOD { get; set; }
        public Nullable<System.DateTime> END_PERIOD { get; set; }
        public Nullable<int> VENDOR { get; set; }
        public Nullable<System.DateTime> EXPECTED_DATE { get; set; }
        public Nullable<System.DateTime> END_RENT_DATE { get; set; }
        public string SUPPLY_METHOD { get; set; }
        public Nullable<bool> IS_PROJECT { get; set; }
        public string PROJECT_NAME { get; set; }
        public string PO_NUMBER { get; set; }
        public Nullable<int> REMARK { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
        public Nullable<System.DateTime> START_RENT_DATE { get; set; }
        public string VENDOR_NAME { get; set; }
        public string APPROVED_FLEET { get; set; }
        public Nullable<System.DateTime> APPROVED_FLEET_DATE { get; set; }
        public string VENDOR_POLICE_NUMBER { get; set; }
        public string VENDOR_CHASIS_NUMBER { get; set; }
        public string VENDOR_ENGINE_NUMBER { get; set; }
        public Nullable<System.DateTime> VENDOR_CONTRACT_START_DATE { get; set; }
        public Nullable<System.DateTime> VENDOR_CONTRACT_END_DATE { get; set; }
        public string VENDOR_MANUFACTURER { get; set; }
        public string VENDOR_MODEL { get; set; }
        public string VENDOR_SERIES { get; set; }
        public string VENDOR_TRANSMISSION { get; set; }
        public string VENDOR_COLOUR { get; set; }
        public string VENDOR_BODY_TYPE { get; set; }
        public Nullable<bool> VENDOR_AIR_BAG { get; set; }
        public Nullable<bool> VENDOR_ABS { get; set; }
        public string VENDOR_BRANDING { get; set; }
        public string VENDOR_PURPOSE { get; set; }
        public string VENDOR_PO_NUMBER { get; set; }
        public string VENDOR_PO_LINE { get; set; }
        public Nullable<bool> VENDOR_VAT { get; set; }
        public Nullable<bool> VENDOR_RESTITUTION { get; set; }
        public string VENDOR_VENDOR { get; set; }
        public string EMPLOYEE_ID_CREATOR { get; set; }
        public string EMPLOYEE_ID_FLEET_APPROVAL { get; set; }
        public Nullable<int> CAR_GROUP_LEVEL { get; set; }
        public Nullable<long> CFM_IDLE_ID { get; set; }
    
        public virtual MST_EMPLOYEE MST_EMPLOYEE { get; set; }
        public virtual MST_REMARK MST_REMARK { get; set; }
        public virtual MST_REASON MST_REASON { get; set; }
        public virtual MST_EPAF MST_EPAF { get; set; }
    }
}
