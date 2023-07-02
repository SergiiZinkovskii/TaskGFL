using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Diagnostics;
using System.IO;
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
     

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public ActionResult Error(string? errorMessage)
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = errorMessage });
    }
}
