using System.ComponentModel.DataAnnotations;

namespace EmailVerfication.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string? FullName { get; set; }

		[Required, EmailAddress]
		public string? Email { get; set; }

		[Required]
		public required string Password { get; set; }

		public bool IsVerified { get; set; }

		public string? VerificationToken { get; set; }

		[Required]
		public string Position { get; set; }

		[Required]
		public decimal Salary { get; set; }

		public DateTime DateOfJoining { get; set; }

		public string? PhoneNumber { get; set; }

		public string? Address { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public string? BankAccountNumber { get; set; }
		// Added Role with default value "Employee"
		[Required]
		public string Role { get; set; } = "Employee";

	}
}
