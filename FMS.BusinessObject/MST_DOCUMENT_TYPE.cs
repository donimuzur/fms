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
    
    public partial class MST_DOCUMENT_TYPE
    {
        public MST_DOCUMENT_TYPE()
        {
            this.MST_REMARK = new HashSet<MST_REMARK>();
            this.MST_REASON = new HashSet<MST_REASON>();
            this.MST_EPAF = new HashSet<MST_EPAF>();
        }
    
        public int MST_DOCUMENT_TYPE_ID { get; set; }
        public string DOCUMENT_TYPE { get; set; }
        public string DOCUMENT_INISIAL { get; set; }
    
        public virtual ICollection<MST_REMARK> MST_REMARK { get; set; }
        public virtual ICollection<MST_REASON> MST_REASON { get; set; }
        public virtual ICollection<MST_EPAF> MST_EPAF { get; set; }
    }
}
