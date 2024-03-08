//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace University.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EstimationRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EstimationRequest()
        {
            this.UploadedDocuments = new HashSet<UploadedDocument>();
        }
    
        public int Id { get; set; }
        public string ApplicantName { get; set; }
        public string MortgagerName { get; set; }
        public string EstimationAddress1 { get; set; }
        public string HardCopyFiles { get; set; }
        public string ManagerResponse { get; set; }
        public int CustomerType { get; set; }
        public int PropertyType { get; set; }
        public int ValuationPurpose { get; set; }
        public string HardCopyStatus { get; set; }
        public string CSRRemark { get; set; }
        public string EstimationReport { get; set; }
        public int UploadedBy { get; set; }
        public System.DateTime DateUploaded { get; set; }
        public Nullable<int> AssignedBy { get; set; }
        public Nullable<int> AssignedTo { get; set; }
        public int Status { get; set; }
        public string RequestingBranch { get; set; }
        public string EstimationAddress2 { get; set; }
        public string ValuerRemark { get; set; }
        public Nullable<System.DateTime> DateAssigned { get; set; }
        public Nullable<System.DateTime> ValuationDate { get; set; }
        public string ValuationTime { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual luCustomerType luCustomerType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UploadedDocument> UploadedDocuments { get; set; }
        public virtual luPropertyType luPropertyType { get; set; }
        public virtual luDocumentStatu luDocumentStatu { get; set; }
        public virtual User User2 { get; set; }
        public virtual luValuationPurpose luValuationPurpose { get; set; }
    }
}
