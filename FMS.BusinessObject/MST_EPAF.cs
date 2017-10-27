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
    
    public partial class MST_EPAF
    {
        public MST_EPAF()
        {
            this.TRA_CRF = new HashSet<TRA_CRF>();
            this.TRA_CSF = new HashSet<TRA_CSF>();
            this.TRA_CTF = new HashSet<TRA_CTF>();
        }
    
        public long MST_EPAF_ID { get; set; }
        public int DOCUMENT_TYPE { get; set; }
        public string EPAF_ACTION { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string COST_CENTER { get; set; }
        public Nullable<System.DateTime> EFFECTIVE_DATE { get; set; }
        public Nullable<int> GROUP_LEVEL { get; set; }
        public string CITY { get; set; }
        public string BASE_TOWN { get; set; }
        public Nullable<bool> EXPAT { get; set; }
        public Nullable<bool> LETTER_SEND { get; set; }
        public string LAST_UPDATED { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string EPAF_NUMBER { get; set; }
        public Nullable<System.DateTime> EPAF_APPROVED_DATE { get; set; }
        public Nullable<int> REMARK { get; set; }
    
        public virtual MST_DOCUMENT_TYPE MST_DOCUMENT_TYPE { get; set; }
        public virtual ICollection<TRA_CRF> TRA_CRF { get; set; }
        public virtual ICollection<TRA_CSF> TRA_CSF { get; set; }
        public virtual ICollection<TRA_CTF> TRA_CTF { get; set; }
        public virtual MST_EMPLOYEE MST_EMPLOYEE { get; set; }
        public virtual MST_REMARK MST_REMARK { get; set; }
    }
}
