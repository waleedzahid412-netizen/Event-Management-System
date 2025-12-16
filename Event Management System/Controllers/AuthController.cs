using EventManagement.Data;
using Microsoft.AspNetCore.Mvc;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Controllers
{
    public class AuthController : Controller
    {
        private ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
      
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (!ModelState.IsValid) {
                return View(model);
            }
            if (_context.Users.Any(u => u.FullName.ToLower().Equals(model.UserName.ToLower()))) {
                ViewBag.ErrorMessage = "Username already exist! pls try another username";
                return View(model);
            }
            var user = new User {
                FullName = model.UserName,
                Email = model.Email,
                Password=BCrypt.Net.BCrypt.HashPassword(model.Password),
                age=model.age



            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
                
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (!ModelState.IsValid) {
                return View();
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName.ToLower().Equals(model.FullName));
            if (user == null) {
                ModelState.AddModelError("FullName", "Username not found");
                return View(model);
            }
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password)) {
                ModelState.AddModelError("Password", "Invalid password");
                return View(model);
            }
            return RedirectToAction("Index", "Home");

        }


    }
}
