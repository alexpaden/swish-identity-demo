using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwishIdentity.Data.Models
{
    public class ProfileHistoryModel
    {
        [Key] 
        public Guid HistoryId { get; set; }
        
        [ForeignKey("Profile")] 
        public Guid ProfileId;
        public virtual ProfileModel Profile { get; set; }

        public string ProfileHistory { get; set; }
    }
}