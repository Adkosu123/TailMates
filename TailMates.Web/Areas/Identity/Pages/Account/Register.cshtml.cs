// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using TailMates.Data.Models; // Keeping TailMates.Data.Models as requested

namespace TailMates.Web.Areas.Identity.Pages.Account // Keeping TailMates.Web as requested
{
	[AllowAnonymous] // Ensures this page is accessible without being logged in
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly IUserEmailStore<ApplicationUser> _emailStore;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;
		private readonly RoleManager<IdentityRole> _roleManager; // NEW: Added RoleManager

		// Consolidated Constructor: All dependencies are injected here
		public RegisterModel(
			UserManager<ApplicationUser> userManager,
			IUserStore<ApplicationUser> userStore,
			SignInManager<ApplicationUser> signInManager,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender,
			RoleManager<IdentityRole> roleManager) // NEW: RoleManager injected directly
		{
			_userManager = userManager;
			_userStore = userStore;
			_emailStore = GetEmailStore();
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
			_roleManager = roleManager; // Assign RoleManager
		}

		/// <summary>
		/// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		/// directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
		public InputModel Input { get; set; } = new InputModel(); // Initialize to avoid null reference

		/// <summary>
		/// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		/// directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string ReturnUrl { get; set; } = string.Empty; // Initialize

		/// <summary>
		/// This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		/// directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>(); // Initialize

		/// <summary>
		/// InputModel for the registration form, including FirstName and LastName.
		/// </summary>
		public class InputModel
		{
			[Required]
			[EmailAddress]
			[Display(Name = "Email")]
			public string Email { get; set; } = string.Empty;

			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string Password { get; set; } = string.Empty;

			[DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; } = string.Empty;

			// NEW: FirstName and LastName for the ApplicationUser
			[Required]
			[Display(Name = "First Name")]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
			public string FirstName { get; set; } = string.Empty;

			[Required]
			[Display(Name = "Last Name")]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
			public string LastName { get; set; } = string.Empty;
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			// Redirect authenticated users from the register page
			if (User.Identity?.IsAuthenticated == true)
			{
				Response.Redirect("/");
			}

			ReturnUrl = returnUrl ?? Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				var user = CreateUser();

				// Assign FirstName and LastName from InputModel to the ApplicationUser
				user.FirstName = Input.FirstName;
				user.LastName = Input.LastName;

				await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
				var result = await _userManager.CreateAsync(user, Input.Password);

				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					// NEW: Assign the "User" role to the newly registered user
					// It's good practice to ensure the role exists, though it should be seeded on startup.
					if (!await _roleManager.RoleExistsAsync("User"))
					{
						_logger.LogError("The 'User' role does not exist. Please ensure ApplicationDbInitializer has run.");
						// Optionally, add a model error to inform the user or administrator
						ModelState.AddModelError(string.Empty, "Registration failed: The 'User' role is not configured. Please contact support.");
						return Page();
					}

					var roleResult = await _userManager.AddToRoleAsync(user, "User");
					if (roleResult.Succeeded)
					{
						_logger.LogInformation("User '{Email}' assigned to 'User' role.", user.Email);
					}
					else
					{
						_logger.LogError("Failed to assign 'User' role to user '{Email}'. Errors: {Errors}", user.Email, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
						foreach (var error in roleResult.Errors)
						{
							ModelState.AddModelError(string.Empty, $"Role assignment failed: {error.Description}");
						}
						// If role assignment fails, you might want to delete the user or handle it specifically
						// For now, we'll let the user proceed but log the error.
					}

					var userId = await _userManager.GetUserIdAsync(user);
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
						protocol: Request.Scheme);

					await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
						$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
					}
					else
					{
						await _signInManager.SignInAsync(user, isPersistent: false);
						return LocalRedirect(returnUrl);
					}
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}

		private ApplicationUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<ApplicationUser>();
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
					$"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
					$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}

		private IUserEmailStore<ApplicationUser> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<ApplicationUser>)_userStore;
		}
	}
}
