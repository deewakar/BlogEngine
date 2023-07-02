using System.Net.Mime;
using System.Security.AccessControl;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BlogEngine.Models;
using BlogEngine.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BlogEngine.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Post.Include(p => p.Author);
        return View(await applicationDbContext.OrderByDescending(p => p.DatePublished).ToListAsync());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
