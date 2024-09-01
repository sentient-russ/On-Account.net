using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OnAccount.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> logger;

        public AdminController(ILogger<AdminController> logger)
        {
            this.logger = logger;
        }
        
        [Authorize(Roles = "Manager")]
        public IActionResult Index()
        {
            ViewData["welcome"] = "Welcome to the admin pannel!";
            
            return View();
        }
    }
}
