using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockSavvy.Services;

namespace StockSavvy.Pages
{
    public class HomeModel : PageModel
    {
        private readonly ILogger<HomeModel> _logger;

        public HomeModel(ILogger<HomeModel> logger)
        {
            _logger = logger;
        }
        public IActionResult OnPostLogoutRequest(UserService userService)
        {
            if (HttpContext.Request.Cookies.Count > 0)
            {
                var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication")||c.Key.Contains("username"));
                foreach (var cookie in siteCookies)
                {
                    Response.Cookies.Delete(cookie.Key);
                }
            }

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");

        }



        [Authorize()]
        public void OnGet()
        {
        }
    }
}
