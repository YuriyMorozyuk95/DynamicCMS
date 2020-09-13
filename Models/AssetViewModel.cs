using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicCMS.Data.Models;
using Microsoft.AspNetCore.Http;

namespace DynamicCMS.Models
{
	public class AssetViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string FilePath { get; set; }
		public IFormFile Files { get; set; }
		public int ProjectId { get; set; }
	}
}
