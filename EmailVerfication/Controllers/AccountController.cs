using EmailVerfication.Data;
using EmailVerfication.Models;
using EmailVerfication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmailVerfication.Controllers
{
	public class AccountController : Controller
	{
		private readonly AppDbContext _dbcontext;
		private readonly EmailService _emailService;

		public AccountController(AppDbContext dbcontext, EmailService emailService)
		{
			_dbcontext = dbcontext;
			_emailService = emailService;
		}

		//Signup GET
		public IActionResult Index()
		{
			return View();
		}

		// Signup GET
		public IActionResult Signup()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Signup(string FullName, string Email, string Password)
		{
			if (ModelState.IsValid)
			{
				// Check if the email is already registered
				var existingUser = _dbcontext.Users.FirstOrDefault(u => u.Email == Email);
				if (existingUser != null)
				{
					ModelState.AddModelError(string.Empty, "Email is already registered.");
					return View();
				}

				// Generate a verification token
				var token = Guid.NewGuid().ToString();

				// Create new user object
				var user = new User
				{
					FullName = FullName,
					Email = Email,
					Password = Password,
					VerificationToken = token,
					IsVerified = false
				};

				// Add user to the database
				_dbcontext.Users.Add(user);
				await _dbcontext.SaveChangesAsync();

				// Generate the verification link
				var verificationLink = Url.Action("VerifyEmail", "Account", new { token }, Request.Scheme);

				// Create the email body
				var emailBody = $"<p>Hi {FullName},</p><p>Please verify your email by clicking <a href='{verificationLink}'>here</a>.</p>";

				// Send verification email
				await _emailService.SendEmailAsync(Email, "Verify Your Email", emailBody);

				return View("VerificationEmailSent");
			}
			return View();
		}

		//Email Verification
		public async Task<IActionResult> VerifyEmail(string token)
		{
			var user = _dbcontext.Users.FirstOrDefault(u => u.VerificationToken == token);
			if (user == null)
			{
				return View("VerificationFailed");
			}

			// Update the user's verification status
			user.IsVerified = true;
			user.VerificationToken = null;
			await _dbcontext.SaveChangesAsync();

			return View("VerificationSuccess");
		}

		// Login GET
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(string Email, string Password)
		{
			var user = _dbcontext.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);

			if (user == null || !user.IsVerified)
			{
				ModelState.AddModelError(string.Empty, "Invalid login attempt or email not verified.");
				return View();
			}

			// Check if the email exists and store it in session
			if (!string.IsNullOrEmpty(user.Email))
			{
				HttpContext.Session.SetString("UserEmail", user.Email); // Store email in session
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Email is not valid.");
				return View();
			}

			// Redirect to the Profile page (or dashboard)
			return RedirectToAction("Profile", "Account");
		}


		public IActionResult Profile()
		{
			var userEmail = HttpContext.Session.GetString("UserEmail");
			if (string.IsNullOrEmpty(userEmail))
			{
				return RedirectToAction("Login");
			}

			var user = _dbcontext.Users.FirstOrDefault(u => u.Email == userEmail);

			if (user == null)
			{
				return RedirectToAction("Login");
			}

			return View(user);
		}

		// Logout action
		public IActionResult Logout()
		{
			// Clear the session
			HttpContext.Session.Clear();

			// Redirect to the login page (or home page)
			return RedirectToAction("Login", "Account");
		}

	}
}
