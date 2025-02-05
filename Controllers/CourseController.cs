using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBridage.Data;
using SkillBridage.Models;
using System.Security.Claims;

namespace SkillBridage.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CourseController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var courses = _context.Course
                .Where(c => c.Mentor == mentorId)
                .Include(c => c.Bookings) 
                .ToList();

            ViewBag.TotalCourses = courses.Count();
            ViewBag.TotalStudents = courses.Sum(c => c.Bookings.Count);

            return View(courses);
        }

        public IActionResult Course()
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = _context.Course
                .Where(c => c.Mentor == mentorId)
                .Include(c => c.Bookings) 
                .ToList();

            return View(courses);

        }
        


        [HttpGet]
        public IActionResult CreateNewCourse()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateNewCourse(Course course, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return Content("File not selected");
            }

            // رفع الصورة
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "IMG", photo.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            course.Image = photo.FileName;

            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            course.Mentor = mentorId;

            _context.Add(course);
            _context.SaveChanges();

            return RedirectToAction("Course");
        }

        public IActionResult EditCourse(int id)
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var editCourse = _context.Course.SingleOrDefault(c => c.Id == id && c.Mentor == mentorId);

            if (editCourse == null)
            {
                return Unauthorized();
            }

            return View(editCourse);
        }

        [HttpPost]
        public IActionResult UpdateCourse(Course course, IFormFile photo)
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingCourse = _context.Course.AsNoTracking().FirstOrDefault(c => c.Id == course.Id && c.Mentor == mentorId);

            if (existingCourse == null)
            {
                return Unauthorized();
            }

            if (photo != null && photo.Length > 0)
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "IMG", photo.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                course.Image = photo.FileName;
            }

            course.Mentor = existingCourse.Mentor; 

            _context.Course.Update(course);
            _context.SaveChanges();

            return RedirectToAction("Course");
        }

        public IActionResult DeleteCourse(int id)
{
    var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var course = _context.Course
        .Include(c => c.Bookings) 
        .SingleOrDefault(c => c.Id == id && c.Mentor == mentorId);

    if (course == null)
    {
        return Unauthorized();
    }

    _context.Course.Remove(course);
    _context.SaveChanges();

    var courses = _context.Course
        .Where(c => c.Mentor == mentorId)
        .Include(c => c.Bookings) 
        .ToList();

    return PartialView("_Partial/Courses_partial", courses);
}


        public IActionResult CourseStatus()
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = _context.Course.Where(c => c.Mentor == mentorId).ToList();
            return View(courses);
        }

        //public IActionResult EditCourseStatus(int id)
        //{
        //    var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var editCourseStatus = _context.Course.SingleOrDefault(c => c.Id == id && c.Mentor == mentorId);

        //    if (editCourseStatus == null)
        //    {
        //        return Unauthorized();
        //    }

        //    return View(editCourseStatus);
        //}

        public IActionResult EditCourseStatus(int id)
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var editCourseStatus = _context.Course.SingleOrDefault(c => c.Id == id && c.Mentor == mentorId);

            if (editCourseStatus == null)
            {
                return Unauthorized();
            }

            _context.SaveChanges();

            TempData["EditSuccess"] = true; // Pass success flag to the view
            return View(editCourseStatus);
        }


        [HttpPost]
        public IActionResult UpdateCourseStatus(Course course)
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingCourse = _context.Course.SingleOrDefault(c => c.Id == course.Id && c.Mentor == mentorId);

            if (existingCourse == null)
            {
                return Unauthorized();
            }

            existingCourse.Status = course.Status;

            _context.SaveChanges();

            return RedirectToAction("CourseStatus");
        }
        


        [HttpGet]
        public IActionResult SearchCourse()
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier); // جلب معرف المعلم الحالي
            var courses = _context.Course.Where(c => c.Mentor == mentorId).ToList(); // جلب الدورات الخاصة بالمعلم
            return View(courses); // تمرير الدورات إلى العرض
        }

        [HttpPost]
        public IActionResult SearchCourse(string Name)
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier); // جلب معرف المعلم الحالي

            // إذا كان اسم الدورة غير فارغ، قم بالبحث
            if (Name != null && Name.Length > 0)
            {
                var search = _context.Course
                    .Where(c => c.Name.Contains(Name) && c.Mentor == mentorId) // البحث فقط ضمن الدورات الخاصة بالمعلم
                    .ToList();
                return PartialView("_Partial/Courses_partial", search); // عرض الدورات التي تم العثور عليها في الـ Partial View
            }

            // إذا لم يتم تقديم أي اسم، عرض جميع الدورات الخاصة بالمعلم
            var courses = _context.Course.Where(c => c.Mentor == mentorId).ToList();
            return PartialView("_Partial/Courses_partial", courses); // عرض الدورات في الـ Partial View
        }


        public IActionResult CourseDetails()
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = _context.Course.Where(c => c.Mentor == mentorId).ToList();

            // عرض قائمة الدورات في ViewData لاستخدامها في الـ View
            ViewData["Courses"] = courses;

            if (courses == null || courses.Count == 0)
            {
                return NotFound();
            }

            return View(courses); // عرض الصفحة التي تحتوي على الـ select لاختيار الدورة
        }

        public IActionResult GetCourseDetails(int courseId)
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // جلب الدورة المحددة مع المحتويات
            var course = _context.Course
                .Include(c => c.ContentItem) // تضمين المحتويات المرتبطة
                .SingleOrDefault(c => c.Id == courseId && c.Mentor == mentorId);

            if (course == null)
            {
                return NotFound(); // إذا لم يتم العثور على الدورة
            }

            // إعادة البارتشيال الخاص بتفاصيل الدورة
            return PartialView("_Partial/_CourseContent", course);
        }




        [HttpPost]
        public IActionResult AddContent(ContentItem contentItem)
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var course = _context.Course
                .Include(c => c.ContentItem)
                .SingleOrDefault(c => c.Id == contentItem.CourseId && c.Mentor == mentorId);

            if (course == null)
            {
                return Unauthorized();
            }

            // إضافة المحتوى الجديد
            _context.ContentItem.Add(contentItem);
            _context.SaveChanges();

            // إعادة عرض المحتويات فقط
            course = _context.Course
                .Include(c => c.ContentItem)
                .SingleOrDefault(c => c.Id == contentItem.CourseId);

            return PartialView("_Partial/_CourseContent", course);
        }




        [HttpPost]
        public IActionResult DeleteContent(int id)
        {
            var contentItem = _context.ContentItem.SingleOrDefault(c => c.Id == id);

            if (contentItem == null)
            {
                return NotFound();
            }

            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _context.Course.SingleOrDefault(c => c.Id == contentItem.CourseId && c.Mentor == mentorId);

            if (course == null)
            {
                return Unauthorized();
            }

            _context.ContentItem.Remove(contentItem);
            _context.SaveChanges();

            // إعادة عرض المحتويات بعد الحذف
            course = _context.Course
                .Include(c => c.ContentItem)
                .SingleOrDefault(c => c.Id == contentItem.CourseId);

            return PartialView("_Partial/_CourseContent", course);
        }


        public IActionResult EditContent(int id)
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var contentItem = _context.ContentItem
                                      .Include(c => c.Course) // تضمين الدورة لعرض تفاصيل الدورة
                                      .SingleOrDefault(c => c.Id == id);

            if (contentItem == null || contentItem.Course.Mentor != mentorId)
            {
                return Unauthorized();
            }

            return View(contentItem);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult UpdateContent(ContentItem contentItem)
        {
            var mentorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // التحقق من وجود الدورة التي ينتمي إليها المحتوى وأنها تعود للمرشد الحالي
            var course = _context.Course
                .Include(c => c.ContentItem)
                .SingleOrDefault(c => c.Id == contentItem.CourseId && c.Mentor == mentorId);

            if (course == null)
            {
                return Unauthorized();
            }

            var existingContent = _context.ContentItem.SingleOrDefault(c => c.Id == contentItem.Id);

            if (existingContent == null)
            {
                return NotFound();
            }

            existingContent.Title = contentItem.Title;
            existingContent.Description = contentItem.Description;

            _context.ContentItem.Update(existingContent);
            _context.SaveChanges();

            return RedirectToAction("CourseDetails");

        }




    }
}
