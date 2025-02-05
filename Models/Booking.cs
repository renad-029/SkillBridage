using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SkillBridage.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public string UserId { get; set; }

       
        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
        
        public DateTime LastModified { get; set; } = DateTime.Now;

         
        [ForeignKey("UserId")]
         public IdentityUser User { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }


    }
}
