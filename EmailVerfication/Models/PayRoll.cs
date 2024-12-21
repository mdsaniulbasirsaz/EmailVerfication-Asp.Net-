using EmailVerfication.Models;
using System.ComponentModel.DataAnnotations;

namespace EmailVerification.Models
{
	public class PayRoll
	{
		[Key]
		public int PayrollId { get; set; } // Changed to PayrollId

		// Foreign key to the User model
		public int Id { get; set; }  // This should correspond to the User's Id

		// Navigation property to the User model
		public User? User { get; set; }

		[Required]
		public decimal BasicSalary { get; set; }

		public decimal? Bonus { get; set; }

		public decimal? Deductions { get; set; }

		[Required]
		public decimal TotalSalary { get; set; }

		[Required]
		public DateTime? PayDate { get; set; }

		public string? Remarks { get; set; }

		// Optional: You can add a field for pay status (e.g., Paid, Pending)
		public string PayStatus { get; set; } = "Pending";
	}
}
