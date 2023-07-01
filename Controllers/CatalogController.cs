using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Text.Json;
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
                return View("Empty");
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


    public ActionResult Empty() => View();

    public ActionResult ImportFromFile()
    {
        try
        {
            string filePath = "catalogs.json";
            string json = System.IO.File.ReadAllText(filePath);
            List<Catalog> catalogs = JsonSerializer.Deserialize<List<Catalog>>(json);

            _context.Database.ExecuteSqlRaw("DELETE FROM Catalogs");
            _context.SaveChanges();

            foreach (Catalog catalog in catalogs)
            {
                var existingCatalog = _context.Catalogs.Find(catalog.Id);

                if (existingCatalog != null)
                {
                    _context.Entry(existingCatalog).CurrentValues.SetValues(catalog);
                }
                else
                {
                    _context.Add(catalog);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            return View("Error");
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
        string json = JsonSerializer.Serialize(catalogs);
        System.IO.File.WriteAllText("catalogs.json", json);
    }


    public ActionResult ImportFromFolder(string path)
     {
        string rootDirectory = path;
        try
        {
            _context.Database.ExecuteSqlRaw("DELETE FROM Catalogs");
            ImportCatalogs(null, rootDirectory);
            return RedirectToAction("Index", 1);
        }
        catch (Exception ex)
        {
            return View("Error");
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
