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
    
    public partial class SysMenuAccess
    {
        public int MenuID { get; set; }
        public int ModuleLayerID { get; set; }
        public int RoleID { get; set; }
    
        public virtual SysMenu SysMenu { get; set; }
        public virtual SysRole SysRole { get; set; }
    }
}
