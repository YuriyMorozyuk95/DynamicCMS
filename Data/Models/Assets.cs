namespace DynamicCMS.Data.Models
{
	public class Asset : BaseModel
	{
		public string Name { get; set; }
		public string FilePath { get; set; }

		public Project Project { get; set; }
		public int ProjectId { get; set; }
	}
}
