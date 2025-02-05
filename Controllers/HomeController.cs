using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillBridage.Data;
using SkillBridage.Models;

namespace SkillBridage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;  

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;  
        }


        public IActionResult Index()
        {
            var courses = _context.Course.Where(c => c.Status == CourseStatus.Active).ToList();
            return View(courses);
        }

        [HttpGet]
        public IActionResult Course()
        {
            var course = _context.Course.ToList();
            return View(course);
        }

        
        [HttpPost]
        public IActionResult Course(string Name)
        {
               
        if(Name != null && Name.Length > 0)
        {
            var search = _context.Course.Where(c => c.Name.Contains(Name)).ToList();
            return PartialView("_Partial/_courseParial", search);
        }
        var Course = _context.Course.ToList();
        return PartialView("_Partial/_courseParial", Course);
        }   

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
