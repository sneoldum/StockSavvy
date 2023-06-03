﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockSavvy.Models;
using StockSavvy.Services;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace StockSavvy.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;


        [BindProperty]
        public string UserName { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }
        [BindProperty]
        public string UserLoginEx { get; set; }
        [BindProperty]
        public string UserRegisterEx { get; set; }

        private readonly IHttpContextAccessor _contextAccessor;

        public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor contextAccessor )
        {
            _logger = logger;
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));

        }

        public IActionResult OnPostLoginRequest(UserService userService)
        {
            if (UserName != null)
            {
                var userToCheck = userService.GetOneByUsername(UserName);

                

                //string s = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                if (userToCheck != null)
                {
                    if (HashingHelper.VerifyPasswordHash(Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
                    {
                        var claims = new List<Claim>() {
                            new Claim(ClaimTypes.NameIdentifier, Convert.ToString(userToCheck.Id))
                        };
                        var identity = new ClaimsIdentity(claims, "site");

                        _contextAccessor.HttpContext!.SignInAsync(new GenericPrincipal(identity, null)).Wait();



                        return RedirectToPage("/Home");
                    }
                    else
                    {
                        UserLoginEx = "Please check your password or Username!";
                    }
                }
                else
                {
                    UserLoginEx = "User not exists!";
                }
            }
            return Page();

        }

        public void OnPostRegisterRequest(UserService userService)
        {
            if (UserName != null)
            {
                //var userToCheck = userService.GetOneByUsername(UserName);

                //if (userToCheck !=null)
                //{
                //    UserRegisterEx = "User already exists!";
                //}
                //else
                //{

                    byte[] userHash, userSalt;

                HashingHelper.CreatePasswordHash(Password, out userHash, out userSalt);


                //string userHashStr = System.Text.Encoding.UTF8.GetString(userHash, 0, userHash.Length);
                //string userSaltStr = System.Text.Encoding.UTF8.GetString(userSalt, 0, userSalt.Length);


                var user = new UserMongoModel
                {
                    Username = UserName,
                    FirstName = FirstName,
                    LastName = LastName,
                    PasswordHash = userHash,
                    PasswordSalt = userSalt,
                    Status = true
                };

                //var user = new UserMongoModel();
                //user.PasswordHash = Password;
                //user.FirstName=FirstName; 
                //user.LastName=LastName;
                //user.Username = UserName;
                //user.PasswordSalt = Password;
                //user.Status = true;

                userService.Create(user);
                    UserRegisterEx = "User registered successfully!";
                //}

            }
        }
        public void OnGet()
        {

        }
    }
}