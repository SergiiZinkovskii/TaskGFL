using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TestGFL.Models;

namespace TestGFL.Controllers
{
    public class FromFileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FromFileController(ApplicationDbContext context)
        {
            _context = context;
        }
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

                return RedirectToAction("Index", "Catalog");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Catalog");
            }
        }

        public ActionResult Export()
        {
            var catalogs = _context.Catalogs.ToList();
            SaveCatalogsToFile(catalogs);
            return RedirectToAction("Index", "Catalog");
        }

        private void SaveCatalogsToFile(List<Catalog> catalogs)
        {
            string json = JsonSerializer.Serialize(catalogs);
            System.IO.File.WriteAllText("catalogs.json", json);
        }
    }
}
