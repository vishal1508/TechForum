//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tech_Forum.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Subscriber_Table
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Subscriber_Table()
        {
            this.Post_Table = new HashSet<Post_Table>();
        }
    
        public string userid { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public Nullable<double> rating { get; set; }
        public string mobile { get; set; }
        public string description { get; set; }
        public bool IsEmailVerified { get; set; }
        public System.Guid ActivationCode { get; set; }
        public string ResetPasswordCode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post_Table> Post_Table { get; set; }
    }
}
