using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

using Demo.CookieAuthBlazor.Shared;
using Demo.CookieAuthBlazor.Client.Services;

namespace Demo.CookieAuthBlazor.Client
{
    public class CustomAuthStateProvider: AuthenticationStateProvider
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<CustomAuthStateProvider> _logger;

        public CustomAuthStateProvider(ApiClient apiClient, ILogger<CustomAuthStateProvider> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public void Clear()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
        }

        public void Set(DtoUser dtoUser)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(DtoToState(dtoUser)));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var dtoUser = await _apiClient.AccountGetInfo();
                return DtoToState(dtoUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AccountGetInfo failed");
            }
                
            return new AuthenticationState(new ClaimsPrincipal());
        }

        private static AuthenticationState DtoToState(DtoUser dtoUser)
        {
            if (dtoUser?.Username == null) return new AuthenticationState(new ClaimsPrincipal());

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Sid, dtoUser.Id.ToString()),
                new Claim(ClaimTypes.Name, dtoUser.Username),
                new Claim("FullName", dtoUser.FullName)
            }, "Cookies");

            identity.AddClaims(dtoUser.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);
            return state;
        }
    }
}
