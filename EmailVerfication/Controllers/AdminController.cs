using EmailVerfication.Data;
using EmailVerification.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailVerification.Controllers
{
	public class AdminController : Controller
	{
		// Injecting the ApplicationDbContext to access the database
		private readonly AppDbContext _context;

		public AdminController(AppDbContext context)
		{
			_context = context;
		}

		// Action for the Dashboard
		public IActionResult Dashboard()
		{
			// Fetch all users from the database
			var users = _context.Users.ToList();  // Retrieves all users from the Users table

			// Pass the users list to the view
			return View(users);
		}

		// GET: Create PayRoll for a specific user
		[HttpGet]
		public IActionResult CreatePayRoll(int userId)
		{
			// Retrieve the user from the database using userId
			var user = _context.Users.FirstOrDefault(u => u.Id == userId);
			if (user == null)
			{
				// Return NotFound if the user doesn't exist
				return NotFound();
			}

			// Initialize a new PayRoll object with the user's data
			var payRoll = new PayRoll
			{
				User = user, // Attach the retrieved user to the PayRoll
				Id = userId
			};

			// Return the view with the PayRoll object
			return View(payRoll);
		}

		[HttpPost]
		public IActionResult CreatePayRoll(PayRoll payRoll)
		{
			// Check if the model is valid (ensures all required fields are filled)
			if (ModelState.IsValid)
			{
				// Ensure all fields are converted to decimal for calculation
				decimal basicSalary = payRoll.BasicSalary; // 0m is decimal zero
				decimal bonus = payRoll.Bonus ?? 0m; // Use 0 if Bonus is null
				decimal deductions = payRoll.Deductions ?? 0m; // Use 0 if Deductions is null

				// Calculate the TotalSalary: basicSalary + bonus - deductions
				payRoll.TotalSalary = basicSalary + bonus - deductions;

				// Attach the userId to ensure we link the payroll to the correct user
				var user = _context.Users.FirstOrDefault(u => u.Id == payRoll.User.Id);
				if (user == null)
				{
					// Return NotFound if the user does not exist in the database
					return NotFound();
				}

				// Save the payroll data to the database
				_context.PayRolls.Add(payRoll);
				_context.SaveChanges(); // Persist the changes to the database

				// Redirect to the Admin Index page after saving
				return RedirectToAction("Index", "Admin");
			}

			// If the model is invalid, return the view with validation errors
			return View(payRoll);
		}



		// Action to display PayRoll details for the user
		public IActionResult ViewPayRollDetails(int userId)
		{
			var payRoll = _context.PayRolls
				.Include(p => p.User)  // Make sure to load the User details
				.FirstOrDefault(p => p.User.Id == userId);

			if (payRoll == null)
			{
				return NotFound();  // Return 404 if no payroll record is found
			}

			return View(payRoll);  // Return the payroll details view
		}

		// Optional: Delete action for user
		public IActionResult Delete(int userId)
		{
			var user = _context.Users.FirstOrDefault(u => u.Id == userId);
			if (user == null)
			{
				return NotFound(); // If user is not found
			}

			_context.Users.Remove(user);
			_context.SaveChanges();

			return RedirectToAction("Index"); // Redirect to the user list after deletion
		}
	}
}
