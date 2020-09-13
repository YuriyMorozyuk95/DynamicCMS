using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DynamicCMS.Data.Models
{
	public class ApplicationUser: IdentityUser
	{
		public ApplicationUser()
		{
			Projects = new List<Project>();
		}
		public List<Project> Projects { get; set; }
	}
}
