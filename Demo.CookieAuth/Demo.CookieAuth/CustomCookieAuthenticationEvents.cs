﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Demo.CookieAuth
{
    public class CustomCookieAuthenticationEvents: CookieAuthenticationEvents
    {
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var lastCheckOnClaim = context.Principal
                .Claims
                .FirstOrDefault(c => c.Type == "LastCheckOn")?
                .Value;

            if (string.IsNullOrEmpty(lastCheckOnClaim) || !CheckUser(context, lastCheckOnClaim))
            {
                // ***************************************************************************
                // DEMO: If the user information changed in the database since it was created,
                // reject the principal, and sign-out the user
                // ***************************************************************************

                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        private bool CheckUser(CookieValidatePrincipalContext context, string claim)
        {
            var lastCheckOn = DateTime.Parse(claim, null, DateTimeStyles.RoundtripKind);

            // This is to avoid hitting database on every click
            if (lastCheckOn.AddMinutes(5) > DateTime.UtcNow) return true;

            // if 5 minutes has passed since the last check, pull the user from DB, check that the user is still active
            // TODO:
            // - load user from DB
            // - if the user is blocked, or expired, return false

            // DEMO: For the demo, we're assuming the user is still valid, with the same roles,
            // so we're replacing the principal with the same claims, except "LastCheckOn" which is refreshed with new date.

            var claims = context.Principal.Claims.Where(c => c.Type != "LastCheckOn").ToList();

            claims.Add(new Claim("LastCheckOn", $"{DateTime.UtcNow:o}"));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            context.ReplacePrincipal(new ClaimsPrincipal(claimsIdentity));
            context.ShouldRenew = true;

            return true;
        }
    }
}
