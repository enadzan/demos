using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Demo.CookieAuthBlazor.Shared;

namespace Demo.CookieAuthBlazor.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [AllowAnonymous]
        [Route("sign-in")]
        [HttpPost]
        public async Task<IActionResult> SignIn([Required]DtoSignIn dtoSignIn)
        {
            // TODO: use any method to check the credentials (for demo, it's hard-coded to guest/guest)

            if (dtoSignIn == null || !(dtoSignIn.Username == "guest" && dtoSignIn.Password == "guest"))
            {
                ModelState.AddModelError("", "Incorrect username or password");
                return BadRequest(ModelState);
            }

            // TODO: create dtoUser based on information pulled from a DB (for demo, it's hard-coded)

            var dtoUser = new DtoUser
            {
                Id = 123, // some made up ID
                Username = "guest",
                FullName = "Guest user",
                Roles = new List<string>
                {
                    "dev_role",
                    "qa_role"
                }
            };

            await CreateCookieAsync(dtoUser, dtoSignIn.RememberMe);

            // Claims principal is stored in the AuthenticationTicket in the cookie, and cannot be read by Blazor client.
            // Blazor client will recreate ClaimsPrincipal based on the DtoUser on the client side (for authorization-aware UI).
            return Ok(dtoUser);
        }

        /// <summary>
        /// This is used by Blazor client on first load. If the user is authenticated (cookie exists),
        /// then user data will be returned and Blazor client will create ClaimsPrincipal on client, without
        /// requiring the user to log in again.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("info")]
        [HttpGet]
        public IActionResult Info()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false)) return Ok(new DtoUser());

            return Ok(new DtoUser
            {
                Id = int.Parse(User.Claims.Single(c => c.Type == ClaimTypes.Sid).Value),
                Username = User.Identity.Name,
                FullName = User.Identity.Name,
                Roles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList()
            });
        }

        [Route("sign-out")]
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        /// <summary>
        /// Creates claims for the user, including role claims and signs in the user
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="persistent"></param>
        /// <returns></returns>
        private Task CreateCookieAsync(DtoUser dto, bool persistent)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, dto.Id.ToString()),
                new Claim(ClaimTypes.Name, dto.Username),
                new Claim("FullName", dto.FullName),
                new Claim("LastCheckOn", $"{DateTime.UtcNow:o}")    // use to detect if the user has changed (blocked etc)
            };

            // Add roles from dto
            claims.AddRange(dto.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties authProperties = null;

            if (persistent)
            {
                // **********************************************************************************************
                // If RememberMe is true, this will create a persistent cookie (persists across browser restarts)
                //
                // Otherwise, if RememberMe is false, this will be skipped and a "session" cookie is created
                // Session cookie is deletes when ALL instances of the browser are closed.
                // **********************************************************************************************

                authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
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
