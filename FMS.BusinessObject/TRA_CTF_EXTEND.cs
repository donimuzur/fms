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
    
    public partial class TRA_CTF_EXTEND
    {
        public long TRA_CTF_EXTEND_ID { get; set; }
        public Nullable<long> TRA_CTF_ID { get; set; }
        public Nullable<System.DateTime> NEW_PROPOSED_DATE { get; set; }
        public string EXTEND_PO_NUMBER { get; set; }
        public string EXTEND_PO_LINE { get; set; }
        public Nullable<decimal> EXTEND_PRICE { get; set; }
        public Nullable<int> REASON { get; set; }
    
        public virtual MST_REASON MST_REASON { get; set; }
        public virtual TRA_CTF TRA_CTF { get; set; }
    }
}