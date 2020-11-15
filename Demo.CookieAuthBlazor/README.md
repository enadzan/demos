# Cookie Authentication in Blazor WebAssembly

The solution relies on SameSite=Strict cookies to implement cookie authentication without OAuth, Identity or tokens. Before using this, make sure your clients' browsers [support SameSite settings for cookies](https://caniuse.com/mdn-http_headers_set-cookie_samesite_strict).

Blazor WebAssembly customizations are done based on the documentation found here: [ASP.NET Core Blazor authentication and authorization](https://docs.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-3.1).
