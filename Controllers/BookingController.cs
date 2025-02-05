using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBridage.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SkillBridage.Models;
using System.Security.Claims;

namespace SkillBridage.Controllers
{
    [Authorize(Roles = "Student")]  
    public class BookingController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public BookingController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            var bookedCourses = _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Course)
                .ToList();

            var numberOfCourses = bookedCourses.Count;

            var courseDetails = bookedCourses.Select(b => new
            {
                CourseName = b.Course.Name,
                StartDate = b.Course.StartDate.ToString("yyyy-MM-dd"),
                EndDate = b.Course.EndDate.ToString("yyyy-MM-dd")
            }).ToList();

            ViewBag.NumberOfCourses = numberOfCourses;
            ViewBag.CourseDetails = courseDetails;

            return View();

        }


        [HttpGet]
        public IActionResult AvailableCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // الحصول على userId للمستخدم الحالي
            var courses = _context.Course
                .Where(c => c.Status == CourseStatus.Active)  // جلب الكورسات النشطة فقط
                .ToList();

            // إذا لم توجد كورسات، يتم عرض رسالة
            if (!courses.Any())
            {
                TempData["Message"] = "لا توجد دورات متاحة حاليًا.";
            }

            // التحقق من حالة الاشتراك لكل دورة
            foreach (var course in courses)
            {
                // التحقق إذا كان المستخدم قد اشترك في الدورة
                var existingBooking = _context.Bookings
                    .FirstOrDefault(b => b.UserId == userId && b.CourseId == course.Id);

                // إذا كانت قيمة existingBooking غير null، فهذا يعني أن المستخدم قد اشترك
                ViewData[$"IsUserBooked_{course.Id}"] = existingBooking != null;
            }

            // إعادة عرض الكورسات
            return View(courses);
        }



        // إجراء الحجز
        [HttpPost]
        public IActionResult BookCourse(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var course = _context.Course.FirstOrDefault(c => c.Id == courseId);
            if (course == null || course.Status != CourseStatus.Active)
            {
                TempData["Message"] = "الدورة غير موجودة أو التسجيل غير متاح.";
                return RedirectToAction("AvailableCourses");
            }

            var existingBooking = _context.Bookings
                .FirstOrDefault(b => b.UserId == userId && b.CourseId == courseId);

            if (existingBooking != null)
            {
                TempData["Message"] = "أنت مسجل بالفعل في هذه الدورة.";
                return RedirectToAction("AvailableCourses");
            }

            var booking = new Booking
            {
                CourseId = courseId,
                UserId = userId,
                BookingDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            TempData["Message"] = "تم تسجيل الحجز بنجاح!";
            return RedirectToAction("Confirmation");
        }

        // عرض رسالة تأكيد
        public IActionResult Confirmation()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }


        // عرض حجوزات المستخدم
        public IActionResult MyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var bookings = _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Course)
                .ToList();

            if (!bookings.Any())
            {
                TempData["Message"] = "ليس لديك حجوزات حالياً.";
            }

            return View(bookings); 
        }

        [HttpPost]
        public IActionResult CancelBooking(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            // التحقق إذا كان الحجز موجودًا وكان للمستخدم نفسه
            var booking = _context.Bookings
                .FirstOrDefault(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "الحجز غير موجود أو لا يمكنك إلغاء هذا الحجز.";
                return RedirectToAction("MyBookings");
            }

            // حذف الحجز من قاعدة البيانات
            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            TempData["Message"] = "تم إلغاء الحجز بنجاح.";
            return RedirectToAction("MyBookings");
        }

        public IActionResult CourseDetails(int courseId)
        {
            var course = _context.Course
      .Include(c => c.ContentItem)
      .FirstOrDefault(c => c.Id == courseId);

            if (course == null)
            {
                TempData["Message"] = "Course not found.";
                return RedirectToAction("MyBookings");
            }

            return View(course);
        }












    }
}
