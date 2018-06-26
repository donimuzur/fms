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
    
    public partial class ARCH_TRA_CTF
    {
        public long TRA_CTF_ID { get; set; }
        public string DOCUMENT_NUMBER { get; set; }
        public FMS.Core.Enums.DocumentStatus DOCUMENT_STATUS { get; set; }
        public Nullable<long> EPAF_ID { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string COST_CENTER { get; set; }
        public Nullable<int> GROUP_LEVEL { get; set; }
        public Nullable<int> REASON { get; set; }
        public string SUPPLY_METHOD { get; set; }
        public string POLICE_NUMBER { get; set; }
        public Nullable<int> VEHICLE_YEAR { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string VEHICLE_USAGE { get; set; }
        public Nullable<System.DateTime> END_RENT_DATE { get; set; }
        public Nullable<System.DateTime> EFFECTIVE_DATE { get; set; }
        public Nullable<bool> IS_TRANSFER_TO_IDLE { get; set; }
        public Nullable<decimal> BUY_COST { get; set; }
        public Nullable<bool> EXTEND_VEHICLE { get; set; }
        public string WITHD_PIC { get; set; }
        public string WITHD_PHONE { get; set; }
        public Nullable<System.DateTime> WITHD_DATE { get; set; }
        public string WITHD_CITY { get; set; }
        public string WITHD_ADDRESS { get; set; }
        public Nullable<decimal> EMPLOYEE_CONTRIBUTION { get; set; }
        public Nullable<decimal> PENALTY { get; set; }
        public Nullable<decimal> REFUND_COST { get; set; }
        public Nullable<decimal> BUY_COST_TOTAL { get; set; }
        public Nullable<int> USER_DECISION { get; set; }
        public string PENALTY_PO_NUMBER { get; set; }
        public string PENALTY_PO_LINE { get; set; }
        public Nullable<decimal> PENALTY_PRICE { get; set; }
        public Nullable<int> REMARK { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string VEHICLE_LOCATION { get; set; }
        public string EMPLOYEE_ID_CREATOR { get; set; }
        public string EMPLOYEE_ID_FLEET_APPROVAL { get; set; }
        public string APPROVED_FLEET { get; set; }
        public Nullable<System.DateTime> APPROVED_FLEET_DATE { get; set; }
        public Nullable<System.DateTime> DATE_SEND_VENDOR { get; set; }
        public string ARCHIVED_BY { get; set; }
        public Nullable<System.DateTime> ARCHIVED_DATE { get; set; }
    }
}
