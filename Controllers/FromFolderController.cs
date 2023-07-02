using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestGFL.Models;

namespace TestGFL.Controllers
{
    public class FromFolderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FromFolderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult ImportFromFolder(string path)
        {
            string rootDirectory = path;
            try
            {
                if (!Directory.Exists(rootDirectory))
                {
                    var errorMessage = "Invalid folder path. Please enter a valid path.";
                    return RedirectToAction("Error", new { errorMessage });
                }

                _context.Database.ExecuteSqlRaw("DELETE FROM Catalogs");
                ImportCatalogs(null, rootDirectory);
                return RedirectToAction("Index", "Catalog");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Catalog");
            }
        }

        private int _nextId = 1;

        private void ImportCatalogs(int? parentId, string directoryPath)
        {
            string[] subdirectories = Directory.GetDirectories(directoryPath);

            foreach (string subdirectory in subdirectories)
            {
                string catalogName = Path.GetFileName(subdirectory);
                Catalog catalog = new Catalog
                {
                    Id = _nextId,
                    Name = catalogName,
                    ParentId = parentId
                };
                _context.Catalogs.Add(catalog);
                _context.SaveChanges();

                _nextId++;

                ImportCatalogs(catalog.Id, subdirectory);
            }
        }
    }
}
