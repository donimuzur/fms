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
    
    public partial class TRA_WORKFLOW_HISTORY
    {
        public long TRA_WORKFLOW_HISTORY_ID { get; set; }
        public Nullable<int> MODUL_ID { get; set; }
        public Nullable<long> FORM_ID { get; set; }
        public int ACTION { get; set; }
        public string ACTION_BY { get; set; }
        public Nullable<System.DateTime> ACTION_DATE { get; set; }
        public Nullable<int> REMARK { get; set; }
    
        public virtual MST_MODUL MST_MODUL { get; set; }
        public virtual MST_REMARK MST_REMARK { get; set; }
    }
}
