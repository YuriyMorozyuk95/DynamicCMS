using DynamicCMS.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DynamicCMS.Data
{
	public class ApplicationDbContext  : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Asset> Assets { get; set; }
		public DbSet<Project> Projects { get; set; }
	}
}
