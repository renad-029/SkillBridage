using System.ComponentModel.DataAnnotations;

namespace SkillBridage.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public CourseStatus Status { get; set; }

        public string Mentor { get; set; }

        // علاقة مع جدول الحجز
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();


        // خاصية لحساب عدد المسجلين
        public int StudentCount => Bookings?.Count ?? 0;


        public ICollection<ContentItem> ContentItem { get; set; }  // ارتباط مع المحتوى

    }
}
