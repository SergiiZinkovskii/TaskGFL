using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestGFL.Models;

public class CatalogController : Controller
{
    private readonly ApplicationDbContext _context;

    public CatalogController(ApplicationDbContext context)
    {
        _context = context;
    }

    public ActionResult Index(int? id)
    {
        if (id == null)
        {
            var rootCatalog = _context.Catalogs.FirstOrDefault(c => c.ParentId == null);
            if (rootCatalog == null)
                return View("Error");
            return RedirectToAction("Index", new { id = rootCatalog.Id });
        }
        else
        {
            var catalog = _context.Catalogs.Include(c => c.Children).SingleOrDefault(c => c.Id == id);
            if (catalog == null)
                return View("Error");
            return View(catalog);
        }
    }

    public ActionResult ImportFromFile()
    {
        try
        {
            string filePath = "catalogs.txt"; 
            string[] lines = System.IO.File.ReadAllLines(filePath);

            _context.Database.ExecuteSqlRaw("DELETE FROM Catalogs");

            foreach (string line in lines)
            {
                Catalog catalog = ParseCatalogFromLine(line);
                _context.Catalogs.Add(catalog);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Обробка помилок
            return View("Error");
        }
    }

    private Catalog ParseCatalogFromLine(string line)
    {
        Catalog catalog = new Catalog();

        string[] parts = line.Split(',');

        foreach (string part in parts)
        {
            string[] keyValue = part.Trim().Split(':');

            string key = keyValue[0].Trim();
            string value = keyValue[1].Trim();

            if (key.Equals("Id", StringComparison.OrdinalIgnoreCase))
            {
                catalog.Id = int.Parse(value);
            }
            else if (key.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                catalog.Name = value;
            }
            else if (key.Equals("ParentId", StringComparison.OrdinalIgnoreCase))
            {
                catalog.ParentId = string.IsNullOrEmpty(value) ? null : (int?)int.Parse(value);
            }
        }

        return catalog;
    }


    public ActionResult ImportFromFolder()
    {
        try
        {
            string rootDirectory = "TestGFL"; 
            _context.Database.ExecuteSqlRaw("DELETE FROM Catalogs");
            ImportCatalogs(null, rootDirectory);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Обробка помилок
            return View("Error");
        }
    }


    private void ImportCatalogs(int? parentId, string directoryPath)
    {
            string[] subdirectories = Directory.GetDirectories(directoryPath);

        foreach (string subdirectory in subdirectories)
        {
        
            string catalogName = Path.GetFileName(subdirectory);
            Catalog catalog = new Catalog
            {
                Name = catalogName,
                ParentId = parentId
            };
            _context.Catalogs.Add(catalog);
            _context.SaveChanges();
            ImportCatalogs(catalog.Id, subdirectory);
        }
    }


    public ActionResult Export()
    {
        var catalogs = _context.Catalogs.ToList();
        SaveCatalogsToFile(catalogs);
        return RedirectToAction("Index");
    }


    private void SaveCatalogsToFile(List<Catalog> catalogs)
    {
        using (StreamWriter writer = new StreamWriter("catalogs.txt"))
        {
            foreach (var catalog in catalogs)
            {
                writer.WriteLine($"Id: {catalog.Id}, Name: {catalog.Name}, ParentId: {catalog.ParentId}");
            }
        }
    }
}
