using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using Demo.CookieAuthBlazor.Shared;

namespace Demo.CookieAuthBlazor.Client.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigation;

        public ApiClient(HttpClient httpClient, NavigationManager navigation)
        {
            _httpClient = httpClient;
            _navigation = navigation;
        }

        public async Task<DtoUser> AccountGetInfo()
        {
            return await GetAsync("account/info", new DtoUser());
        }

        public async Task<DtoUser> AccountSignIn(DtoSignIn dtoSignIn)
        {
            return await PostAsync("account/sign-in", dtoSignIn, new DtoUser());
        }

        public async Task AccountSignOut()
        {
            await PostAsync("account/sign-out", new {});
        }

        public async Task<WeatherForecast[]> GetWeatherForecast()
        {
            return await GetAsync("WeatherForecast", new WeatherForecast[0]);
        }

        private async Task<T> GetAsync<T>(string requestUri, T defaultResult)
        {
            var response = await _httpClient.GetAsync(requestUri);
            return await ReadFromJsonAsync(response, defaultResult);
        }

        private async Task<TResult> PostAsync<TValue, TResult>(string requestUri, TValue value, TResult defaultResult)
        {
            var response = await _httpClient.PostAsJsonAsync(requestUri, value);
            return await ReadFromJsonAsync(response, defaultResult);
        }

        private async Task PostAsync<TValue>(string requestUri, TValue value)
        {
            var response = await _httpClient.PostAsJsonAsync(requestUri, value);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigation.NavigateTo("sign-in");
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }
        }

        private async Task<T> ReadFromJsonAsync<T>(HttpResponseMessage response, T defaultResult)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigation.NavigateTo("sign-in");
                return defaultResult;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var modelErrors = await response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>();
                throw new ModelValidationException(modelErrors);
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
