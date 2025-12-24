using Event_Management_System.DTOs;
using Event_Management_System.Interfaces;
using EventManagement.Configuration;
using EventManagement.Data;
using EventManagement.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Event_Management_System.Controllers
{
    public class AuthController : Controller
    {
    private readonly IAuthService _authService;

        public AuthController(IAuthService service)
        {
            _authService = service;
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
        public async Task<IActionResult> Register(RegisterViewDTO model)
        {

            if (!ModelState.IsValid) {
                return View(model);
            }
            try { 
            await _authService.RegisterAsync(model);
            return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewDTO dto) {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var result = await _authService.LoginAsync(dto);

                Response.Cookies.Append("jwt_token", result.Token);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }

        }
        public IActionResult Logout() {
            Response.Cookies.Delete("jwt_token");
            return RedirectToAction("Login", "Auth");
        }




    }
}
