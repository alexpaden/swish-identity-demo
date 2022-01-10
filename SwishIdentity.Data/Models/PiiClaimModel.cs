using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwishIdentity.Data.Models
{
    public class PiiClaimModel
    {
        [Key]
        public Guid ClaimTx { get; set; }
        
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public virtual ProfileModel Profile { get; set; }
        
        [ForeignKey("Manager")]
        public Guid ManagerId { get; set; }
        public virtual ManagerModel Manager { get; set; }
        
        public DateTime DateTime { get; set; }

        public string Other { get; set; }
    }
}