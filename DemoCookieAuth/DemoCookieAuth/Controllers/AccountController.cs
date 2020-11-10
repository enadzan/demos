using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using DemoCookieAuth.Models;

namespace DemoCookieAuth.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(SignInModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // **********************************************************************************
                // DEMO: Dummy authentication - use whatever method you want to check the credentials
                // **********************************************************************************
                if (model.Username == "guest" && model.Password == "guest")
                {
                    await CreateCookieAsync(model);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("Password", "Invalid username or password");
            }

            // login failed
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Creates claims for the user, including role claims and signs in the user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Task CreateCookieAsync(SignInModel model)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, model.Username),   // probably better to put user ID here
                new Claim(ClaimTypes.Name, model.Username),
                new Claim("FullName", "Guest user")          // use real name of your user
            };

            // ******************************
            // DEMO: Add dummy roles
            // ******************************
            claims.Add(new Claim(ClaimTypes.Role, "qa_role"));
            claims.Add(new Claim(ClaimTypes.Role, "dev_role"));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties authProperties = null;

            if (model.RememberMe)
            {
                // **********************************************************************************************
                // If RememberMe is true, this will create a persistent cookie (persists across browser restarts)
                //
                // Otherwise, if RememberMe is false, this will be skipped and a "session" cookie is created
                // Session cookie is deletes when ALL instances of the browser are closed.
                // **********************************************************************************************

                authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                    IsPersistent = true
                };
            }

            return HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
