using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DynamicCMS.Data;
using DynamicCMS.Data.Models;
using DynamicCMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace DynamicCMS.Controllers
{
    public class AssetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetsController(ApplicationDbContext context)
        {
	        _context = context;
        }
        private string GetContentType(string path)  
        {  
	        var types = GetMimeTypes();  
	        var ext = Path.GetExtension(path).ToLowerInvariant();  
	        return types[ext];  
        }  
   
        private Dictionary<string, string> GetMimeTypes()  
        {  
	        return new Dictionary<string, string>  
	        {  
		        {".txt", "text/plain"},  
		        {".pdf", "application/pdf"},  
		        {".doc", "application/vnd.ms-word"},  
		        {".docx", "application/vnd.ms-word"},  
		        {".xls", "application/vnd.ms-excel"},  
		        {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},  
		        {".png", "image/png"},  
		        {".jpg", "image/jpeg"},  
		        {".jpeg", "image/jpeg"},  
		        {".gif", "image/gif"},  
		        {".csv", "text/csv"}  
	        };  
	        }
        public async Task<IActionResult> Download(string id)  
        {  
	        //if (filename == null)  
		       // return Content("filename not present");  
  
	        //var path = Path.Combine(  
		       // Directory.GetCurrentDirectory(),  
		       // "wwwroot", filename);  
		      var filePath = id;
	          var memory = new MemoryStream();  
	          using (var stream = new FileStream(filePath, FileMode.Open))  
	          {  
		          await stream.CopyToAsync(memory);  
	          }  
	          memory.Position = 0;  
	          return File(memory, GetContentType(filePath), Path.GetFileName(filePath));  
        } 

        // GET: Assets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Assets.Include(a => a.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Assets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .Include(a => a.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // GET: Assets/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", nameof(Project.Name));
            return View();
        }

        // POST: Assets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,FilePath,ProjectId,Id")] Asset asset)
        {
            if (ModelState.IsValid)
            {
	            _context.Add(asset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Upload), new { id = asset.Id });
            }

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", nameof(Project.Name), asset.ProjectId);
            return View(asset);
        }

        private Asset UploadFile(AssetViewModel model)
        {
	        var files = model.Files;
	        string filepath = string.Empty;

	        if (files != null)
	        {
		        if (files.Length > 0)
		        {
			        //Getting FileName
			        var fileName = Path.GetFileName(files.FileName);

			        //Assigning Unique Filename (Guid)
			        var myUniqueFileName = model.Name;

			        //Getting file Extension
			        var fileExtension = Path.GetExtension(fileName);

			        // concatenating  FileName + FileExtension
			        var newFileName = String.Concat(myUniqueFileName, fileExtension);

			        // Combines two strings into a path.
			        filepath =
				        new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets")).Root + $@"\{newFileName}";

			        using (FileStream fs = System.IO.File.Create(filepath))
			        {
				        files.CopyTo(fs);
				        fs.Flush();
			        }
		        }
	        }

			var asset = new Asset()
			{
				Id = model.Id,
				FilePath = filepath,
				Name = model.Name,
				ProjectId = model.ProjectId
			};
			return asset;
		}

        // GET: Assets/Edit/5
        public async Task<IActionResult> Upload(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }

            var assetViewModel = new AssetViewModel()
            {
	            Id = asset.Id,
	            FilePath = asset.FilePath,
	            Name = asset.Name,
	            ProjectId = asset.ProjectId
            };

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", nameof(Project.Name), assetViewModel.ProjectId);
            return View(assetViewModel);
        }

		//POST: Assets/Edit/5
  //       To protect from overposting attacks, enable the specific properties you want to bind to, for 
  //       more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Upload(int id, [Bind("Name,FilePath,Files,ProjectId,Id")] AssetViewModel model)
		{
			if (id != model.Id)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", nameof(Project.Name), model.ProjectId);
				return View(model);
			}

			try
			{
                RemvoeFileIfExist(model.FilePath);
                var asset = UploadFile(model);

				_context.Update(asset);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!AssetExists(model.Id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}

		// GET: Assets/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .Include(a => a.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Assets.FindAsync(id);

            RemvoeFileIfExist(asset.FilePath);

            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private static void RemvoeFileIfExist(string filePath)
        {
	        if (System.IO.File.Exists(filePath))
	        {
		        System.IO.File.Delete(filePath);
	        }
        }

        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.Id == id);
        }
    }
}
