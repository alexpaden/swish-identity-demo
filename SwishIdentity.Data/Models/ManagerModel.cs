using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwishIdentity.Data.Models
{
    public class ManagerModel
    {
        [Key]
        public Guid ManagerId { get; set; }
        
        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual SwishUser User { get; set; }
        
        public bool ManagerEnabled { get; set; }

        public string BusinessName { get; set; }
        
        [EmailAddress]
        public string BusinessEmail { get; set; }
        
        public string Reason { get; set; }
    }
}