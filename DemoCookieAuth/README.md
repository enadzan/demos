# Simple Cookie Authentication in ASP.NET Core

This is a demonstration of role-based authorization with cookies, without using ASP.NET Identity. This demo is created based on the documentation available here: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1

## How to read the source

### 1. Open the solution in Visual Studio 2019

Build and run to see if everything is OK. Try the "Test Auth" link from the top menu. It will redirect you to the login form. Use the following credentials to log in:

- Username: `guest`
- Password: `guest`

There is no database in the project, these are hard-coded. This is an "empty" MVC project, with cookie authentication wiring added.

### 2. Read "Startup.cs"

Go ahead and check out [Startup.cs](DemoCookieAuth\Startup.cs). Note the comments marked with "DEMO".

### 3. Read "AccountController.cs"

Now read the account controller in [AccountController.cs](DemoCookieAuth\Controllers\AccountController.cs). Also, note the comments.

### 4. Finally take a look at the "CustomCookieAuthenticationEvents.cs"

This [CustomCookieAuthenticationEvents.cs](DemoCookieAuth\CustomCookieAuthenticationEvents.cs) class demonstrates principal verification that is performed on every request. This demo skips the check if the last check was within 5 minutes, but that is optional. Here it is used to demonstrate a way to avoid hitting the DB on every request.

### 5. Fork and make pull requests if you see something problematic :)
