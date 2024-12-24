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
				Id = userId, // Link to the user's Id
				User = user, // Attach the retrieved user to the PayRoll
				BasicSalary = user.Salary // Populate default salary from the user
			};

			// Return the view with the PayRoll object
			return View(payRoll);
		}

		// POST: Save PayRoll
		[HttpPost]
		public IActionResult CreatePayRoll(PayRoll payRoll)
		{
			// Validate the model
			if (ModelState.IsValid)
			{
				// Retrieve the user from the database to link the payroll
				var user = _context.Users.FirstOrDefault(u => u.Id == payRoll.Id);
				if (user == null)
				{
					return NotFound();
				}

				// Link the user to the payroll
				payRoll.User = user;

				// Calculate the total salary
				payRoll.TotalSalary = payRoll.BasicSalary
									  + (payRoll.Bonus ?? 0m)
									  - (payRoll.Deductions ?? 0m);

				// Add the payroll to the database
				_context.PayRolls.Add(payRoll);
				_context.SaveChanges();

				// Redirect to the index or any other page
				return RedirectToAction("Dashboard", "Admin");
			}

			// If model validation fails, return the same view
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
				TempData["Message"] = "No payroll record found for the specified user."; // Set message
				return RedirectToAction("Dashboard", "Admin");
			}

			return View(payRoll);  // Return the payroll details view
		}

			// Action to delete a user
		public IActionResult Delete(int userId)
		{
			var user = _context.Users.FirstOrDefault(u => u.Id == userId);
			if (user != null)
			{
				// Remove the user from the database
				_context.Users.Remove(user);
				_context.SaveChanges();  // Save changes to the database

				// Set a success message to be displayed in the view
				TempData["SuccessMessage"] = "User deleted successfully!";
			}
			else
			{
				// If user not found, set an error message
				TempData["ErrorMessage"] = "User not found!";
			}

			// Redirect to the Dashboard after deletion
			return RedirectToAction("Dashboard");  // Redirect to the correct dashboard action
		}

	}
}
