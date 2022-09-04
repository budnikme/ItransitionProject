using System.Diagnostics;
using CourseProject.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Web.Controllers;

public class ErrorController: Controller

{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        return View("Error",new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
    
    public IActionResult PageNotFound()
    {
        return View();
    }
    
    public IActionResult Forbidden()
    {
        return View();
    }
}