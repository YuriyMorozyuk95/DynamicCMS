using System.Collections.Generic;

namespace DynamicCMS.Data.Models
{
	public class Project : BaseModel
	{
		public Project()
		{
			UnityAssets = new List<Asset>();
		}
		public string Name { get; set; }

		public ApplicationUser User { get; set; }
		public string UserId { get; set; }

		public List<Asset> UnityAssets { get; set; }
	}
}