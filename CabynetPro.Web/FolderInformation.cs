//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CabynetPro.Web
{
    using System;
    using System.Collections.Generic;
    
    public partial class FolderInformation
    {
        public long Id { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int UserCreated { get; set; }
        public string FolderName { get; set; }
        public int CategoryId { get; set; }
        public string FolderDescription { get; set; }
        public long ParentFolderId { get; set; }
        public bool IsDeleted { get; set; }
    }
}