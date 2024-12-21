using EmailVerfication.Models;
using EmailVerification.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailVerfication.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		public DbSet<User> Users { get; set; }
		public DbSet<PayRoll> PayRolls { get; set; }

	}
}
