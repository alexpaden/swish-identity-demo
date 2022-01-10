using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwishIdentity.Data.Models
{
    public class ProfileModel
    {
        [Key]
        public Guid ProfileId { get; set; }
        
        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual SwishUser User { get; set; }

        public string Name { get; set; }
        
        public string Country { get; set; }
        
        [EmailAddress] public string SharedEmail { get; set; }
        
        public string ImgFrontUrl { get; set; }
        
        public string ImgRearUrl { get; set; }
        
    }
}