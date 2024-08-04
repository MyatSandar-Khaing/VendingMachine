using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using System;
using VendingMachine.Dto;
using VendingMachine.Models;
using VendingMachine.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace VendingMachine.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDBContext _context;
     
        public AccountController(ApplicationDBContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _context.Accounts.Where(x => x.Email == model.Email && x.Password == model.Password).SingleOrDefaultAsync();
                if (result != null)
                {
                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, result.ID.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, result.Name));
                    claims.Add(new Claim(ClaimTypes.Role, result.Role));

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPriciple = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPriciple);
                    return RedirectToAction("Index", "Product");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            ViewData["Error"] = "Invalid user name or password.";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            //await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }

}
