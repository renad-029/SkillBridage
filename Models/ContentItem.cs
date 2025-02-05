namespace SkillBridage.Models
{
    public class ContentItem
    {
        public int Id { get; set; }
        public string Title { get; set; } // عنوان الوحدة
        public string Description { get; set; } // وصف الوحدة
        public int CourseId { get; set; } // الربط بالدورة
        public Course Course { get; set; } // الدورة المرتبطة


    }
}
