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

	}
}
