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
		public IActionResult Signup(User user)
		{
			if (user == null)
			{
				// Handle the case where the user is null, which should not normally happen
				return View();
			}

			if (ModelState.IsValid)
			{
				// Process the valid user data, e.g., save to database
				return RedirectToAction("Success"); // Redirect on successful signup
			}

			return View(user);
		}

		[HttpPost]
		public async Task<IActionResult> Signup(string FullName, string Email, string Password, string Position, decimal? Salary, DateTime? DateOfJoining, string? Address = null,
		string? PhoneNumber = null,
		string? BankAccountNumber = null,
		DateTime? DateOfBirth = null
		)
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

				// Assign default values if Salary or DateOfJoining are not provided
				Salary = Salary ?? 0.00m; // Set default salary if not provided
				DateOfJoining = DateOfJoining ?? DateTime.Now; // Set current date if not provided
															   // Assign default values if fields are not provided
				Address = Address ?? string.Empty; // Default to empty string
				PhoneNumber = PhoneNumber ?? string.Empty; // Default to empty string
				BankAccountNumber = BankAccountNumber ?? string.Empty; // Default to empty string
				DateOfBirth = DateOfBirth ?? DateTime.MinValue;
				// Create new user object
				var user = new User
				{
					FullName = FullName,
					Email = Email,
					Password = Password,
					Position = Position,
					Address = Address,
					Salary = Salary.Value, // Use the default or provided value
					DateOfJoining = DateOfJoining.Value, // Use the default or provided value
					VerificationToken = token,
					IsVerified = false,
					PhoneNumber = PhoneNumber,
					BankAccountNumber = BankAccountNumber,
					DateOfBirth = DateOfBirth,
					Role = "Employee",
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
		// GET: Account/UpdateProfile
		public IActionResult UpdateProfile()
		{
			// Get the current user's email from the session
			var userEmail = HttpContext.Session.GetString("UserEmail");
			if (string.IsNullOrEmpty(userEmail))
			{
				return RedirectToAction("Login");
			}

			// Fetch the user details from the database
			var user = _dbcontext.Users.FirstOrDefault(u => u.Email == userEmail);
			if (user == null)
			{
				return RedirectToAction("Login");
			}

			// Return the view with the user data
			return View(user);
		}

		// POST: Account/UpdateProfile
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateProfile(string FullName, string Email, string Position, decimal? Salary, DateTime? DateOfJoining,
			string? Address, string? PhoneNumber, string? BankAccountNumber, DateTime? DateOfBirth)
		{
			// Check if the model state is valid
			if (ModelState.IsValid)
			{
				// Get the current user's email from the session
				var userEmail = HttpContext.Session.GetString("UserEmail");
				if (string.IsNullOrEmpty(userEmail))
				{
					return RedirectToAction("Login");
				}

				// Fetch the user from the database
				var user = _dbcontext.Users.FirstOrDefault(u => u.Email == userEmail);
				if (user == null)
				{
					return RedirectToAction("Login");
				}

				// Update the user's profile
				user.FullName = FullName ?? user.FullName;
				user.Email = Email ?? user.Email;
				user.Position = Position ?? user.Position;
				user.Salary = Salary ?? user.Salary;
				user.DateOfJoining = DateOfJoining ?? user.DateOfJoining;
				user.Address = Address ?? user.Address;
				user.PhoneNumber = PhoneNumber ?? user.PhoneNumber;
				user.BankAccountNumber = BankAccountNumber ?? user.BankAccountNumber;
				user.DateOfBirth = DateOfBirth ?? user.DateOfBirth;

				// Save changes to the database
				await _dbcontext.SaveChangesAsync();

				// Optionally, display a success message and redirect to the profile page
				TempData["SuccessMessage"] = "Profile updated successfully!";
				return RedirectToAction("Profile");
			}

			// If the model state is invalid, return the form with validation errors
			return View();
		}

	}
}
